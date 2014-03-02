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
		TurningLeft,
		TuringRight,
		Halted,
		Scanning
	}
}

