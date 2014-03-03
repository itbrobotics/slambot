using System;
using System.Threading;
using Driver;

namespace SLAM
{
	public class SlamController
	{
		private Robot robot;
		private Map map;
		private MapView mapView;
		private MapWindow window;

		private EkfSlam ekfSlam;
		private SerialProxy proxy;

		private Thread simulationThread;

		#region Public Constructors

		public SlamController ()
		{
			robot = new Robot ();
			map = new Map (robot, 2.97, 2.25);
			mapView = new MapView (map);
			window = new MapWindow (mapView);

			window.DeleteEvent += delegate
			{
				//StopSimulation (); // Kill the thread when the window closes.
				proxy.Release ();
			};

			window.ShowAll ();

			ekfSlam = new EkfSlam (1); // 1 degree per scan.

			proxy = new SerialProxy ();
			proxy.OdometryUpdated += new EventHandler<OdometryUpdateEventArgs> (SerialProxy_OdometryUpdate);
			proxy.Scanned += new EventHandler<ScanEventArgs> (SerialProxy_Scan);
		
			//AddSampleLandmarks ();
		}

		#endregion

		#region Public Methods

		public void StartSimulation ()
		{
			simulationThread = new Thread (PushRobot);
			simulationThread.Start ();
		}

		public void StopSimulation ()
		{
			simulationThread.Abort ();
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
			// Extract any landmarks then update the slam database with any new landmarks.
			ekfSlam.UpdateAndAddLineLandmarks (ekfSlam.ExtractLineLandmarks (e.Readings.ToArray (),
				robot.Position));

			// Now update the model.
			map.UpdateLandmarks (ekfSlam.GetDB ());
		}

		#endregion

		#region Private Methods

		private void AddSampleLandmarks ()
		{
			double[] laserDataAt0 = new double[170] { 	0.9283505439,0.9422680854,0.9601374626,0.8300687789,
														0.8230239868,0.8261167526,0.8044672966,0.8085910797,
														0.8044672966,0.8085910797,0.8080755233,0.8049828529,
														0.8092782974,0.8132302284,0.8164948463,0.8123711585,
														0.8245704650,0.8489690780,0.8250858306,0.8201030731,
														0.8484536170,0.8360824584,0.8323023796,0.8189002990,
														0.8395189285,0.8189002990,0.8276632308,0.8276632308,
														0.8225086212,0.8142612457,0.8185566902,0.8218213081,
														0.8108247756,0.8106529235,0.8180412292,0.8178693771,
														0.8216494560,0.8216494560,0.8225086212,0.8266323089,
														0.8257731437,0.8304123878,0.8300687789,0.8309277534,
														0.8259449958,0.8297250747,0.8297250747,0.8309277534,
														0.8300687789,0.8257731437,0.8300687789,0.8257731437,
														0.8309277534,0.8345360755,0.8345360755,0.8384879112,
														0.8268040657,0.8257731437,0.8345360755,0.8432990074,
														0.8517181396,0.8345360755,0.8517181396,0.8527490615,
														0.8474226951,0.8434707641,0.9520618438,0.9560137748,
														0.9463917732,0.9503436088,0.9503436088,0.9457043647,
														0.9500000000,0.9420961380,0.9463917732,0.9460481643,
														0.9372852325,0.9415806770,0.9405498504,0.9316150665,
														0.9328178405,0.9328178405,0.9369416236,0.9369416236,
														0.9312714576,0.9312714576,0.9323023796,0.9323023796,
														0.9323023796,0.9312714576,0.9350515365,0.9359106063,
														0.9357387542,0.9316150665,0.9359106063,0.9307559967,
														0.9348796844,0.9352233886,0.9352233886,0.9355669975,
														0.9352233886,0.9343642234,0.9300687789,0.9312714576,
														0.9355669975,0.9352233886,0.9350515365,0.9336769104,
														0.9340206146,0.9348796844,0.9348796844,0.9348796844,
														0.9340206146,0.9336769104,0.9348796844,0.9348796844,
														0.9350515365,0.9350515365,0.9340206146,0.9340206146,
														0.9391752243,0.9436425209,0.9477663993,0.9479381561,
														0.9479381561,0.9479381561,0.9424398422,0.9467353820,
														0.9470789909,0.9719930648,0.9680412292,0.9515464782,
														0.9515464782,0.9596220016,1.0496563911,1.0831614685,
														1.0831614685,1.2269759178,0.9560137748,0.9726804733,
														1.2269759178,1.2310996055,1.2309278249,1.2266323566,
														1.2266323566,1.2266323566,1.2264604568,1.2264604568,
														1.2295532226,1.2307560443,1.2302405834,1.2262886762,
														1.2259449958,1.2262886762,1.2262886762,1.2259449958,
														1.2262886762,1.2259449958,1.2262886762,1.2259449958,
														1.2302405834,1.2302405834,1.2302405834,1.2300686836,
														1.2300686836,1.2300686836,1.2302405834,1.2302405834,
														1.2302405834,1.2341923713 };

			// Start at x = 0, y = 0, rotation = 0.
			double[] robotPosition = new double[3] { 0, 0, 0 };

			Landmark[] landmarkResults = ekfSlam.ExtractLineLandmarks (laserDataAt0, robotPosition);

			ekfSlam.UpdateAndAddLineLandmarks (landmarkResults);

			map.AddLandmarks (ekfSlam.GetDB ());
		}

		private void PushRobot ()
		{
			while (true)
			{
				robot.Y += 0.1;
				Thread.Sleep (1000);
			}
		}

		#endregion
	}
}

