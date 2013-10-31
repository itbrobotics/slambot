/**
* File: Robot.ino
*
* 
*
* @author Joshua Michael Daly
* @version 28/10/2013
*/

#include <Servo.h>
#include <AFMotor.h>
#include <SharpIR.h>
#include <MotorController.h>
#include <ServoController.h>

SharpIR rangeFinder(5);
MotorController motorController;
ServoController servoController(10);

int nearestObject = 0;

/************************************************************
* System Functions
************************************************************/

void setup()
{
  Serial.begin(9600);
}

void loop()
{
  int distance = rangeFinder.getDistance();
  
  Serial.print("Distance to object: ");
  Serial.println(distance);
  
//  nearestObject = rangeFinder.getDistance();
//  
//  if (nearestObject > 3000)
//  {
//    motorController.driveForward();
//  }
//  else
//  {
//    motorController.releaseAllMotors();
//    motorController.driveBackward(500);
//    
//    int distanceLeft = lookLeft();
//    int distanceRight = lookRight();
//    
//    if (distanceLeft > distanceRight)
//    {
//      motorController.turnLeft(FORWARD, 1000);
//    }
//    else
//    {
//      motorController.turnRight(FORWARD, 1000);
//    }
//  }
}

/************************************************************
* Robot Functions
************************************************************/

int lookLeft()
{
  servoController.rotate(150, true);
  int distance = rangeFinder.getDistance();
  servoController.rotate(90, true);
  
  return distance;
}

int lookRight()
{
  servoController.rotate(30, true);
  int distance = rangeFinder.getDistance();
  servoController.rotate(90, true);
  
  return distance;
}
