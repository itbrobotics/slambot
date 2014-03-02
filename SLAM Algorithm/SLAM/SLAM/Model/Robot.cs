using System;

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
		private double x; // Current x position in the grid.
		private double y; // Current y position in the grid.
		private double rotation; // Robot rotation in radians.
		private RobotState state; // What are we currently doing?

		// Physical dimensions of the robot in meters.
		private double width;
		private double height;

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

		#endregion

		#region Public Constructors

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
			double robotWidth, double robotHeight)
		{
			x = xPosition;
			y = yPosition;
			rotation = currentRotation;
			state = currentState;
			width = robotWidth;
			height = robotHeight;
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

