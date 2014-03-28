using System;

namespace SLAM
{
	/// <summary>
	/// Robot state.
	/// </summary>
	public enum RobotState
	{
		MovingForward,
		MovingBackward,
		RotatingLeft,
		RotatingRight,
		Halted,
		Scanning
	}
}

