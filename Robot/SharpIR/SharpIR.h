/**
* File: SharpIR.h
*
* 
*
* @author Joshua Michael Daly
* @version 04/12/2013
*/

#ifndef SharpIR_h

#define SharpIR_h
  
#include <Arduino.h>

#define EXPONENT -1
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

  /**
  * Creates a SharpIR object with a user specified pin number 
  * and theretical distance.
  *
  * @param pinNumber pin number the sensor is connected to
  * @param distance theretical distance to use for calculations.
  */
  SharpIR(int pinNumber, int distance);

  /************************************************************
  * Public SharpIR Getters and Setters Prototypes
  ************************************************************/

  /**
  * Gets the theretical distance being used for distance calculations.
  * 
  * @return the current theretical distance
  */ 
  int getThereticalDistance();

  /**
  * Sets the theretical distance to be used for distance calculations.
  *
  * @param distance the theretical distance to use
  */
  void setThereticalDistance(int distance); 

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
  * Theretical distance used during distance calculations.
  */
  int thereticalDistance;

  /**
  * Pin number the Sharp sensor is connected to.
  */
  int pin;

  /************************************************************
  * Private Methods
  ************************************************************/

  void init();
};

#endif
