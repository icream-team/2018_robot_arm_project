#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "MPU6050.h"

#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

#define OUTPUT_READABLE_QUATERNION
#define OUTPUT_READABLE_YAWPITCHROLL

#define vibe1 A2
#define vibe2 A3
#define MAGNET1 7
#define MAGNET2 3
#define MAGNET3 4
#define MAGNET4 5
#define MAGNET5 6

MPU6050 mpu;

bool dmpReady = false;
uint8_t mpuIntStatus;
uint8_t devStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];

Quaternion q;
VectorInt16 aa;
VectorInt16 aaReal;
VectorInt16 aaWorld;
VectorFloat gravity;

float euler[3];
float ypr[3];

uint8_t teapotPacket[14] = { '$', 0x02, 0,0, 0,0, 0,0, 0,0, 0x00, 0x00, '\r', '\n' };

volatile bool mpuInterrupt = false;

void dmpDataReady()
{
    mpuInterrupt = true;
}

int vibrationcommand; // 진동 커맨드
boolean finish = false; // 진동이 끝났는가
unsigned long preTime = 0; // 이전 기록 시간
int continuationTime = 500; // 진동 지속 시간

void setup()
{
    pinMode( vibe1, OUTPUT );
    pinMode( vibe2, OUTPUT );
    pinMode( MAGNET1, INPUT );
    pinMode( MAGNET2, INPUT );
    pinMode( MAGNET3, INPUT );
    pinMode( MAGNET4, INPUT );
    pinMode( MAGNET5, INPUT );
    
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
        Wire.begin();
        TWBR = 24; // 400kHz I2C clock (200kHz if CPU is 8MHz)
    #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
        Fastwire::setup(400, true);
    #endif

    Serial.begin(9600);

    while(!Serial);

    Serial.println( F("Initializing I2C device..."));
    mpu.initialize();

    Serial.println(F("Testing device connections..."));
    Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));

    Serial.println(F("\nSend any character to begin DMP programming and demo: "));
    while(Serial.available() && Serial.read());
    while(!Serial.available());
    while(Serial.available()&&Serial.read());

    Serial.println(F("Initializing DMP..."));
    devStatus = mpu.dmpInitialize();

    mpu.setXGyroOffset(0);
    mpu.setYGyroOffset(0);
    mpu.setZGyroOffset(0);
    mpu.setZAccelOffset(0);

    if(devStatus==0)
    {
        Serial.println(F("Enabling DMP..."));
        mpu.setDMPEnabled(true);

        Serial.println(F("Enabling interrupt detection (Arduino external interrupt 0)..."));
        attachInterrupt(0, dmpDataReady, RISING);
        mpuIntStatus = mpu.getIntStatus();

        Serial.println(F("DMP ready! Waiting for first interrupt..."));
        dmpReady = true;

        packetSize = mpu.dmpGetFIFOPacketSize();
    }
    else
    {
        Serial.print(F("DMP Initialization failed (code "));
        Serial.print(devStatus);
        Serial.println(F(")"));
    }

}



void timecheck()
{
    unsigned long curTime = millis();

    if( curTime > preTime + continuationTime )
    {
        analogWrite( vibe1, 0 );
        analogWrite( vibe2, 0 );

        finish = true;
    }
}

void send_pos_and_ang()
{
    if ((mpuIntStatus & 0x10) || fifoCount == 1024)
    {
        // reset so we can continue cleanly
        mpu.resetFIFO();
        Serial.println(F("FIFO overflow!"));

        // otherwise, check for DMP data ready interrupt (this should happen frequently)
    } else if (mpuIntStatus & 0x02)
    {
        // wait for correct available data length, should be a VERY short wait
        while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

        // read a packet from FIFO
        mpu.getFIFOBytes(fifoBuffer, packetSize);

        // track FIFO count here in case there is > 1 packet available
        // (this lets us immediately read more without waiting for an interrupt)
        fifoCount -= packetSize;

        #ifdef OUTPUT_READABLE_QUATERNION
        // display quaternion values in easy matrix form: w x y z
        mpu.dmpGetQuaternion(&q, fifoBuffer);
        Serial.print("POX");
        Serial.println(q.w);

        Serial.print("POY");
        Serial.println(q.x);

        Serial.print("POZ");
        Serial.println(q.z);

        Serial.flush();
        #endif

        #ifdef OUTPUT_READABLE_YAWPITCHROLL
        // display Euler angles in degrees
        mpu.dmpGetQuaternion(&q, fifoBuffer);
        mpu.dmpGetGravity(&gravity, &q);
        mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
        Serial.print("ANX");
        Serial.println( int( ypr[0] * 180 / M_PI ) );

        Serial.print("ANY");
        Serial.println( int( ypr[1] * 180 / M_PI ) );

        Serial.print("ANZ");
        Serial.println( int( ypr[2] * 180 / M_PI ) );

        Serial.flush();

        #endif
    }
}

void convension_serial_message( char * message )
{
    if ( message[0] == "V" && message[1] == "I" )
    {
        vibration_signal( (int)message[2] );
    }
}

void vibration_signal( int type )
{
    preTime = millis();
    finish = false;

    if( type == 1 )
    {
        analogWrite( vibe1, 100 );
        analogWrite( vibe2, 100 );
        continuationTime = 250;
    }
    else if( type == 2 )
    {
        analogWrite( vibe1, 300 );
        analogWrite( vibe2, 300 );
        continuationTime = 150;
    }
    else if( type == 3 )
    {
        analogWrite( vibe1, 200 );
        analogWrite( vibe2, 200 );
        continuationTime = 500;
    }
}

void loop()
{
    timecheck();

    if (!dmpReady) return;

    send_pos_and_ang();

    mpuIntStatus = mpu.getIntStatus();

    fifoCount = mpu.getFIFOCount();

    if(Serial.available())
    {
      char temp_value[10];
      char temp = 'a';
      for( int i = 0; i < 10; i++ )
      {
        temp  = Serial.read();

        if( !temp ) break;
        temp_value[i] = Serial.read();
      }

        convension_serial_message( temp_value );
    }
}

