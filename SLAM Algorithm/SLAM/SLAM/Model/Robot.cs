using System;
using Driver;
using System.Collections.Generic;

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

		// Current x position in the grid.
		private double x;
		// Current y position in the grid.
		private double y;
		// Robot rotation in radians.
		private double heading;
		// Original rotation of the robot.
		private double originalRotation;
		// What are we currently doing?
		private RobotState state;

		// Physical dimensions of the robot in meters.
		private double width;
		private double height;

		// Sensor specific.
		private double mouseCpi;
		private List<double[]> pathPointList = new List<double[]>();

		// Resolution of the sensor in Counts Per Inch (CPI).
		// Event raised whenever there is a change on the robot model, any observers can
		// choose to act upon it.
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
				RaiseRobotUpdate ();
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
				RaiseRobotUpdate ();
			}
		}

		/// <summary>
		/// Gets or sets the rotation of the robot in radians.
		/// </summary>
		/// <value>The new rotation in radians.</value>
		public double Heading
		{
			get
			{
				return heading;
			}
			set
			{
				heading = value;
				RaiseRobotUpdate ();
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
				RaiseRobotUpdate ();
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
				double[] position = new double[3] { x, y, heading * 180 / Math.PI };

				return position;
			}
			set
			{
				// This should be checked to make sure it is actually valid! 
				x = value [0];
				y = value [1];
				heading = value [2];
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


		/// <summary>
		/// Gets or sets the x position of the robot in its environment.
		/// </summary>
		/// <value>The new x position in meters.</value>
		public List<double[]> PathPointList
		{
			get
			{
				return pathPointList;
			}
			set
			{
				pathPointList = value;
				RaiseRobotUpdate ();
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
			heading = 0;
			originalRotation = double.MaxValue;
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
			heading = currentRotation;
			originalRotation = heading;
			state = currentState;
			width = robotWidth;
			height = robotHeight;
			mouseCpi = cpi;
		}

		#endregion

		#region Public Methods

		public void GoFoward ()
		{
			char[] command = { 'w', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.MovingForward;
		}

		public void GoBackward ()
		{
			char[] command = { 's', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.MovingBackward;
		}

		public void RotateLeft ()
		{
			char[] command = { 'a', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.RotatingLeft;
		}

		public void RotateRight ()
		{
			char[] command = { 'd', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.RotatingRight;
		}

		public void Halt ()
		{
			char[] command = { 'q', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.Halted;
		}

		public void Scan ()
		{
			char[] command = { 'e', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.Scanning;
		}

		public void Reset ()
		{
			char[] command = { 'z', '\n' };
			SerialProxy.GetInstance.Send (command);

			state = RobotState.Resetting;
		}

		/// <summary>
		/// Updates the odometry.
		/// </summary>
		/// <param name="e">E.</param>
		public void UpdateOdometry (OdometryUpdateEventArgs e)
		{
			bool raiseEvent = false;

			// First update.
			if (originalRotation == double.MaxValue)
			{
				PathPointList.Add (new double[2] { x, y });
				originalRotation = e.Theta;
				raiseEvent = true;
			}
			else
			{
				// Check if we have changed rotation.
				heading = e.Theta - originalRotation;
				double change = e.Theta - originalRotation;

				// Rotated by more than a degree?
				if (change >= 0.02 || change <= -0.02)
					raiseEvent = true;

				if (heading < 0)
				{
					heading += Math.PI * 2; 
				}
				else if (heading > 2 * Math.PI)
				{
					heading -= Math.PI * 2;
				}

				heading = Math.Round (heading, 2);

				/*				
				 * Calculate the change as follows:
				 *  xm = (displacement / sensor cpi) * conversion of inches to meters
				 *  ym = (displacement / sensor cpi) * conversion of inches to meters
				 */
				double xm = (e.X / mouseCpi) * inchToMeter;
				double ym = (e.Y / mouseCpi) * inchToMeter;

				bool hasMoved = CalculateDisplacement (xm, ym);
			
				if (hasMoved)
					PathPointList.Add (new double[2] { x, y });
					if (!raiseEvent)
						raiseEvent = true;
			}

			if (raiseEvent)
				RaiseRobotUpdate ();
		}

		#endregion
		/*protected void go (){
			double xx;
			bool xy= true;

			xx = x;
			GoForward ();
			do{
				if((xx-x)>=5){
					xy= false;
						}

			}while(xy);

			Halt ();
		}*/


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
		private void RaiseRobotUpdate ()
		{
			RobotUpdateEventArgs args = new RobotUpdateEventArgs (this);
			OnRobotUpdate (args);
		}

		#endregion

		#region Private Methods

		private bool CalculateDisplacement (double xm, double ym)
		{
			// We have moved forwards or backwards.
			if (ym != 0.0)
			{
				// Use Pythagoras's theorem to calculate our x and y displacement
				// relative to our heading.
				x += Math.Round(ym * Math.Sin (heading), 3);
				y += Math.Round(ym * Math.Cos (heading), 3);

				return true;
			}

			return false;
		}

		#endregion

	}
}

