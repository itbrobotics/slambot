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

		/************************************************************
		 * Public Constructors
		 ***********************************************************/

		public Landmark()
		{
			totalTimesObserved = 0;
			id = -1;
			life = LIFE;
			pos = new double[2];
			a = -1;
			b = -1;
		}

		/************************************************************
		 * Public Methods
		 ***********************************************************/

		public override string ToString ()
		{
			string text =	"Landmark : " + this.id + "\n"
							+ "\tid = " + this.id + "\n" 
				           	+ "\tlife = " + this.life + "\n"
		                   	+ "\ttimesObserved = " + this.totalTimesObserved + "\n"
		                   	+ "\tx = " + this.pos[0] + "\n"
		                   	+ "\ty = " +  this.pos[1] + "\n"
		                   	+ "\trange = " + this.range + "\n"
		                   	+ "\tbearing = " + this.bearing + "\n"
		                   	+ "\ta = " + this.a + "\n"
							+ "\tb = " + this.b + "\n\n";

			return string.Format (text, this);
		}
	}
}

