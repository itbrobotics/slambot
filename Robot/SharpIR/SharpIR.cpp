/**
* File: SharpIR.cpp
*
* 
*
* @author Joshua Michael Daly
* @version 04/12/2013
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
  this->init();
}

/**
* Creates a SharpIR object attached to a defined pin.
*
* @param pinNumber pin number the sensor is connected to
*/
SharpIR::SharpIR(int pinNumber)
{
  this->init();
  this->pin = pinNumber;
}

/**
* Creates a SharpIR object with a user specified pin number 
* and theretical distance.
*
* @param pinNumber pin number the sensor is connected to
* @param distance theretical distance to use for calculations.
*/
SharpIR::SharpIR(int pinNumber, int distance)
{
  this->init();
  this->thereticalDistance = distance;
}

/************************************************************
* Public Getters and Setters
************************************************************/

/**
* Gets the theretical distance being used for distance calculations.
* 
* @return the current theretical distance
*/ 
int SharpIR::getThereticalDistance()
{
  return this->thereticalDistance;
}

/**
* Sets the theretical distance to be used for distance calculations.
*
* @param distance the theretical distance to use
*/
void SharpIR::setThereticalDistance(int distance)
{
  this->thereticalDistance = distance;
}

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
* @return distance in centimetres
*/
double SharpIR::getDistance()
{
  double volts = analogRead(this->pin) * VALUE_PER_STEP;
  double distance = this->thereticalDistance * pow(volts, EXPONENT);
  
  return distance;
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

/************************************************************
* Private Methods
************************************************************/

void SharpIR::init()
{
  this->thereticalDistance = 27; // Suggested theretical distance.
  this->pin = 0; // Assume analog pin 0.
}
