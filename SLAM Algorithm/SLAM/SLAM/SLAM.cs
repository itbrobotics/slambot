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
			Gdk.Threads.Init ();
			Gdk.Threads.Enter ();
			Application.Init ();
			SetupDialog setupDialog = new SetupDialog ();
			setupDialog.ShowAll ();
			Application.Run ();
			Gdk.Threads.Leave ();
		}

		#endregion

		#region Public Constructors

		public SLAM (double mapWidth, double mapHeight, double cellSize, string port)
		{
			robot = new Robot ();
			map = new SlamMap (robot, mapWidth, mapHeight, cellSize);
			mapView = new MapView (map);

			ekfSlam = new EkfSlam (1); // 1 degree per scan.

			proxy = SerialProxy.GetInstance;
			proxy.Port = port;

			proxy.OdometryUpdated += new EventHandler<OdometryUpdateEventArgs> (SerialProxy_OdometryUpdate);
			proxy.Scanned += new EventHandler<ScanEventArgs> (SerialProxy_Scan);

			window = new MapWindow (mapView);

			window.DeleteEvent += delegate
			{
				proxy.Release ();
			};

			//AddSampleLandmarks ();
		}

		#endregion

		#region Public Methods

		public void Start ()
		{
			proxy.Start ();
			window.ShowAll ();
		}

		#endregion

		#region Private Event Handlers

		private void SerialProxy_OdometryUpdate (object sender, OdometryUpdateEventArgs e)
		{
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
			Gdk.Threads.Enter ();
			try
			{
				// Extract any landmarks then update the slam database with any new landmarks.
				ekfSlam.UpdateAndAddLineLandmarks (ekfSlam.ExtractLineLandmarks (e.Readings.ToArray (),
					robot.Position));

				ekfSlam.RemoveBadLandmarks (e.Readings.ToArray (), robot.Position);

				// Now update the model.
				map.UpdateLandmarks (ekfSlam.GetDB ());
			}
			finally
			{
				Gdk.Threads.Leave ();
			}
		}

		#endregion

		#region Private Method

		private void AddSampleLandmarks  ()
		{
			double[] readings = new double[170] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7,1.8,1.9, 2.0,2.1,2.2,2.3,2.4,2.5,2.6,2.7,2.8,2.9,3.0,0.5878006935,0.5829896926,0.5835051536,0.5762886524,0.5823023796,0.5797250747,0.5876288414,0.5865979194,0.5878006935,0.5876288414,0.5804123878,0.5840206146,0.5798968791,0.5883161544,0.5843642234,0.5871133804,0.5886597633,0.5876288414,0.5886597633,0.5886597633,0.5876288414,0.5891752719,0.5924398422,0.5819587707,0.5893470764,0.5886597633,0.0085910644,0.0085910644,0.5902061939,0.5927834987,0.5938144207,0.5972508907,0.5943298816,0.5984535694,0.5974226474,0.6010309219,0.6022336483,4.5996563434,4.6108247756,4.6003436565,4.0082474231,4.6154639244,4.0082474231,4.6670102596,4.0082474231,4.6671821594,4.0077319583,4.0085910644,4.0085910644,4.0085910644,4.0085910644,4.0085910644,4.6804123401,4.6768041133,4.6721649169,4.6721649169,4.6764605045,4.9336769104,4.6852233886,4.9298969268,4.9326459884,4.7438144207,4.7422680854,4.0082474231,4.0079037799,4.0085910644,4.7453608036,4.7463917732,4.0082474231,4.7391752719,4.7407216548,4.7302405834,4.0082474231,4.0085910644,4.7336769580,4.7326460361,4.0082474231,4.0070446734,4.0085910644,4.0085910644,4.0085910644,4.0085910644,4.6460481166,4.6427835464,4.6419243812,4.6460481166,4.6470789909,4.0082474231,4.6412370681,4.0085910644,4.0085910644,4.0085910644,4.6369415283,4.0082474231,4.6371133804,4.6309278011,4.6371133804,4.6376288414,4.6383161544,4.6369415283,4.6328178405,4.6340206146,4.6331614971,4.6328178405,4.6340206146,4.6331614971,4.6292095661,4.6340206146,4.6256013393,4.6328178405,4.6266323089,4.6331614971,4.6304123401,4.6328178405,4.6328178405,4.6383161544,4.6371133804,4.6371133804,4.6424398899,4.6376288414,4.6378006935,4.6352233409,4.6378006935,4.6304123401,4.0089347066,4.0085910644,4.0082474231,4.0085910644,4.0085910644,4.6340206146,4.0085910644,4.0082474231,4.6292095661,4.0085910644,4.6551546573,4.0075601377, 4.7506872177, 4.6778349876, 4.0085910644 };
			double[] position = new double[3] { 0, 0, 0 };

			ekfSlam.RemoveBadLandmarks (readings, position);

			// Extract any landmarks then update the slam database with any new landmarks.
			ekfSlam.UpdateAndAddLineLandmarks (ekfSlam.ExtractLineLandmarks (readings,
				position));

			// Now update the model.
			map.UpdateLandmarks (ekfSlam.GetDB ());
		}

		#endregion
	}
}

