using System;

namespace SLAM
{
	/// <summary>
	/// An event to handle any updates to the robot.
	/// </summary>
	public class RobotUpdateEventArgs : EventArgs
	{
		private Robot robot;

		#region Public Properties

		public Robot Robot
		{
			get
			{
				return robot;
			}
		}

		#endregion

		#region Public Constructors

		public RobotUpdateEventArgs (Robot updatedRobot)
		{
			robot = updatedRobot;
		}

		#endregion
	}
}

