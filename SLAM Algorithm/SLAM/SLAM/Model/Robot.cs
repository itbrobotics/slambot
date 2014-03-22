using System;
using Driver;

namespace SLAM
{
	/// <summary>
	/// Contains the exploration robot model as part of the Model-View-Controller (MVC)
	/// design pattern. This class does not contain any view specific code instead it acts as
	/// a state model for the robot recording its current x and y position, rotation, and state.
	/// 
	/// Whenever there is a change to the robot a <code>RobotUpdateEventArgs</code> is raised
	/// to inform any observers that there has been an update to the robot's state.
	/// </summary>
	public class Robot
	{
		private const double inchToMeter = 0.0254;

		private double x; // Current x position in the grid.
		private double y; // Current y position in the grid.
		private double rotation; // Robot rotation in radians.
		private double relativeRotation; // Robot rotation relative to the earth's magnetic field.
		private RobotState state; // What are we currently doing?

		// Physical dimensions of the robot in meters.
		private double width;
		private double height;

		// Sensor specific.
		private double mouseCpi; // Resolution of the sensor in Counts Per Inch (CPI).
	
		// Event raised whenever there is a change on the robot model, any observers can 
		// choose to act upon the changes.
		public event EventHandler<RobotUpdateEventArgs> RobotUpdated;

		#region Public Properties

		/// <summary>
		/// Gets or sets the x position of the robot in its environment.
		/// </summary>
		/// <value>The new x position in meters.</value>
		public double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
				RaiseUpdateRobot ();
			}
		}

		/// <summary>
		/// Gets or sets the y position of the robot in its environment.
		/// </summary>
		/// <value>The new y position in meters.</value>
		public double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
				RaiseUpdateRobot ();
			}
		}

		/// <summary>
		/// Gets or sets the rotation of the robot in radians.
		/// </summary>
		/// <value>The new rotation in radians.</value>
		public double Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
				RaiseUpdateRobot ();
			}
		}

		/// <summary>
		/// Gets or sets the state of the robot <see cref="SLAM.RobotState"/>.
		/// </summary>
		/// <value>The new state of the robot.</value>
		public RobotState State
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
				RaiseUpdateRobot ();
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width of the robot in meters.</value>
		public double Width
		{
			get
			{
				return width;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height of the robot in meters.</value>
		public double Height
		{
			get
			{
				return height;
			}
		}

		/// <summary>
		/// Gets the robot's position as an array containing x, y, and rotation.
		/// </summary>
		/// <value>The position as an array.</value>
		public double[] Position 
		{
			get
			{
				// We are returning the rotation as degrees here because the EKF SLAM
				// algorithm takes degrees not radians.
				double[] position = new double[3] { x, y, rotation * 180 / Math.PI };

				return position;
			}
		}

		/// <summary>
		/// Gets or sets the cpi of the mouse sensor.
		/// </summary>
		/// <value>The cpi resolution.</value>
		public double Cpi
		{
			get
			{
				return mouseCpi;
			}
			set
			{
				mouseCpi = value;
			}
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.Robot"/> class with the
		/// default settings for the Pirate robot platform.
		/// </summary>
		public Robot ()
		{
			x = 0;
			y = 0;
			rotation = 0;
			relativeRotation = -7;
			state = RobotState.Halted;
			width = 0.18;
			height = 0.23;
			mouseCpi = 800;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.Robot"/> class.
		/// </summary>
		/// <param name="xPosition">X position in meters.</param>
		/// <param name="yPosition">Y position in meters.</param>
		/// <param name="currentRotation">Current rotation in radians.</param>
		/// <param name="currentState">Current state <see cref="SLAM.RobotState"/>.</param>
		/// <param name="robotWidth">Robot width in meters.</param>
		/// <param name="robotHeight">Robot height in meters.</param>
		public Robot (double xPosition, double yPosition, double currentRotation, RobotState currentState,
			double robotWidth, double robotHeight, double cpi)
		{
			x = xPosition;
			y = yPosition;
			rotation = currentRotation;
			relativeRotation = -7;
			state = currentState;
			width = robotWidth;
			height = robotHeight;
			mouseCpi = cpi;
		}

		#endregion

		#region Public Methods

		public void GoFoward ()
		{

		}

		public void GoBackward ()
		{

		}

		public void RotateLeft ()
		{

		}

		public void RotateRight ()
		{

		}

		public void Halt ()
		{

		}

		public void Scan ()
		{

		}

		/// <summary>
		/// Updates the odometry.
		/// </summary>
		/// <param name="e">E.</param>
		public void UpdateOdometry (OdometryUpdateEventArgs e)
		{
			bool raiseEvent = false;

			/*
			 * Calculate the change as follows:
			 *  xm = (displacement / sensor cpi) * conversion of inches to meters
			 *  ym = (displacement / sensor cpi) * conversion of inches to meters
			 */
			double xm = (e.X / mouseCpi) * inchToMeter;
			double ym = (e.Y / mouseCpi) * inchToMeter;

			// We have moved along the x-axis.
			if (xm != 0.0)
			{
				x += xm;

				if (!raiseEvent)
					raiseEvent = true;
			}

			// We have moved along the y-axis.
			if (ym != 0.0)
			{
				y += ym;

				if (!raiseEvent)
					raiseEvent = true;
			}

			// On the first odometry report the value will be minus -7.
			// We do not care about the rotation of the robot relative
			// to the earth's magnetic field. We only want the change.
			if (relativeRotation == -7)
			{
				rotation = e.Theta;

				if (!raiseEvent)
					raiseEvent = true;
			}
			else
			{
				double change = e.Theta - relativeRotation;
				relativeRotation = e.Theta;

				if (change >= 0.02) // Radians, or 1 degree.
				{
					rotation = e.Theta;

					// Correct for when signs are reversed.
					if (rotation < 0)
					{
						rotation += 2 * Math.PI;
					}

					// Check for wrap due to addition of declination.
					if (rotation > 2 * Math.PI)
					{
						rotation -= 2 * Math.PI;
					}

					if (!raiseEvent)
						raiseEvent = true;
				}
			}

			if (raiseEvent)
				RaiseUpdateRobot ();
		}

		#endregion

		#region Protected Event Handlers

		/// <summary>
		/// Raises the robot update event.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnRobotUpdate (RobotUpdateEventArgs e)
		{
			if (RobotUpdated != null)
			{
				RobotUpdated (this, e);
			}
		}

		#endregion

		#region Private Event Handlers

		/// <summary>
		/// Raises the update robot event.
		/// </summary>
		private void RaiseUpdateRobot ()
		{
			RobotUpdateEventArgs args = new RobotUpdateEventArgs (this);
			OnRobotUpdate (args);
		}

		#endregion
	}
}

