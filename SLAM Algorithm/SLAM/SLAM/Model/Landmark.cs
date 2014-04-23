using System;

namespace SLAM
{
	/// <summary>
	/// Landmark.
	/// </summary>
	public class Landmark
	{
		const int LIFE = 40;

		public double[] pos; 			// Landmarks (x, y) position relative to the map.
		public double[] x1y1;			// Landmarks (x1, y1) position relative to the map.
		public double[] x2y2;			// Landmarks (x2, y2) position relative to the map.
		public int id; 					// The landmarks unique ID.
		public int life; 				// A life counter used to determine whether to discard a landmark.
		public int totalTimesObserved; 	// The number of times we have seen a landmark.
		public double range; 			// Last observed range to landmark.
		public double bearing; 			// Last observed bearing to landmark.

		// RANSAC: Now store equation of a line.
		// slope (m) = -a / b
		public double a;
		public double b;
		public double rangeError; 	// Distance from robot position to the wall we are using as 
									// a landmark (to calculate error).

		public double bearingError; // Bearing from robot position to the wall we are using as 
									// a landmark (to calculate error).

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.Landmark"/> class.
		/// </summary>
		public Landmark()
		{
			totalTimesObserved = 0;
			id = -1;
			life = LIFE;
			pos = new double[2];
			x1y1 = new double[2];
			x2y2 = new double[2];
			a = -1;
			b = -1;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="SLAM.Landmark"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="SLAM.Landmark"/>.</returns>
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
							+ "\tb = " + this.b + "\n"
			              	+ "\tx1 = " + this.x1y1[0] + "\n"
			              	+ "\ty1 = " + this.x1y1[1] + "\n"
			              	+ "\tx2 = " + this.x2y2[0] + "\n"
			              	+ "\ty2 = " + this.x2y2[1] + "\n\n";

			return string.Format (text, this);
		}

		#endregion
	}
}

