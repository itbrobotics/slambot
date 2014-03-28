using Gtk;
using System;
using System.Threading;
using Driver;

namespace SLAM
{
	public class SLAM
	{
		private Robot robot;
		private SlamMap map;
		private MapView mapView;
		private MapWindow window;

		private EkfSlam ekfSlam;
		private SerialProxy proxy;

		#region Main

		public static void Main (String[] args)
		{
			//TODO: The use of GDK Locking is discouraged when using Mono as it could potentially
			// leave GTK+ in an inconsistent state. The proper solution here would be to use 
			// ThreadNotify or Application.Invoke but for the moment this quick fix will suffice.
			Gdk.Threads.Init ();
			Gdk.Threads.Enter ();
			Application.Init ();
			SLAM controller = new SLAM ();
			controller.Start ();
			Application.Run ();
			Gdk.Threads.Leave ();
		}

		#endregion

		#region Public Constructors

		public SLAM ()
		{
			robot = new Robot ();
			map = new SlamMap (robot, 6.3, 3.3);
			mapView = new MapView (map);

			ekfSlam = new EkfSlam (1); // 1 degree per scan.

			proxy = SerialProxy.GetInstance;
			proxy.OdometryUpdated += new EventHandler<OdometryUpdateEventArgs> (SerialProxy_OdometryUpdate);
			proxy.Scanned += new EventHandler<ScanEventArgs> (SerialProxy_Scan);

			window = new MapWindow (mapView);

			window.DeleteEvent += delegate
			{
				proxy.Release ();
			};
		}

		#endregion

		#region Public Methods

		public void Start ()
		{
			window.ShowAll ();
		}

		#endregion

		#region Private Event Handlers

		private void SerialProxy_OdometryUpdate (object sender, OdometryUpdateEventArgs e)
		{
			//TODO: The use of GDK Locking is discouraged when using Mono as it could potentially
			// leave GTK+ in an inconsistent state. The proper solution here would be to use 
			// ThreadNotify or Application.Invoke but for the moment this quick fix will suffice.
			Gdk.Threads.Enter ();
			try
			{
				robot.UpdateOdometry (e);
			}
			finally
			{
				Gdk.Threads.Leave ();
			}
		}

		/// <summary>
		/// Handles a scan, simply updates the landmark database and map if required
		/// by extracting them using the EKF algorithm.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SerialProxy_Scan (object sender, ScanEventArgs e)
		{
			//TODO: The use of GDK Locking is discouraged when using Mono as it could potentially
			// leave GTK+ in an inconsistent state. The proper solution here would be to use 
			// ThreadNotify or Application.Invoke but for the moment this quick fix will suffice.
			Gdk.Threads.Enter ();
			try
			{
			// Extract any landmarks then update the slam database with any new landmarks.
			ekfSlam.UpdateAndAddLineLandmarks (ekfSlam.ExtractLineLandmarks (e.Readings.ToArray (),
				robot.Position));

			// Now update the model.
			map.UpdateLandmarks (ekfSlam.GetDB ());
			}
			finally
			{
				Gdk.Threads.Leave ();
			}
		}

		#endregion
	}
}

