/**
 * File: Robot.ino
 * 
 * 
 * 
 * @author Joshua Michael Daly
 * @version 08/03/2014
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
  Serial.setTimeout(500);
  Serial.begin(9600);

  sonarServo.attach(10);

  mouseInit();
  //compassInit();
MoterSpeedSlow();
 
}
void MoterSpeedFast(){
   motor1.setSpeed(MOTOR_SPEED);
  motor2.setSpeed(MOTOR_SPEED);
  motor3.setSpeed(MOTOR_SPEED);
  motor4.setSpeed(MOTOR_SPEED);
  
}
void MoterSpeedSlow(){
  motor1.setSpeed(128);
  motor2.setSpeed(128);
  motor3.setSpeed(128);
  motor4.setSpeed(128);
  
  
}
void loop()
{ 
  if (timeCount == 0.0)
  {
     timeCount = millis(); // Store time since startup.
  }
  
  // Check to see if at least one character is available.
  if (Serial.available()) 
  {
    // Redeclare this every time to clear the buffer.
    char buffer[MAX_CHARACTERS]; 
    
    bytes = Serial.readBytesUntil(terminator, buffer, MAX_CHARACTERS);
    
    if (bytes > 0)
    {
      processCommand(buffer);
    }
  }

  // Read mouse registers.
  mouse.write(0xeb);  // Give me data!
  mouse.read();       // Ignore ack.
  mouse.read();       // Ignore stat.
  x += (char)mouse.read();
  y += (char)mouse.read();

//  // Read compass rotation.
//  MagnetometerScaled scaled = compass.ReadScaledAxis();
//
//  int MilliGauss_OnThe_XAxis = scaled.XAxis; // (or YAxis, or ZAxis)
//
//  // Calculate heading when the magnetometer is level, then correct for signs of axis.
//  theta = atan2(scaled.YAxis, scaled.XAxis);
//
//  // Correct for when signs are reversed.
//  if(theta < 0)
//  {
//    theta += 2 * PI;
//  }
//
//  // Check for wrap due to addition of declination.
//  if(theta > 2 * PI)
//  {
//    theta -= 2 * PI;
//  }

  // To save bombarding the serial line.
  if (millis() - timeCount >= messageRate)
  {
    // Send odometry data to host.
    Serial.print(odometryHeader);
    Serial.print(separator);
    Serial.print(x, DEC);
    Serial.print(separator);
    Serial.print(y, DEC);
    Serial.print(separator);
    Serial.print(theta, DEC);
    Serial.println(); // Message terminated by \n.
    
    timeCount = 0.0;
    x = 0;
    y = 0;
  }
}

/************************************************************
 * Robot Functions
 ************************************************************/

void processCommand(char command[])
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
//    // Second byte is the angle to rotate too,
//    // divide by 100 to get radians.
//    double angle = command[1] / 100;
//    
//    if (command[1] >= 0 && command[1] <= 6.28)
//    {
//      // Rotate the robot to this position.
//    }
//    else
//    {
//      Serial.print("Rotation outside of bounds: ");
//      Serial.println((short)command[1]); 
//    }
    break;
  case 'e':
#if DEBUG
    Serial.println("Scan");
#endif
    halt();
    scan();
    break;
  default: 
    Serial.print("Unknown command \"");
    Serial.print(command[0]);
    Serial.println("\"");
  }  
}

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
  compass = HMC5883L(); // Construct a new HMC5883L compass.
  error = compass.SetScale(0.88); // Set the scale of the compass.

    // Ignore the errors because they don't go away.
//  if (error != 0) // If there is an error, print it out.
//  {
//    Serial.println(compass.GetErrorText(error));
//  }

  error = compass.SetMeasurementMode(Measurement_Continuous); // Set the measurement mode to Continuous

//  if (error != 0) // If there is an error, print it out.
//  {
//    Serial.println(compass.GetErrorText(error));
//  }
}

void getHeading()
{
  // Read compass rotation.
  MagnetometerScaled scaled = compass.ReadScaledAxis();

  int MilliGauss_OnThe_XAxis = scaled.XAxis; // (or YAxis, or ZAxis)

  // Calculate heading when the magnetometer is level, then correct for signs of axis.
  theta = atan2(scaled.YAxis, scaled.XAxis);

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

  for (unsigned char i = 179; i >= 0; i++)
  {
    Serial.print(",");
    Serial.print(distances[i], DEC); 
  }

  Serial.println(); // Message terminated by \n.
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
  distance = ((duration / 2) / 29) / 100;

  return distance;
}

void rotate(double heading)
{
  // Update theta.
  getHeading();
  double angle = heading - theta;
  
  #if DEBUG
    Serial.print("Current Heading = ");
    Serial.println(theta);
  #endif
  
  /*
  * Check to see if our angle has extended beyond the 0/360 degree 
  * line. Our maximum turning arcs from right or left is 180 degrees,
  * so if our angle has crossed the line we must "wrap" it around.
  */
  if (angle < -M_PI)
  {
     angle += M_PI * 2; 
  }
  else if (angle > M_PI)
  {
    angle -= M_PI * 2;
  }
  
  #if DEBUG
    Serial.print("Angle = ");
    Serial.println(angle);
  #endif
  
  // Set up a buffer on either side of our requested angle of 5 degrees.
  // I've done this to stop the robot rotating for a long period of time
  // because it may take several rotations before it reads an extact 
  // match from the compass.
  double leftBuffer = heading + 0.09;
  double rightBuffer = heading - 0.09;
  
  // Wrap around the buffer values if needed.
  if (leftBuffer > M_PI * 2)
  {
    leftBuffer -= M_PI * 2;
  }
  
  if (rightBuffer < -(M_PI * 2))
  {
    rightBuffer += M_PI * 2;
  }
  
  #if DEBUG
    Serial.print("LeftBuffer = ");
    Serial.println(leftBuffer);
    Serial.print("RightBuffer = ");
    Serial.println(rightBuffer);
  #endif
  
  // Our circle is based on a left hand axis where all negative values
  // indicated movement around the left of the circle.
  if (angle < 0)
  {
    Serial.println("Turning Left");
    turnLeft();  
  }
  else if (angle > 0)
  {
    Serial.println("Turning Right");
    turnRight();
  }
  
  while (!(theta >= rightBuffer && theta <= leftBuffer))
  {
    getHeading();
    
    #if DEBUG
      Serial.println(theta);
    #endif
    
    //delay(66); // To run at the 15HZ the HMC5883L works at.
  }

  halt();

  Serial.print("Current Heading = ");
  Serial.println(theta); // Output in degre
}

/************************************************************
 * Motor Functions
 ************************************************************/

void goForward()
{
  MoterSpeedSlow();
  motor1.run(FORWARD);
  motor2.run(FORWARD);
  motor3.run(FORWARD);
  motor4.run(FORWARD);
}

void goBackward()
{
  MoterSpeedSlow();
  motor1.run(BACKWARD);
  motor2.run(BACKWARD);
  motor3.run(BACKWARD);
  motor4.run(BACKWARD);
}

void turnLeft()
{
  MoterSpeedFast();
  motor1.run(BACKWARD);
  motor2.run(FORWARD);
  motor3.run(FORWARD);
  motor4.run(BACKWARD);
}

void turnRight()
{
  MoterSpeedFast();
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
