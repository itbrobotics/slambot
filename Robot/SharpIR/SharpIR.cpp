/**
* File: SharpIR.cpp
*
* 
*
* @author Joshua Michael Daly
* @version 27/10/2013
*/

#include <Arduino.h>
#include "SharpIR.h"

/************************************************************
* Public SharpIR Constructors
************************************************************/

/**
* Default constructor.
*/
SharpIR::SharpIR()
{
  this->pin = 0; // Assume analog pin 0.
}

/**
* Constructors a SharpIR object attached to a defined pin.
*
* @param pinNumber pin number the sensor is connected to
*/
SharpIR::SharpIR(int pinNumber)
{
  this->pin = pinNumber;
}

/************************************************************
* Public SharpIR Functions
************************************************************/

/**
* Gets the pin number the Sharp sensor is connected to.
*
* @return pin number the sensor is connected to
*/
int SharpIR::getPin()
{
  return this->pin;
}

/**
* Sets the pin number the Sharp sensor is connected to.
*
* @param pin pin number the sensor is connected to
*/
void SharpIR::setPin(int pinNumber)
{
  this->pin = pinNumber;
}

/**
* Gets the distance reading from the Sharp range finder in millimetres.
*
* @return distance in millimetres
*/
double SharpIR::getDistance()
{
  double volts = analogRead(this->pin) * VALUE_PER_STEP;
  double distance = THERETICAL_DISTANCE * pow(volts, EXPONENT);
  
  return distance * 10;
}

/**
* Gets the distance reading from the Sharp range finder in analog units.
*
* @return distance in analog units
*/
int SharpIR::getRawDistance()
{
  return analogRead(this->pin); 
}
