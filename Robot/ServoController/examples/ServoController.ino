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

// Cannot be included until boolean problem with headers is
// resolved.
//#include "ServoController.h"

/*
* Create servo object to control a servo a maximum of eight 
* servo objects can be created.
*/
Servo servo;
 
int servoPosition = 0; // Variable to store the servo position. 

/************************************************************
* System Functions
************************************************************/

void setup()
{
  servo.attach(10);
}

void loop()
{
  reset(true);
  delay(5000);
  
  for(servoPosition = 0; servoPosition < 180; servoPosition += 1)  // goes from 0 degrees to 180 degrees 
  {                                  // in steps of 1 degree 
    rotate(servoPosition);              // tell servo to go to position in variable 'pos' 
    delay(15);                       // waits 15ms for the servo to reach the position 
  } 
  
  for(servoPosition = 180; servoPosition >= 1; servoPosition -= 1)     // goes from 180 degrees to 0 degrees 
  {                                
    rotate(servoPosition, true);
  } 
}

/************************************************************
* Servo Functions
************************************************************/

/**
* Resets the servo to 0 degrees.
*/ 
void reset()
{
  servo.write(0);
}

/**
* Resets the servo to 0 degrees optionally waiting for the 
* servo to reach its position.
*
* @param wait  if true the function will not return until the
*              servo has reached 0 degrees.
*/
void reset(boolean wait)
{
  reset();
  
  while (servo.read() != 0)
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
void rotate(unsigned short angle)
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
void rotate(unsigned short angle, boolean wait)
{
  servo.write(angle);
  
  while (servo.read() != angle)
  { 
    // Busy waiting. 
  }
}
