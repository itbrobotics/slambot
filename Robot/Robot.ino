/**
* File: Robot.ino
* 
* This sketch defines the behavior for a "naive" random wandering robot.
* It is simply capable of progressing forward until it encounters an
* object within a pre-defined distance, then using a servo mounted
* sensor it choose to turn left or right. It then continues progressing
* forward if able.
* 
* @author Joshua Michael Daly
* @version 04/12/2013
*/

#include <Servo.h>
#include <AFMotor.h>
#include <SharpIR.h>
#include <MotorController.h>
#include <ServoController.h>

#define DEBUG 1

SharpIR rangeFinder;
MotorController motorController;
ServoController servoController;

int nearestObject;

/************************************************************
* System Functions
************************************************************/

void setup()
{
  #ifdef DEBUG
    Serial.begin(9600);
  #endif
  
  rangeFinder.setPin(5);
  servoController.setPin(10);
}

void loop()
{ 
  nearestObject = 0;
  
  // Don't accept a negative value for the distance.
  while (nearestObject <= 0)
  {
    nearestObject += rangeFinder.getDistance();
  }     

  #if DEBUG
    Serial.print("Distance to object: ");
    Serial.print(nearestObject);
    Serial.println("cm");
  #endif
  
  if (nearestObject > 30) // Object is further than 30cm away.
  {
    motorController.driveForward();
    
    #if !DEBUG
      // Give the Sharp sensor time to respond, this is not
      // needed when sending serial data because of the
      // delay created by those functions.
      delay(10); 
    #endif
  }
  else
  {
    motorController.releaseAllMotors();
    motorController.driveBackward(500);
    
    int distanceLeft = lookLeft();
    int distanceRight = lookRight();
    
    #if DEBUG
      Serial.print("Left Distance: ");
      Serial.print(distanceLeft);
      Serial.println("cm");
      Serial.print("Right Distance: ");
      Serial.print(distanceRight);
      Serial.println("cm");
    #endif
    
    if (distanceLeft > distanceRight)
    {
      #if DEBUG
        Serial.println("Turning Left!");
      #endif
      
      motorController.turnLeft(FORWARD, 1500);
    }
    else
    {
      #if DEBUG
        Serial.println("Turning Right!");
      #endif
      
      motorController.turnRight(FORWARD, 1500);
    }
  }
}

/************************************************************
* Robot Functions
************************************************************/

int lookLeft()
{
  servoController.rotate(150);
  delay(700);
  
  int distance = rangeFinder.getDistance();
  
  servoController.rotate(90);
  delay(700);
  
  return distance;
}

int lookRight()
{
  servoController.rotate(30);
  delay(700);
 
  int distance = rangeFinder.getDistance();
  
  servoController.rotate(90);
  delay(700);
  
  return distance;
}
