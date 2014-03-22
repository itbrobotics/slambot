using System;
using System.Threading;
using Driver;

namespace SLAM
{
	public class SlamController
	{
		private Robot robot;
		private SlamMap map;
		private MapView mapView;
		private MapWindow window;

		private EkfSlam ekfSlam;
		private SerialProxy proxy;

		private Thread simulationThread;

		#region Public Constructors

		public SlamController ()
		{
			robot = new Robot ();
			map = new SlamMap (robot, 5.5, 3.5);
			mapView = new MapView (map);

			ekfSlam = new EkfSlam (1); // 1 degree per scan.

			//AddSampleLandmarks ();

			proxy = SerialProxy.GetInstance;
			proxy.OdometryUpdated += new EventHandler<OdometryUpdateEventArgs> (SerialProxy_OdometryUpdate);
			proxy.Scanned += new EventHandler<ScanEventArgs> (SerialProxy_Scan);

			window = new MapWindow (mapView);

			window.DeleteEvent += delegate
			{
				proxy.Release ();
			};

			//StartSimulation ();
			window.ShowAll ();
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
			Console.WriteLine (e.ToString ());
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
			double[] laserDataAt0 = new double[170] { 0.8647766113,0.8651202201,0.8701030731,0.8764604568,0.9434707641,0.9451889991,0.3852233695,0.3807559967,0.3759449958,0.3762886524,0.3771477699,0.3771477699,0.3771477699,0.3816151142,0.3819587707,0.3819587707,0.3816151142,0.3864261150,0.3864261150,0.3864261150,0.3951889991,0.3910653114,0.4039518833,0.9539519309,0.9178693771,0.9623710632,0.9503436088,0.9585910797,0.9364260673,0.9445016860,0.9487972259,0.9539519309,0.9407217025,0.9412370681,0.9441580772,0.9481100082,0.9448453903,0.9407217025,0.9407217025,0.9445016860,0.9431271553,0.8900342941,0.9073883056,0.8903779029,0.8945017814,0.8817869186,0.8816151618,0.8852233886,0.8852233886,0.8807559967,0.8764604568,0.8805841445,0.8807559967,0.8800686836,0.8759449958,0.8756013870,0.8713058471,0.8756013870,0.8749140739,0.8745704650,0.8707902908,0.8701030731,0.8702749252,0.8701030731,0.8701030731,0.8702749252,0.8656357765,0.8697594642,0.8697594642,0.8658076286,0.8692440032,0.8692440032,0.8651202201,0.8692440032,0.8690721511,0.8687284469,0.8685566902,0.8568728446,0.8685566902,0.8644330024,0.8644330024,0.8683848381,0.8683848381,0.8606529235,0.8680412292,0.8680412292,0.8680412292,0.8640893936,0.8635738372,0.8649484634,0.8640893936,0.8640893936,0.8640893936,0.8678694725,0.8678694725,0.8640893936,0.8640893936,0.8640893936,0.8676976203,0.8518899917,0.8640893936,0.8676976203,0.8680412292,0.8676976203,0.8678694725,0.8678694725,0.8716495513,0.8716495513,0.8716495513,0.8719931602,0.8676976203,0.8716495513,0.8723367691,0.8723367691,0.8764604568,0.8719931602,0.8721649169,0.8809278488,0.8762886047,0.8764604568,0.8807559967,0.8798969268,0.8809278488,0.8810996055,0.8809278488,0.8853952407,0.8900342941,0.9182130813,0.9182130813,0.9192440032,0.8773195266,0.9099656105,0.9486253738,0.9580756187,0.9448453903,0.9539519309,1.0192439556,0.9869415283,0.9034364700,0.9943299293,1.5152920722,1.9649484634,0.0000000000,0.0000000000,3.0625429153,1.9228521347,1.9092782974,1.9137457847,4.4845361709,1.9209622383,3.1915807723,1.9298968315,1.9005154609,0.0000000000,1.8608246803,1.5058419704,1.0405498743,1.5214776992,1.5128865242,1.4420962333,3.1843643188,1.9158075332,1.5013744831,2.3298969268,0.4659793853,0.4661511898,0.4620274543,0.4701030731,0.4824742317,0.4529209613};

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
				if (robot.Y >= map.Height / 2)
				{
					robot.Y -= 0.01;
				}
				else
				{
					robot.Y += 0.01;
				}

				Thread.Sleep (500);
			}
		}

		#endregion
	}
}

