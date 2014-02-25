using System;

namespace Driver
{
	public class Observer
	{
		private SerialProxy serialProxy;

		#region Public Constructor

		public Observer (SerialProxy serialProxy)
		{
			this.serialProxy = serialProxy;
			this.serialProxy.OdometryUpdated += new EventHandler<OdometryUpdateEventArgs> (SerialProxy_OdometryUpdate);
		}

		#endregion

		#region Private Event Handlers

		private void SerialProxy_OdometryUpdate (object sender, OdometryUpdateEventArgs e)
		{
			Console.WriteLine (e.ToString ());
		}

		private void SerialProxy_Scan (object sender, ScanEventArgs e)
		{
			Console.WriteLine (e.ToString ());
		}

		#endregion

	}
}

