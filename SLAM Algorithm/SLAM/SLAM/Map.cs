using System;
using System.Collections.Generic;

namespace SLAM
{
	public class Map
	{
		private List<Landmark> mapLandmarks = new List<Landmark>();
		private double[] robotPosition = new double[3] { 0, 0, 0 };

		/************************************************************
		 * Public Properties
		 ***********************************************************/

		public List<Landmark> SlamLandmarks
		{
			get
			{
				return this.mapLandmarks;
			}
			set
			{
				this.mapLandmarks = value;
			}
		}

		public double[] RobotPosition
		{
			get
			{
				return this.robotPosition;
			}
			set
			{
				this.robotPosition = value;
			}
		}

		/************************************************************
		 * Public Constructors
		 ***********************************************************/

		public Map ()
		{
			double[] laserDataAt0 = new double[180] {1.08, 1.08, 1.08, 1.08, 1.07, 1.08, 1.14, 1.10, 1.15, 1.23, 1.23, 1.34, 1.06, 1.04, 1.03, 1.03, 1.04, 1.03, 1.04, 1.03, 1.03, 1.04, 1.04, 1.03, 1.03, 1.02, 1.03, 1.03, 1.04, 1.04, 1.04, 1.04, 1.04, 1.04, 1.04, 1.03, 1.03, 1.03, 1.03, 1.04, 1.05, 1.05, 1.04, 1.05, 1.04, 1.05, 1.05, 1.04, 1.05, 1.04, 1.06, 1.06, 1.06, 1.50, 1.06, 1.50, 1.36, 1.37, 1.36, 1.35, 1.35, 1.35, 1.23, 1.25, 1.25, 1.25, 1.23, 1.23, 1.22, 1.22, 1.23, 1.23, 1.23, 1.23, 1.23, 1.22, 1.22, 1.23, 1.23, 1.23, 1.22, 1.23, 1.21, 1.22, 1.22, 1.22, 1.23, 1.22, 1.22, 1.14, 1.21, 1.21, 1.20, 1.21, 1.20, 1.20, 1.20, 1.20, 1.20, 1.21, 1.09, 1.09, 1.09, 1.09, 1.09, 1.09, 1.08, 1.23, 1.09, 1.22, 1.08, 1.09, 1.08, 1.08, 1.08, 1.07, 1.07, 1.08, 1.08, 1.08, 1.08, 1.13, 1.09, 1.09, 1.08, 1.08, 1.08, 1.08, 1.08, 1.08, 1.13, 1.14, 1.14, 1.14, 1.13, 1.14, 1.13, 1.14, 1.15, 1.15, 1.15, 1.14, 1.14, 1.15, 1.14, 1.14, 1.14, 1.15, 1.14, 1.14, 1.14, 1.14, 1.14, 1.14, 1.14, 1.14, 1.15, 1.14, 1.14, 1.14, 1.14, 1.13, 1.14, 1.14, 1.15, 1.14, 1.14, 1.15, 1.15, 1.15, 1.14, 1.15, 1.17, 1.16, 1.16, 1.18, 1.17, 1.16, 1.17, 1.17};

			Landmarks landmarks = new Landmarks (1);

			// Start at x = 0, y = 0, rotation = 0.
			double[] robotPosition = new double[3] { 0, 0, 0 };

			Landmark[] landmarkResults = landmarks.ExtractLineLandmarks (laserDataAt0, robotPosition);

			landmarks.UpdateAndAddLineLandmarks (landmarkResults);

			this.mapLandmarks.AddRange (landmarks.GetDB());

//			Landmark landmark1 = new Landmark();
//			landmark1.id = 1;
//			landmark1.pos[0] = 1;
//			landmark1.pos[1] = 2;
//			landmark1.a = 3;
//			landmark1.b = 3;

//			Landmark landmark2 = new Landmark();
//			landmark2.id = 2;
//			landmark2.pos[0] = -1;
//			landmark2.pos[1] = -2;
//
//			Landmark landmark3 = new Landmark();
//			landmark3.id = 3;
//			landmark3.pos[0] = -2;
//			landmark3.pos[1] = -1;

//			this.mapLandmarks.Add (landmark1);
//			this.mapLandmarks.Add (landmark2);
//			this.mapLandmarks.Add (landmark3);
		}
	}
}

