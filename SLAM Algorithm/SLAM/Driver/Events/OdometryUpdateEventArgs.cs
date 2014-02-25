using System;

namespace Driver
{
	public class OdometryUpdateEventArgs : EventArgs
	{
		public readonly int X;
		public readonly int Y;
		public readonly double Theta;

		#region Public Constructors

		public OdometryUpdateEventArgs (int x, int y, double theta)
		{
			this.X = x;
			this.Y = y;
			this.Theta = theta;
		}

		#endregion

		#region Public Methods

		public override string ToString ()
		{
			return "x = " + this.X + " y = " + this.Y + " theta = " + this.Theta;
		}

		#endregion

	}
}

