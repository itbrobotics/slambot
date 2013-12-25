using System;

namespace SLAM
{
	public class Landmark
	{
		const int LIFE = 40;

		public double[] pos; //landmarks (x,y) position relative to map
		public int id; //the landmarks unique ID
		public int life; //a life counter used to determine whether to discard a landmark
		public int totalTimesObserved; //the number of times we have seen landmark
		public double range; //last observed range to landmark
		public double bearing; //last observed bearing to landmark

		//RANSAC: Now store equation of a line
		public double a;
		public double b;
		public double rangeError; //distance from robot position to the wall we are using as a landmark (to calculate error)
		public double bearingError; //bearing from robot position to the wall we are using as a landmark (to calculate error)

		public Landmark()
		{
			totalTimesObserved = 0;
			id = -1;
			life = LIFE;
			pos = new double[2];
			a = -1;
			b = -1;
		}

		//keep track of bad landmarks?
	}
}

