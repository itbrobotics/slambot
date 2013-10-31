/**
* File: ServoController.ino
*
* Provides some basic servo functionality, built upon the 
* servo example that came with the arduino IDE.
*
* The servo object can be rotated to a given angle, based on
* on the parameters passed to rotate it will either wait for
* the servo to reach its position or continue on.
*
* Reset simply returns the servo to 0 degrees with or without
* waiting for it, again based on the parameters passed.
*
* @author Joshua Michael Daly
* @version 27/10/2013
*/

#include <Servo.h>
#include "ServoController.h"

/************************************************************
* Public ServoController Constructors
************************************************************/

ServoController::ServoController()
{
  this->servo.attach(0); // Assume pin 0.
  this->servoPosition = this->servo.read();  
}

ServoController::ServoController(int pin)
{
  this->servo.attach(pin); // Assume pin 0.
  this->servoPosition = this->servo.read();  
}

/************************************************************
* Public SharpIR Functions
************************************************************/

/**
* Resets the servo to 0 degrees.
*/ 
void ServoController::reset()
{
  this->servo.write(0);
}

/**
* Resets the servo to 0 degrees optionally waiting for the 
* servo to reach its position.
*
* @param wait  if true the function will not return until the
*              servo has reached 0 degrees.
*/
void ServoController::reset(boolean wait)
{
  reset();
  
  while (this->servo.read() != 0)
  {
    // Busy waiting.
  } 
}

/**
* Rotates the servo to the angle specified without waiting for
* the servo to reach it.
*
* @param angle  position to move the servo to
*/
void ServoController::rotate(unsigned short angle)
{
  servo.write(angle); 
}

/**
* Rotates the servo to the angle specified optionally waiting for
* the servo to reach its position.
*
* @param angle  position to move the servo to
* @param wait   if true the function will not return until the
*               servo has reached angle.
*/
void ServoController::rotate(unsigned short angle, boolean wait)
{
  servo.write(angle);
  
  while (servo.read() != angle)
  { 
    // Busy waiting. 
  }
}
