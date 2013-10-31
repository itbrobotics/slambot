/**
* Example code for the SharpIR libraries getDistance() and
* getRawDistance() functions.
*
* @author Joshua Michael Daly
* @version 30/10/2013
*/

#include <SharpIR.h>

SharpIR sharpIR;

void setup()
{
  Serial.begin(9600);
  sharpIR.setPin(5);
}

void loop()
{
  // Print the distance in millimeters.
  Serial.print("Distance in millimeters: ");
  Serial.println(sharpIR.getDistance());
  Serial.print("Distance in raw units: ");
  Serial.println(sharpIR.getRawDistance());
  delay(2000);
}

