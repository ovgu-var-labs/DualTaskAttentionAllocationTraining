#include "Servo.h"  

Servo servo1; 
Servo servo2;
Servo servo3;
Servo servo4;
Servo servo5;
Servo servo6;
Servo servo7;
Servo servo8;

Servo servoMask1;
Servo servoMask2;

int firstServo = 0;
int secondServo = 0;
int lastServo = 0;
int palpationTime = 6000;
unsigned long timer = 0;
unsigned long mask1Timer = 0;
unsigned long mask2Timer = 0;
bool runTask = false;
bool runDemo = false;
int angle = 0;
int multiplier = 1;

bool servosReset = false;

void setup()
{
  Serial.begin(9600); 
  
  servo1.attach(6);  
  servo2.attach(7);
  servo3.attach(10);
  servo4.attach(12);
  servo5.attach(5);
  servo6.attach(8);
  servo7.attach(9);
  servo8.attach(11);

  servoMask1.attach(3);
  servoMask2.attach(2);
}

void loop()
{ 
  switch (Serial.read()) {
    case 'S':
      runTask = true;
      break;
    case 'O':
      runTask = false;
      runDemo = false;
      break;
    case 'D':
      runDemo = true;
      break;
  }

  if(!runDemo && !runTask && !servosReset) {
      servo1.write(110); 
      servo2.write(90);
      servo3.write(80);
      servo4.write(80);
      servo5.write(90);
      servo6.write(90);
      servo7.write(90);
      servo8.write(90);

      servosReset = true;
  }

  if (runDemo) {
    servosReset = false;
    servo1.write(60); 
    servo2.write(60);    
    servo3.write(130);    
    servo4.write(130);    
    servo5.write(60);    
    servo6.write(50);   
    servo7.write(60);   
    servo8.write(60);
  }
  
  if(runTask) {
    servosReset = false;
    if(millis() - mask1Timer > 3200) {
        mask1Timer = millis();
        angle =  multiplier*30 + angle;
        if(angle >= 180) {
          multiplier = -1;
          angle = 180;
        }
        else if(angle <= 0) {
          multiplier = 1;
          angle = 0;
        }
        servoMask1.write(angle);
      }

    if(millis() - mask2Timer > 4500) {
        mask2Timer = millis();
        angle =  multiplier*30 + angle;
        if(angle >= 180) {
          multiplier = -1;
          angle = 180;
        }
        else if(angle <= 0) {
          multiplier = 1;
          angle = 0;
        }
        servoMask2.write(angle);
      }
    
    if(millis() - timer > palpationTime/2) {
      timer = millis();
      lastServo = secondServo;
      secondServo = firstServo;
      while(lastServo == firstServo || secondServo == firstServo) {
        firstServo = random(1,9);
      } 

      if(secondServo != 1) {
        servo1.write(110); 
      }
      if(secondServo != 2) {
        servo2.write(90);
      }
      if(secondServo != 3) {
        servo3.write(80);
      }
      if(secondServo != 4) {
        servo4.write(80);
      }
      if(secondServo != 5) {
        servo5.write(90);
      }
      if(secondServo != 6) {
        servo6.write(90);
      }
      if(secondServo != 7) {
        servo7.write(90);
      }
      if(secondServo != 8) {
        servo8.write(90);
      }

      while (millis()-timer < 1000) {}
      Serial.println(String(firstServo));

      if(firstServo==1) {
        servo1.write(60); 
      }
      else if(firstServo == 2) {
        servo2.write(60);
      }
      else if(firstServo == 3) {
        servo3.write(130);
      }
      else if(firstServo == 4) {
        servo4.write(130);
      }
      else if(firstServo == 5) {
        servo5.write(60);
      }
      else if(firstServo == 6) {
        servo6.write(50);
      }
      else if(firstServo == 7) {
        servo7.write(50);
      }
      else if(firstServo == 8) {
        servo8.write(60);
      }
      timer = millis();
    } 
  }
}
