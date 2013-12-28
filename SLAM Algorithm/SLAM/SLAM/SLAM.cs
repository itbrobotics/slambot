using System;

namespace SLAM
{
	public class SLAM
	{
		private const int DegreesPerScan = 1;

		public SLAM ()
		{
			
		}

		public static void Main (String[] args)
		{
			/*
			 * Step 1: 	Create a Landmarks object set to 1 
			 * 			degree per scan.
			 * 
			 * Step 2: 	Collect laser and robot position data 
			 * 		   	at the same time.
			 * 
			 * Step 3: 	Extract the line landmarks from the
			 * 		   	data.
			 * 
			 * Step 4:	Update the landmarks database with
			 * 			the new landmarks.
			 * 
			 * Step 5:	Remove any bad landmarks or doubles
			 * 			from the database.
			 * 
			 * Step 6:	Display the results in some form.
			 * 
			 * Step 7:	Repeat steps 2-6 for a finite time.
			 */
			Landmarks landmarks = new Landmarks (DegreesPerScan);

			double[] laserDataAt0 = new double[180] { 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 0.40, 
													  0.40, 0.40, 0.41, 0.48, 0.48, 0.48, 0.48, 0.47, 
													  0.48, 0.42, 0.40, 0.40, 0.39, 0.39, 0.38, 0.38, 
													  0.38, 0.38, 0.38, 0.38, 0.38, 0.38, 0.38, 0.38, 
													  0.38, 0.38, 0.38, 0.38, 0.38, 0.38, 0.38, 0.38, 
													  0.38, 0.38, 0.38, 0.38, 0.38, 0.39, 0.39, 0.00, 
													  0.39, 0.39, 0.39, 0.39, 0.39, 0.38, 0.39, 0.39, 
													  0.39, 0.39, 0.39, 0.40, 0.39, 0.39, 0.39, 0.39, 
													  0.39, 0.40, 0.40, 0.40, 0.40, 0.41, 0.40, 0.40, 
													  0.40, 0.40, 0.41, 0.41, 0.41, 0.42, 0.49, 0.49, 
													  0.49, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 
													  0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 
													  0.48, 0.47, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 
													  0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 
													  0.48, 0.48, 0.48, 0.48, 0.48, 0.48, 0.49, 0.48, 
													  0.42, 0.42, 0.42, 0.41, 0.42, 0.42, 0.41, 0.41, 
													  0.41, 0.41, 0.40, 0.41, 0.40, 0.40, 0.40, 0.40, 
													  0.40, 0.40, 0.40, 0.40, 0.40, 0.40, 0.39, 0.39, 
													  0.40, 0.40, 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 
													  0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 
													  0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 0.39, 
													  0.39, 0.39, 0.00, 0.40, 0.39, 0.40, 0.39, 0.00, 
													  0.40, 0.40, 0.00, 0.40 };

			double[] laserDataAt10 = new double[180] { 0.35, 0.35, 0.35, 0.35, 0.35, 0.35, 0.36, 0.36, 
													   0.36, 0.36, 0.35, 0.36, 0.35, 0.36, 0.36, 0.35, 
													   0.35, 0.35, 0.35, 0.35, 0.35, 0.34, 0.35, 0.35, 
													   0.35, 0.35, 0.35, 0.00, 0.35, 0.35, 0.34, 0.35, 
													   0.35, 0.35, 0.35, 0.35, 0.35, 0.00, 0.35, 0.34, 
													   0.35, 0.00, 0.35, 0.35, 0.35, 0.35, 0.35, 0.34, 
													   0.35, 0.35, 0.35, 0.35, 0.35, 0.35, 0.35, 0.35, 
													   0.35, 0.35, 0.35, 0.35, 0.36, 0.35, 0.36, 0.36, 
													   0.35, 0.36, 0.36, 0.37, 0.36, 0.37, 0.37, 0.38, 
													   0.37, 0.38, 0.37, 0.41, 0.41, 0.41, 0.41, 0.41, 
													   0.42, 0.42, 0.42, 0.41, 0.41, 0.41, 0.41, 0.41, 
													   0.00, 0.41, 0.41, 0.40, 0.41, 0.40, 0.40, 0.41, 
													   0.41, 0.41, 0.41, 0.41, 0.41, 0.40, 0.41, 0.41, 
													   0.41, 0.41, 0.41, 0.41, 0.41, 0.41, 0.41, 0.41, 
													   0.41, 0.40, 0.40, 0.41, 0.41, 0.41, 0.41, 0.37, 
													   0.41, 0.36, 0.36, 0.36, 0.35, 0.35, 0.00, 0.35, 
													   0.35, 0.35, 0.35, 0.35, 0.34, 0.35, 0.34, 0.00, 
													   0.35, 0.34, 0.34, 0.34, 0.34, 0.34, 0.00, 0.34, 
													   0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 
													   0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 
													   0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 0.34, 
													   0.34, 0.34, 0.00, 0.34, 0.34, 0.34, 0.34, 0.34, 
													   0.34, 0.34, 0.34, 0.34 };

			double[] laserDataAt20 = new double[180] { 0.29, 0.29, 0.29, 0.28, 0.00, 0.00, 0.00, 1.43, 
													   0.31, 0.31, 0.30, 0.30, 0.30, 0.30, 0.30, 0.31, 
													   0.30, 0.30, 0.30, 0.30, 0.30, 0.29, 0.29, 0.29, 
													   0.29, 0.29, 0.29, 0.00, 0.29, 0.29, 0.29, 0.29, 
													   0.29, 0.00, 0.29, 0.29, 0.00, 0.29, 0.29, 0.29, 
													   0.29, 0.29, 0.29, 0.29, 0.00, 0.29, 0.29, 0.29, 
													   0.00, 0.30, 0.30, 0.30, 0.30, 0.29, 0.29, 0.29, 
													   0.29, 0.00, 0.30, 0.30, 0.30, 0.29, 0.30, 0.30, 
													   0.30, 0.30, 0.00, 0.31, 0.30, 0.30, 0.31, 0.31, 
													   0.30, 0.30, 0.30, 0.31, 0.30, 0.00, 0.00, 0.00, 
													   0.00, 0.00, 0.00, 0.00, 0.33, 0.32, 0.31, 0.31, 
													   0.31, 0.31, 0.31, 0.31, 0.31, 0.31, 0.30, 0.31, 
												       0.31, 0.31, 0.00, 0.32, 0.31, 0.31, 0.31, 0.31, 
													   0.31, 0.31, 0.00, 0.31, 0.31, 0.31, 0.30, 0.31, 
													   0.31, 0.31, 0.31, 0.31, 0.31, 0.30, 0.31, 0.31, 
													   0.31, 0.31, 0.32, 0.32, 0.32, 0.31, 0.00, 0.32, 
													   0.29, 0.29, 0.29, 0.29, 0.29, 0.29, 0.28, 0.28, 
													   0.00, 0.29, 0.28, 0.28, 0.28, 0.28, 0.28, 0.28, 
													   0.28, 0.28, 0.28, 0.28, 0.28, 0.28, 0.28, 0.28, 
													   0.28, 0.28, 0.28, 0.28, 0.27, 0.28, 0.27, 0.27, 	
													   0.27, 0.28, 0.28, 0.28, 0.27, 0.00, 0.28, 0.27, 
													   0.27, 0.28, 0.27, 0.28, 0.28, 0.28, 0.28, 0.28, 
													   0.00, 0.28, 0.28, 0.00 };

			// Start at x = 0, y = 0, rotation = 0.
			double[] robotPosition = new double[3] { 0, 0, 0 };

			Landmark[] landmarkResults = landmarks.ExtractLineLandmarks (laserDataAt0, robotPosition);

			landmarks.UpdateAndAddLineLandmarks (landmarkResults);

			Console.WriteLine ("Number of Extracted Landmarks: " + landmarkResults.Length);
			Console.WriteLine ("Number of Landmarks in database: " + landmarks.GetDBSize ());

			// Move 0.10 meters forward.
			robotPosition [1] = 0.10;

			landmarkResults = landmarks.ExtractLineLandmarks (laserDataAt10, robotPosition);

			landmarks.UpdateAndAddLineLandmarks (landmarkResults);

			Console.WriteLine ("Number of Extracted Landmarks: " + landmarkResults.Length);
			Console.WriteLine ("Number of Landmarks in database: " + landmarks.GetDBSize ());

			// Move 0.20 meters forward
			robotPosition [1] = 0.20;

			landmarkResults = landmarks.ExtractLineLandmarks (laserDataAt20, robotPosition);

			landmarks.UpdateAndAddLineLandmarks (landmarkResults);

			Console.WriteLine ("Number of Extracted Landmarks: " + landmarkResults.Length);
			Console.WriteLine ("Number of Landmarks in database: " + landmarks.GetDBSize ());

			Landmark[] databaseLandmarks = landmarks.GetDB ();

			for (int i = 0; i < databaseLandmarks.Length; i++)
			{
				Console.WriteLine ("\nLandmark " + i + ":\n"
				                   + "\tid = " + databaseLandmarks[i].id + "\n" 
				                   + "\tlife = " + databaseLandmarks[i].life + "\n"
				                   + "\ttimesObserved = " + databaseLandmarks[i].totalTimesObserved + "\n"
				                   + "\tx = " + databaseLandmarks[i].pos[0] + "\n"
				                   + "\ty = " +  databaseLandmarks[i].pos[1] + "\n"
				                   + "\trange = " + databaseLandmarks[i].range + "\n"
				                   + "\tbearing = " + databaseLandmarks[i].bearing + "\n"
				                   + "\ta = " + databaseLandmarks[i].a + "\n"
				                   + "\tb = " + databaseLandmarks[i].b);
			}
		}
	}
}

