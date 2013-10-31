/**
* File: Motor.ino
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
#include <SharpIR.h>
#include "Motor.h"

SharpIR rangeFinder;

/************************************************************
* System Functions
************************************************************/

void setup() 
{
  Serial.begin(9600); // set up Serial library at 9600 bps
  Serial.println("Motor test!");
  
  rangeFinder.setPin(5);
  setMotorSpeeds();
}

void loop() 
{
  int distance = rangeFinder.getDistance();
  
  Serial.println(distance);
  delay(100);
  
  if (distance > 3000)
  {
   driveForward(); 
  }
  else 
  {
    releaseAllMotors();
  }
  
//  driveForward();
//  delay(1000);
//  
//  driveBackward();
//  delay(1000);
//  
//  turnRight(FORWARD);
//  delay(1000);
//  
//  turnRight(BACKWARD);
//  delay(1000);
//  
//  turnLeft(FORWARD);
//  delay(1000);
//  
//  turnLeft(BACKWARD);
//  delay(1000);
//  
//  driveForward(2000);
//  
//  driveBackward(2000);
//  
//  turnRight(FORWARD, 2000);
//  turnRight(BACKWARD, 2000);
//  
//  turnLeft(FORWARD, 2000);
//  turnLeft(BACKWARD, 2000);
//  
//  releaseAllMotors();
//  delay(5000);
}

/************************************************************
* Motor Functions
************************************************************/

/**
* Sets the speed for all 4 motors.
*/
void setMotorSpeeds()
{
  motor1.setSpeed(MOTOR_1_SPEED);
  motor2.setSpeed(MOTOR_2_SPEED);
  motor3.setSpeed(MOTOR_3_SPEED);
  motor4.setSpeed(MOTOR_4_SPEED);
}

/**
* Releases all 4 motors.
*/
void releaseAllMotors()
{
  motor1.run(RELEASE);
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  motor4.run(RELEASE);
}

/**
* Drives all 4 motors forward.
*/
void driveForward()
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
void driveForward(unsigned long duration)
{
  driveForward();
  delay(duration);
  releaseAllMotors();
}

/**
* Drives all 4 Motors backward.
*/
void driveBackward()
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
void driveBackward(unsigned long duration)
{
  driveBackward();
  delay(duration);
  releaseAllMotors();
}

/**
* Turns the robot right using motors 1 and 2. Shuts motors 3 
* and 4 down.
*/
boolean turnRight(unsigned short path)
{
  if (path != FORWARD && path != BACKWARD)
  {
    Serial.println("Invalid path: " + path);
    
    return false; 
  }
  
  // May need to check state of these and enclose in "if".
  motor3.run(RELEASE);
  motor4.run(RELEASE);
  
  motor1.run(path);
  motor2.run(path);
  
  return true;
}

/**
* Turns the robot right using motors 1 and 2 for the duration in milliseconds.
*
* @param duration duration time to turn right in milliseconds
*/
void turnRight(unsigned short path, unsigned long duration)
{
  boolean result = turnRight(path);
  Serial.println(result);
  
  if (result)
  {
    delay(duration);
    motor1.run(RELEASE);
    motor2.run(RELEASE); 
  }
}

/**
* Turns the robot left using motors 3 and 4. Shuts motors 1
* and 2 down.
*/
boolean turnLeft(unsigned short path)
{
  if (path != FORWARD && path != BACKWARD)
  {
    Serial.println("Invalid path: " + path);
    
    return false; 
  }
  
  // May need to check state of these and enclose in "if".
  motor1.run(RELEASE);
  motor2.run(RELEASE);
  
  motor3.run(path);
  motor4.run(path);
  
  return true;
}

/**
* Turns the robot left using motors 3 and 4 the duration in milliseconds.
*
* @param duration duration time to turn left for in milliseconds
*/
void turnLeft(unsigned short path, unsigned long duration)
{
  boolean result = turnLeft(path);
  Serial.println(result);
  
  if (result)
  {
    delay(duration);
    motor3.run(RELEASE);
    motor4.run(RELEASE); 
  }
}

