/**
* File: SharpIR.h
*
* 
*
* @author Joshua Michael Daly
* @version 27/10/2013
*/

#ifndef SharpIR_h

#define SharpIR_h
  
#include <Arduino.h>

#define EXPONENT -1
#define THERETICAL_DISTANCE 27
#define VALUE_PER_STEP 0.0048828125

class SharpIR
{
  public:

  /************************************************************
  * Public SharpIR Constructors
  ************************************************************/

  /**
  * Default constructor.
  */
  SharpIR();

  /**
  * Creates a SharpIR object attached to a defined pin.
  *
  * @param pinNumber pin number the sensor is connected to
  */
  SharpIR(int pinNumber);

  /************************************************************
  * Public SharpIR Function Prototypes
  ************************************************************/

  /**
  * Gets the pin number the Sharp sensor is connected to.
  *
  * @return pin number the sensor is connected to
  */
  int getPin();

  /**
  * Sets the pin number the Sharp sensor is connected to.
  *
  * @param pinNumber pin number the sensor is connected to
  */
  void setPin(int pinNumber);

  /**
  * Gets the distance reading from the Sharp range finder in millimetres.
  *
  * @return distance in millimetres
  */
  double getDistance();

  /**
  * Gets the distance reading from the Sharp range finder in analog units.
  *
  * @return distance in analog units
  */
  int getRawDistance();

  private:

  /************************************************************
  * Private SharpIR Attributes
  ************************************************************/

  /**
  * Pin number the Sharp sensor is connected to.
  */
  int pin;
};

#endif
