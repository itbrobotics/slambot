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
  
}

/************************************************************
* Public SharpIR Functions
************************************************************/

/**
* Sets the digital pin of the servo.
*
* @ param pin digital pin servo is attached to
*/
void ServoController::setPin(int pin)
{
  this->servo.attach(pin);
}

/**
* Gets the current position of the servo.
*
* @return servo position
*/
 int ServoController::getPosition()
{
  return this->servo.read();
}

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
  this->reset();
  
  while (this->servo.read() != 0)
  {
    delay(100);
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
  this->servo.write(angle); 
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
  this->servo.write(angle);
  
  while (this->servo.read() != angle)
  { 
    delay(100); 
  }
}
