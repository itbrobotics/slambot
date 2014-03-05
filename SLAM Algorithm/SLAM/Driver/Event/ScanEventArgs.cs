using System;
using System.Text;
using System.Collections.Generic;

namespace Driver
{
	public class ScanEventArgs : EventArgs
	{
		public readonly List<double> Readings;

		#region Public Constructors

		public ScanEventArgs (List<double> readings)
		{
			this.Readings = readings;
		}

		#endregion

		#region Public Methods

		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder();

			foreach (double reading in Readings)
			{
				builder.Append(reading).Append(" ");
			}

			return builder.ToString ();
		}

		#endregion

	}
}

