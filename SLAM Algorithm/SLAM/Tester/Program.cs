using System;
using Driver;
using SLAM;

namespace Tester
{
	class MainClass
	{
		private SerialProxy proxy;
		private Robot robot;

		public MainClass (string port, int baudRate)
		{
			proxy = SerialProxy.GetInstance;
			proxy.Port = port;
			proxy.BaudRate = baudRate;

			robot = new Robot ();

			proxy.OdometryUpdated += new EventHandler<OdometryUpdateEventArgs> (SerialProxy_OdometryUpdate);
			proxy.Scanned += new EventHandler<ScanEventArgs> (SerialProxy_Scan);
			proxy.Start ();
		}

		public static void Main (string[] args)
		{
			Console.Write ("Enter Port: ");
			string port = Console.ReadLine ();

			Console.Write ("Enter Baud Rate: ");
			string baudRate = Console.ReadLine ();

			MainClass main = new MainClass (port, Int32.Parse (baudRate));
			main.Forward (1.0);
		}

		#region Public Methods

		public void Idle ()
		{
			while (proxy.IsOpen)
			{
				Console.WriteLine ("Robot y: " + robot.Position [1]);
			}
		}

		public void Forward (double distance)
		{
			if (robot.State != RobotState.Halted || robot.State != RobotState.Scanning)
				robot.Halt ();

			double newPostion = robot.Position [1] + distance;

			robot.GoFoward ();

			while (robot.Position [1] < newPostion)
			{
				Console.WriteLine (robot.Position [1]);
			}

			robot.Halt ();
			Console.WriteLine (robot.Position [1]);

			proxy.Release ();
		}

		#endregion

		#region Private Event Handlers

		private void SerialProxy_OdometryUpdate (object sender, OdometryUpdateEventArgs e)
		{
			robot.UpdateOdometry (e);
		}

		/// <summary>
		/// Handles a scan, simply updates the landmark database and map if required
		/// by extracting them using the EKF algorithm.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SerialProxy_Scan (object sender, ScanEventArgs e)
		{

		}

		#endregion
	}
}
