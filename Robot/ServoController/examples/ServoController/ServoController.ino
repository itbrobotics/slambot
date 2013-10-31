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
#include <ServoController.h>

ServoController servoController(10);

/************************************************************
* System Functions
************************************************************/

void setup()
{

}

void loop()
{
  servoController.reset(true);
  delay(5000);
  
  for (int angle = 0; angle < 180; angle++)
  {
    servoController.rotate(angle);
    delay(15);
  }

  for (int angle2 = 180; angle2 >= 1; angle2--)
  {
    servoController.rotate(angle2, true);
  }
}
