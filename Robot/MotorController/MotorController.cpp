/**
* File: MotorController.cpp
*
* Provides basic motor functionality for SLAM Robot built 
* using the adafruit motor shield. The robot contains 4
* standard DC motors.
*
* It is possible to drive forward and backwards, turn left and 
* turn right when travelling forward or backwards. All of these 
* actions can be run for an optional set duration or for an
* indefinite provide of time.
*
* @author Joshua Michael Daly
* @version 27/10/2013
*/

#include <AFMotor.h>
#include "MotorController.h"

/*
* Create the 4 motors on the rover, 64KHZ PWN on motor 1 and 2 
* uses more power but generates less electrical noise. Motors 
* 3 and 4 will only run a 1KHZ and will ignore any setting 
* given.
*/
AF_DCMotor motor1(1, MOTOR12_64KHZ); 
AF_DCMotor motor2(2, MOTOR12_64KHZ); 
AF_DCMotor motor3(3); 
AF_DCMotor motor4(4);

/************************************************************
* Motor Functions
************************************************************/

/**
* Default constructor.
*/ 
MotorController::MotorController()
{
  this->setMotorSpeeds();
}

/**
* Sets the speed for all 4 motors.
*/
void MotorController::setMotorSpeeds()
{
  motor1.setSpeed(MOTOR_1_SPEED);
  motor2.setSpeed(MOTOR_2_SPEED);
  motor3.setSpeed(MOTOR_3_SPEED);
  motor4.setSpeed(MOTOR_4_SPEED);
}

/**
* Releases all 4 motors.
*/
void MotorController::releaseAllMotors()
{
  motor1.run(RELEASE);
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  motor4.run(RELEASE);
}

/**
* Drives all 4 motors forward.
*/
void MotorController::driveForward()
{
  motor1.run(FORWARD);
  motor2.run(FORWARD);
  motor3.run(FORWARD);
  motor4.run(FORWARD);
}

/**
* Drives all 4 Motors forward for the duration in milliseconds.
*
* @param duration time to drive forwards in milliseconds
*/
void MotorController::driveForward(unsigned long duration)
{
  driveForward();
  delay(duration);
  releaseAllMotors();
}

/**
* Drives all 4 Motors backward.
*/
void MotorController::driveBackward()
{
  motor1.run(BACKWARD);
  motor2.run(BACKWARD);
  motor3.run(BACKWARD);
  motor4.run(BACKWARD);
}

/**
* Drives all 4 Motors backward for the duration in milliseconds.
*
* @param duration duration time to drive backwards in milliseconds
*/
void MotorController::driveBackward(unsigned long duration)
{
  driveBackward();
  delay(duration);
  releaseAllMotors();
}

/**
* Turns the robot right using motors 1 and 2. Shuts motors 3 
* and 4 down.
*/
boolean MotorController::turnRight(unsigned short path)
{
  if (path != FORWARD && path != BACKWARD)
  {
    Serial.println("Invalid path: " + path);
    
    return false; 
  }
  
  // May need to check state of these and enclose in "if".
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  
  motor1.run(path);
  motor4.run(path);
  
  return true;
}

/**
* Turns the robot right using motors 1 and 2 for the duration in milliseconds.
*
* @param duration duration time to turn right in milliseconds
*/
void MotorController::turnRight(unsigned short path, unsigned long duration)
{
  boolean result = turnRight(path);
  Serial.println(result);
  
  if (result)
  {
    delay(duration);
    motor1.run(RELEASE);
    motor4.run(RELEASE); 
  }
}

/**
* Turns the robot left using motors 3 and 4. Shuts motors 1
* and 2 down.
*/
boolean MotorController::turnLeft(unsigned short path)
{
  if (path != FORWARD && path != BACKWARD)
  {
    Serial.println("Invalid path: " + path);
    
    return false; 
  }
  
  // May need to check state of these and enclose in "if".
  motor1.run(RELEASE);
  motor4.run(RELEASE);
  
  motor2.run(path);
  motor3.run(path);
  
  return true;
}

/**
* Turns the robot left using motors 3 and 4 the duration in milliseconds.
*
* @param duration duration time to turn left for in milliseconds
*/
void MotorController::turnLeft(unsigned short path, unsigned long duration)
{
  boolean result = turnLeft(path);
  Serial.println(result);
  
  if (result)
  {
    delay(duration);
    motor2.run(RELEASE);
    motor3.run(RELEASE); 
  }
}

