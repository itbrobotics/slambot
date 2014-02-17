/**
 * File: Robot.h
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

#define DEBUG 1

// Speed to run motors at.
#define MOTOR_SPEED 255

// IC2 Uno pins.
#define IC2_CLOCK A4
#define IC2_DATA A5

// PS/2 mouse pins.
#define DATA_PIN A2
#define CLOCK_PIN A3

// Sonar pin.
#define SIG_PIN A1

// Message constants.
const char separator = ',';
const char odometryHeader = 'o';
const char scanReadingsHeader = 's';

// For processing commands on the serial line.
const int maxCharacters = 4; 
char command[maxCharacters]; 
int index = 0;

// Odometry data.
char x;          // X displacement.
char y;          // Y displacement.
double theta;     // Current rotation.

double distances[180]; // Stores range finder distances during scans.

// External sensors.
PS2 mouse(CLOCK_PIN, DATA_PIN);
HMC5883L compass;

int error = 0; // Record any errors that may occur in the compass.

Servo sonarServo;

// Motors attached to Adafruit motor controller.
AF_DCMotor motor1(1, MOTOR12_64KHZ);
AF_DCMotor motor2(2, MOTOR12_64KHZ);
AF_DCMotor motor3(3);
AF_DCMotor motor4(4);
