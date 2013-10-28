#include <SharpIR.h>

SharpIR sharpIR;

void setup()
{
  sharpIR.setPin(1);
}

void loop()
{
  Serial.println(sharpIR.getPin());
}

