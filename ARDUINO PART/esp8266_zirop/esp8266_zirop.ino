#include <Wire.h>
void setup() {
// put your setup code here, to run once:
Wire.begin();
Serial.begin(9600);
Wire.beginTransmission(0x68);
Wire.write(0x6B);
Wire.write(0x0);
Wire.endTransmission();
}
void loop() {
Wire.beginTransmission(0x68);
Wire.write(0x3B);
Wire.endTransmission();
int ax = 0, ay = 0, az = 0;
int gx = 0, gy = 0, gz = 0;
int temp = 0;
Wire.requestFrom(0x68, 14);
ax = Wire.read() << 8;
ax |= Wire.read();
ay = Wire.read() << 8;
ay |= Wire.read();
az = Wire.read() << 8;
az |= Wire.read();
temp = Wire.read() << 8;
temp |= Wire.read();
gx = Wire.read() << 8;
gx |= Wire.read();
gy = Wire.read() << 8;
gy |= Wire.read();
gz = Wire.read() << 8;
gz |= Wire.read();
Serial.print("AX : ");
Serial.print(ax);
Serial.print(" AY : ");
Serial.print(ay);
Serial.print(" AZ : ");
Serial.print(az);
Serial.print(" GX : ");
Serial.print(gx);
Serial.print(" GY : ");
Serial.print(gy);
Serial.print(" GZ : ");
Serial.println(gz);
delay(500);
}
