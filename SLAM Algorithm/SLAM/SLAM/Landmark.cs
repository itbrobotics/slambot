using System;

namespace SLAM
{
	public class Landmark
	{
		const int LIFE = 40;

		public double[] pos; 			// Landmarks (x, y) position relative to the map.
		public int id; 					// The landmarks unique ID.
		public int life; 				// A life counter used to determine whether to discard a landmark.
		public int totalTimesObserved; 	// The number of times we have seen a landmark.
		public double range; 			// Last observed range to landmark.
		public double bearing; 			// Last observed bearing to landmark.

		// RANSAC: Now store equation of a line.
		public double a;
		public double b;
		public double rangeError; 	// Distance from robot position to the wall we are using as 
									// a landmark (to calculate error).

		public double bearingError; // Bearing from robot position to the wall we are using as 
									// a landmark (to calculate error).

		public Landmark()
		{
			totalTimesObserved = 0;
			id = -1;
			life = LIFE;
			pos = new double[2];
			a = -1;
			b = -1;
		}

		// Keep track of bad landmarks?
	}
}

