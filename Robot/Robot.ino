/**
 * File: Robot.ino
 * 
 * 
 *  
 * @author Joshua Michael Daly
 * @version 17/02/2014
 */

#include <ps2.h>
#include <Wire.h>
#include <Servo.h>
#include <AFMotor.h>
#include <HMC5883L.h>
#include "Robot.h"

/************************************************************
 * Arduino Functions
 ************************************************************/

void setup()
{
  Serial.begin(9600);

  sonarServo.attach(10);

  mouseInit();
  compassInit();

  motor1.setSpeed(MOTOR_SPEED);
  motor2.setSpeed(MOTOR_SPEED);
  motor3.setSpeed(MOTOR_SPEED);
  motor4.setSpeed(MOTOR_SPEED);
}

void loop()
{ 
  // Check to see if at least one character is available.
  if (Serial.available()) 
  {
    char character = Serial.read();

    if (character == ';' || index >= maxCharacters)
    {
      // Here we have a full buffer or a terminator.
      processCommand();
      index = 0;
    }
    else
    {
      command[index] = character;
      index++;
    }
  }

  // Read mouse registers.
  mouse.write(0xeb);  // Give me data!
  mouse.read();       // Ignore ack.
  mouse.read();       // Ignore stat.
  x = mouse.read();
  y = mouse.read();

  // Read compass rotation.
  MagnetometerScaled scaled = compass.ReadScaledAxis();

  int MilliGauss_OnThe_XAxis = scaled.XAxis; // (or YAxis, or ZAxis)

  // Calculate heading when the magnetometer is level, then correct for signs of axis.
  float theta = atan2(scaled.YAxis, scaled.XAxis);

  // Correct for when signs are reversed.
  if(theta < 0)
  {
    theta += 2 * PI;
  }

  // Check for wrap due to addition of declination.
  if(theta > 2 * PI)
  {
    theta -= 2 * PI;
  }

  // Send odometry data to host.
  Serial.print(odometryHeader);
  Serial.print(separator);
  Serial.print(x, DEC);
  Serial.print(separator);
  Serial.print(y, DEC);
  Serial.print(separator);
  Serial.print(theta, DEC);
  Serial.println(); // Message terminated by CR/LF.
}

/************************************************************
 * Robot Functions
 ************************************************************/

void processCommand()
{
  switch (tolower(command[0]))
  {
  case 'w':
#if DEBUG
    Serial.println("Forward");
#endif
    goForward();
    break;
  case 's':
#if DEBUG
    Serial.println("Backward");
#endif
    goBackward();
    break;
  case 'a':
#if DEBUG
    Serial.println("Left");
#endif
    turnLeft();
    break;
  case 'd':
#if DEBUG
    Serial.println("Right");
#endif
    turnRight();
    break;  
  case 'q':
#if DEBUG
    Serial.println("Halt");
#endif
    halt();
    break;
  case 'r':
    // Limits of the servo. Second byte is the angle to rotate to.
    if (command[1] >= 0 && command[1] <= 180)
    {
      sonarServo.write(command[1]);
    }
    else
    {
      Serial.print("Rotation outside bounds of servo: ");
      Serial.println((short)command[1]); 
    }
    break;
  case 'e':
#if DEBUG
    Serial.println("Scan");
#endif
    scan();
    break;
  default: 
    Serial.print("Unknown command \"");
    Serial.print(command[0]);
    Serial.println("\"");
    break;
  }  
}

/************************************************************
 * Sensor Functions
 ************************************************************/

void mouseInit()
{
  mouse.write(0xff);  // Reset.
  mouse.read();       // Ack byte.
  mouse.read();       // Blank. 
  mouse.read();       // Blank. 
  mouse.write(0xf0);  // Remote mode.
  mouse.read();       // Ack byte.
  delayMicroseconds(100);
}

void compassInit()
{
  compass = HMC5883L(); // Construct a new HMC5883 compass.
  error = compass.SetScale(0.88); // Set the scale of the compass.

  if (error != 0) // If there is an error, print it out.
  {
    Serial.println(compass.GetErrorText(error));
  }

  error = compass.SetMeasurementMode(Measurement_Continuous); // Set the measurement mode to Continuous

    if (error != 0) // If there is an error, print it out.
  {
    Serial.println(compass.GetErrorText(error));
  }
}

void scan()
{
  for (unsigned char pos = 0; pos < 180; pos++) // Goes from 0 degrees to 180 degrees 
  {                                             // in steps of 1 degree.            
    sonarServo.write(pos);   
    delay(25);                                  // Waits 25ms for the servo to reach the position.
    distances[pos] = takeReading();             // Store sonar reading at current position.
  } 

  sonarServo.write(90);

  // Send readings back to the host.
  Serial.print(scanReadingsHeader);

  for (int i = 0; i < 180; i++)
  {
    Serial.print(",");
    Serial.print(distances[i], DEC); 
  }

  Serial.println(); // Message terminated by CR/LF.
}

double takeReading()
{
  double distance; // Distance to object.
  double duration; // Duration of pulse from PING))).

  // The PING))) is triggered by a HIGH pulse of 2 or more microseconds.
  // Give a short LOW pulse beforehand to ensure a clean HIGH pulse.
  pinMode(SIG_PIN, OUTPUT);
  digitalWrite(SIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(SIG_PIN, HIGH);
  delayMicroseconds(5);
  digitalWrite(SIG_PIN, LOW);

  // The same pin is used to read the signal from the PING))): a HIGH
  // pulse whose duration is the time (in microseconds) from the sending
  // of the ping to the reception of its echo off of an object.
  pinMode(SIG_PIN, INPUT);
  duration = pulseIn(SIG_PIN, HIGH);

  // The speed of sound is 340 m/s or 29 microseconds per centimeter.
  // The ping travels out and back, so to find the distance of the
  // object we take half of the distance travelled.
  distance = duration / 29 / 2;

  return distance;
}

/************************************************************
 * Motor Functions
 ************************************************************/

void goForward()
{
  motor1.run(FORWARD);
  motor2.run(FORWARD);
  motor3.run(FORWARD);
  motor4.run(FORWARD);
}

void goBackward()
{
  motor1.run(BACKWARD);
  motor2.run(BACKWARD);
  motor3.run(BACKWARD);
  motor4.run(BACKWARD);
}

void turnLeft()
{
  motor1.run(BACKWARD);
  motor2.run(FORWARD);
  motor3.run(FORWARD);
  motor4.run(BACKWARD);
}

void turnRight()
{
  motor1.run(FORWARD);
  motor2.run(BACKWARD);
  motor3.run(BACKWARD);
  motor4.run(FORWARD);
}

void halt()
{
  motor1.run(RELEASE);
  motor2.run(RELEASE);
  motor3.run(RELEASE);
  motor4.run(RELEASE);
}
