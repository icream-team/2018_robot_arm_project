void setup()
{
  pinMode( 13, OUTPUT );
  pinMode( 12, OUTPUT );
  
  Serial.begin(9600);
}

int state_13 = 0;
int state_12 = 0;
char i = '0';
void loop()
{
    // Serial.write( data );
    // data를 보낸다.
    Serial.write( i );
    Serial.flush();
    i++;
    if ( i > '9' ) i = '0';
    state_13 = !state_13;
    digitalWrite( 13, state_13 );

    // Serial.available()
    // 시리얼 포트로 넘어온 값이 있는지?
    if( Serial.available() )
    {
      state_12 = !state_12;
      digitalWrite( 12, state_12 );
    }

    delay(500);
}
