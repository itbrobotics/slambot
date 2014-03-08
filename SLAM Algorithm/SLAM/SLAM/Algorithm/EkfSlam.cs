using System;

namespace SLAM
{
	/// <summary>
	/// Summary description for Landmarks.
	/// </summary>
	public class EkfSlam
	{
		double conv = Math.PI / 180.0; // Convert to radians
		const int MAXLANDMARKS = 3000;
		const double MAXERROR = 0.5; // If a landmark is within 20 cm of another landmark its the same landmark.
		public int MINOBSERVATIONS = 15; // Number of times a landmark must be observed to be recognized as a landmark.
		const int LIFE = 40;
		const double MAX_RANGE = 1;
		const int MAXTRIALS = 1000; // RANSAC: max times to run algorithm.
		const int MAXSAMPLE = 10; // RANSAC: randomly select X points.
		const int MINLINEPOINTS = 30; // RANSAC: if less than 40 points left don't bother trying to find consensus (stop algorithm).
		const double RANSAC_TOLERANCE = 0.05; // RANSAC: if point is within x distance of line its part of line.
		const int RANSAC_CONSENSUS = 30; // RANSAC: at least 30 votes required to determine if a line.
		double degreesPerScan = 0.5;
		Landmark[] landmarkDB = new Landmark[MAXLANDMARKS];
		int DBSize = 0;
		int[,] IDtoID = new int[MAXLANDMARKS, 2];
		int EKFLandmarks = 0;

		#region Public Getters

		public int GetSlamID (int id)
		{
			for (int i = 0; i < EKFLandmarks; i++)
			{
				if (IDtoID [i, 0] == id)
				{
					return IDtoID [i, 1];
				} 
			}

			return -1;
		}

		public int GetDBSize ()
		{
			return DBSize;
		}

		public Landmark[] GetDB ()
		{
			Landmark[] temp = new Landmark[DBSize];

			for (int i = 0; i < DBSize; i++)
			{
				temp [i] = landmarkDB [i];
			}

			return temp;
		}

		#endregion

		#region Public Constructors

		public EkfSlam (double degreesPerScan)
		{
			this.degreesPerScan = degreesPerScan;

			for (int i = 0; i < landmarkDB.Length; i++)
			{
				landmarkDB [i] = new Landmark ();
			}
		}

		#endregion

		#region Public Methods

		public int AddSlamID (int landmarkID, int slamID)
		{
			IDtoID [EKFLandmarks, 0] = landmarkID;
			IDtoID [EKFLandmarks, 1] = slamID;
			EKFLandmarks++;

			return 0;
		}

		public int RemoveBadLandmarks (double[] laserdata, double[] robotPosition)
		{
			double maxrange = 0;

			for (int i = 1; i < laserdata.Length - 1; i++)
			{
				// Distance further away than 8.1m we assume are failed returns
				// we get the laser data with max range.
				if (laserdata [i - 1] < 8.1)
				{
					if (laserdata [i + 1] < 8.1)
					{
						if (laserdata [i] > maxrange)
						{
							maxrange = laserdata [i];
						}
					}
				}
			}

			maxrange = MAX_RANGE;
			double[] Xbounds = new double[4];
			double[] Ybounds = new double[4];

			// Get bounds of rectangular box to remove bad landmarks from.
			Xbounds [0] = Math.Cos ((1 * degreesPerScan * conv) + (robotPosition [2] * Math.PI / 180)) * 
				maxrange + robotPosition [0];

			Ybounds [0] = Math.Sin ((1 * degreesPerScan * conv) + (robotPosition [2] * Math.PI / 180)) *
				maxrange + robotPosition [1];

			Xbounds [1] = Xbounds [0] + Math.Cos ((180 * degreesPerScan * conv) + (robotPosition [2] * 
				Math.PI / 180)) * maxrange;

			Ybounds [1] = Ybounds [0] + Math.Sin ((180 * degreesPerScan * conv) + (robotPosition [2] * 
				Math.PI / 180)) * maxrange;

			Xbounds [2] = Math.Cos ((359 * degreesPerScan * conv) + (robotPosition [2] * Math.PI / 180)) *
				maxrange + robotPosition [0];

			Ybounds [2] = Math.Sin ((359 * degreesPerScan * conv) + (robotPosition [2] * Math.PI / 180)) *
				maxrange + robotPosition [1];

			Xbounds [3] = Xbounds [2] + Math.Cos ((180 * degreesPerScan * conv) + (robotPosition [2] * 
				Math.PI / 180)) * maxrange;

			Ybounds [3] = Ybounds [2] + Math.Sin ((180 * degreesPerScan * conv) + (robotPosition [2] * 
				Math.PI / 180)) * maxrange;

			// Now check DB for landmarks that are within this box
			// decrease life of all landmarks in box. If the life reaches zero, remove landmark.
			double pntx, pnty;

			for (int k = 0; k < DBSize + 1; k++)
			{
				pntx = landmarkDB [k].pos [0];
				pnty = landmarkDB [k].pos [1];
				int i = 0;
				int j = 0;
				bool inRectangle;

				if (robotPosition [0] < 0 || robotPosition [1] < 0)
				{
					inRectangle = false;
				}
				else
				{
					inRectangle = true;
				}

				for (i = 0; i < 4; i++)
				{
					if (((((Ybounds [i] <= pnty) && (pnty < Ybounds [j])) || ((Ybounds [j] <= pnty) && (pnty
						< Ybounds [i]))) && (pntx < (Xbounds [j] - Xbounds [i]) * (pnty - Ybounds [i]) / 
						(Ybounds [j] - Ybounds [i]) + Xbounds [i])))
					{
						if (inRectangle == false)
						{
							inRectangle = true;
						}
						else
						{ 
							inRectangle = false;
						}
					}

					j = i;
					i++;
				}

				if (inRectangle)
				{
					// In rectangle so decrease life and maybe remove.
					landmarkDB [k].life--;

					if (landmarkDB [k].life <= 0)
					{
						for (int kk = k; kk < DBSize; kk++) // Remove landmark by copying down rest of DB.
						{
							if (kk == DBSize - 1)
							{
								landmarkDB [kk].pos [0] = landmarkDB [kk + 1].pos [0];
								landmarkDB [kk].pos [1] = landmarkDB [kk + 1].pos [1];
								landmarkDB [kk].life = landmarkDB [kk + 1].life;
								landmarkDB [kk].id = landmarkDB [kk + 1].id;
								landmarkDB [kk].totalTimesObserved = landmarkDB [kk + 1].totalTimesObserved;
							}
							else
							{
								landmarkDB [kk + 1].id--;
								landmarkDB [kk].pos [0] = landmarkDB [kk + 1].pos [0];
								landmarkDB [kk].pos [1] = landmarkDB [kk + 1].pos [1];
								landmarkDB [kk].life = landmarkDB [kk + 1].life;
								landmarkDB [kk].id = landmarkDB [kk + 1].id;
								landmarkDB [kk].totalTimesObserved = landmarkDB [kk + 1].totalTimesObserved;
							}
						}

						DBSize--;
					}
				}
			}

			return 0;
		}

		public Landmark[] UpdateAndAddLineLandmarks (Landmark[] extractedLandmarks)
		{
			// Returns the found landmarks.
			Landmark[] tempLandmarks = new Landmark[extractedLandmarks.Length];

			for (int i = 0; i < extractedLandmarks.Length; i++)
			{
				tempLandmarks [i] = UpdateLandmark (extractedLandmarks [i]);
			}

			return tempLandmarks; 
		}

		public Landmark[] UpdateAndAddLandmarksUsingEKFResults (bool[] matched, int[] id, double[] ranges,
			double[] bearings, double[] robotPosition)
		{
			Landmark[] foundLandmarks = new Landmark[matched.Length];

			for (int i = 0; i < matched.Length; i++)
			{
				foundLandmarks [i] = UpdateLandmark (matched [i], id [i], ranges [i], bearings [i],
					robotPosition);
			}

			return foundLandmarks;
		}

		public int UpdateLineLandmark (Landmark lm)
		{
			// Try to do data-association on landmark.
			int id = GetAssociation (lm);

			// If we failed to associate landmark, then add it to DB.
			if (id == -1)
			{
				id = AddToDB (lm);
			}

			return id;
		}

		public Landmark[] ExtractLineLandmarks (double[] laserdata, double[] robotPosition)
		{
			// Two arrays corresponding to found lines.
			double[] la = new double[100];
			double[] lb = new double[100];
			double[] lx1 = new double[100];
			double[] ly1 = new double[100];
			double[] lx2 = new double[100];
			double[] ly2 = new double[100];
			int totalLines = 0;

			// Array of laser data points corresponding to the seen lines.
			int[] linepoints = new int[laserdata.Length];
			int totalLinepoints = 0;

			// Have a large array to keep track of found landmarks.
			Landmark[] tempLandmarks = new Landmark[400];

			for (int i = 0; i < tempLandmarks.Length; i++)
			{
				tempLandmarks [i] = new Landmark ();
			}

			// FIXME - OR RATHER REMOVE ME SOMEHOW...
			for (int i = 0; i < laserdata.Length - 1; i++)
			{
				linepoints [totalLinepoints] = i;
				totalLinepoints++;
			}

			#region RANSAC

			// RANSAC ALGORITHM
			int numberOfTrials = 0;
			Random random = new Random ();

			while (numberOfTrials < MAXTRIALS && totalLinepoints > MINLINEPOINTS)
			{
				int[] randomSelectedPoints = new int[MAXSAMPLE];
				int temp = 0;
				bool newpoint;

				// Randomly select a subset S1 of n data points and
				// compute the model M1.
				//
				// Initial version chooses entirely randomly. Now choose
				// one point randomly and then sample from neighbours within some defined
				// radius.
				int centerPoint = random.Next (MAXSAMPLE, totalLinepoints - 1);
				randomSelectedPoints [0] = centerPoint;

				for (int i = 1; i < MAXSAMPLE; i++)
				{
					newpoint = false;

					while (!newpoint)
					{
						temp = centerPoint + (random.Next (2) - 1) * random.Next (0, MAXSAMPLE);

						for (int j = 0; j < i; j++)
						{
							if (randomSelectedPoints [j] == temp)
							{
								break;
							}

							// Point has already been selected.
							if (j >= i - 1)
							{
								newpoint = true;
							}

							// Point has not already been selected.
						}
					}

					randomSelectedPoints [i] = temp;
				}

				// Compute model M1.
				// y = a + bx
				double a = 0;
				double b = 0;
				double x1 = 0;
				double y1 = 0;
				double x2 = 0;
				double y2 = 0;

				LeastSquaresLineEstimate (laserdata, robotPosition, randomSelectedPoints, MAXSAMPLE, 
					ref a, ref b, ref x1, ref y1, ref x2, ref y2);

				// Determine the consensus set S1* of points is P
				// compatible with M1 (within some error tolerance).
				int[] consensusPoints = new int[laserdata.Length];
				int totalConsensusPoints = 0;
				int[] newLinePoints = new int[laserdata.Length];
				int totalNewLinePoints = 0;
				double x = 0; 
				double y = 0;
				double d = 0;

				for (int i = 0; i < totalLinepoints; i++)
				{
					// Convert ranges and bearing to coordinates.
					x = (Math.Cos ((linepoints [i] * degreesPerScan * conv) + robotPosition [2] * conv) *
						laserdata [linepoints [i]]) + robotPosition [0];

					y = (Math.Sin ((linepoints [i] * degreesPerScan * conv) + robotPosition [2] * conv) *
						laserdata [linepoints [i]]) + robotPosition [1];

					d = DistanceToLine (x, y, a, b);

					if (d < RANSAC_TOLERANCE)
					{
						// Add points which are close to line.
						consensusPoints [totalConsensusPoints] = linepoints [i];
						totalConsensusPoints++;
					}
					else
					{
						// Add points which are not close to line.
						newLinePoints [totalNewLinePoints] = linepoints [i];
						totalNewLinePoints++;
					}
				}

				// If #(S1*) > t, use S1* to compute (maybe using least
				// squares) a new model M1*.
				if (totalConsensusPoints > RANSAC_CONSENSUS)
				{
					// Calculate updated line equation based on consensus points.
					LeastSquaresLineEstimate (laserdata, robotPosition, consensusPoints,
						totalConsensusPoints, ref a, ref b, ref x1, ref y1, ref x2, ref y2);

					// For now add points associated to line as landmarks to see results. 
					for (int i = 0; i < totalConsensusPoints; i++)
					{
						// Remove points that have now been associated to this line.
						newLinePoints.CopyTo (linepoints, 0);
						totalLinepoints = totalNewLinePoints;
					}

					// Add line to found lines.
					la [totalLines] = a;
					lb [totalLines] = b;
					lx1 [totalLines] = x1;
					ly1 [totalLines] = y1;
					lx2 [totalLines] = x2;
					ly2 [totalLines] = y2;
					totalLines++;

					// Restart search since we found a line.
					numberOfTrials = 0;
				}
				else
				{
					numberOfTrials++;
				}
			}

			#endregion

			// For each line we found:
			// calculate the point on line closest to origin (0,0)
			// add this point as a landmark.
			for (int i = 0; i < totalLines; i++)
			{
				tempLandmarks [i] = GetLineLandmark (la [i], lb [i], robotPosition, lx1[i], ly1[i], lx2[i], ly2[i]);
			}

			// Now return found landmarks in an array of correct dimensions.
			Landmark[] foundLandmarks = new Landmark[totalLines];

			// Copy landmarks into array of correct dimensions.
			for (int i = 0; i < foundLandmarks.Length; i++)
			{
				foundLandmarks [i] = (Landmark)tempLandmarks [i];
			}

			return foundLandmarks;
		}

		public Landmark[] ExtractSpikeLandmarks (double[] laserdata, double[] robotPosition)
		{
			// Have a large array to keep track of found landmarks.
			Landmark[] tempLandmarks = new Landmark[400];

			for (int i = 0; i < tempLandmarks.Length; i++)
			{
				tempLandmarks [i] = new Landmark ();
			}
			Console.WriteLine ();
			int totalFound = 0;

			for (int i = 1; i < laserdata.Length - 1; i++)
			{
				// Check for error measurement in laser data.
				if (laserdata [i - 1] < 8.1)
				{
					if (laserdata [i + 1] < 8.1)
					{
						if ((laserdata [i - 1] - laserdata [i]) + (laserdata [i + 1] - laserdata [i]) > 0.5)
						{ 
							tempLandmarks [i] = GetLandmark (laserdata [i], i, robotPosition);
						}
						else
						{
							if ((laserdata [i - 1] - laserdata [i]) > 0.3)
							{
								tempLandmarks [i] = GetLandmark (laserdata [i], i, robotPosition);
							}
							else if (laserdata [i + 1] < 8.1)
							{
								if ((laserdata [i + 1] - laserdata [i]) > 0.3)
								{
									tempLandmarks [i] = GetLandmark (laserdata [i], i, robotPosition);
								}
							}
						}
					}
				}
			}

			// Get total found landmarks so you can return array of correct dimensions.
			for (int i = 0; i < tempLandmarks.Length; i++)
			{
				if (((int)tempLandmarks [i].id) != -1)
				{
					totalFound++;
				}
			}

			// Now return found landmarks in an array of correct dimensions.
			Landmark[] foundLandmarks = new Landmark[totalFound];

			// Copy landmarks into array of correct dimensions.
			int j = 0;

			for (int i = 0; i < ((Landmark[])tempLandmarks).Length; i++)
			{
				if (((Landmark)tempLandmarks [i]).id != -1)
				{
					foundLandmarks [j] = (Landmark)tempLandmarks [i];
					j++;
				}
			}

			return foundLandmarks;
		}

		public Landmark[] RemoveDoubles (Landmark[] extractedLandmarks)
		{
			int uniquelmrks = 0;
			double leastDistance = 99999;
			double temp;
			Landmark[] uniqueLandmarks = new Landmark[100];

			for (int i = 0; i < extractedLandmarks.Length; i++)
			{
				// Remove landmarks that didn't get associated and also pass
				// landmarks through our temporary landmark validation gate.
				if (extractedLandmarks [i].id != -1 && GetAssociation (extractedLandmarks [i]) != -1)
				{
					leastDistance = 99999;

					// Remove doubles in extractedLandmarks
					// if two observations match same landmark, take closest landmark.
					for (int j = 0; j < extractedLandmarks.Length; j++)
					{
						if (extractedLandmarks [i].id == extractedLandmarks [j].id)
						{
							if (j < i)
							{
								break;
							}

							temp = Distance (extractedLandmarks [j],
							landmarkDB [extractedLandmarks [j].id]);

							if (temp < leastDistance)
							{
								leastDistance = temp;
								uniqueLandmarks [uniquelmrks] = extractedLandmarks [j];
							}
						}
					}
				}

				if (leastDistance != 99999)
				{
					uniquelmrks++;
				}
			}

			// Copy landmarks over into an array of correct dimensions.
			extractedLandmarks = new Landmark[uniquelmrks];

			for (int i = 0; i < uniquelmrks; i++)
			{
				extractedLandmarks [i] = uniqueLandmarks [i];
			}

			return extractedLandmarks;
		}

		public int AlignLandmarkData (Landmark[] extractedLandmarks, ref bool[] matched, ref int[] id, 
			ref double[] ranges, ref double[] bearings, ref double[,] lmrks, ref double[,] exlmrks)
		{
			int uniquelmrks = 0;
			double leastDistance = 99999;
			double temp;
			Landmark[] uniqueLandmarks = new Landmark[100];

			for (int i = 0; i < extractedLandmarks.Length; i++)
			{
				if (extractedLandmarks [i].id != -1)
				{
					leastDistance = 99999;

					// Remove doubles in extractedLandmarks
					// if two observations match same landmark, take closest landmark.
					for (int j = 0; j < extractedLandmarks.Length; j++)
					{
						if (extractedLandmarks [i].id == extractedLandmarks [j].id)
						{
							if (j < i)
							{
								break;
							}

							temp = Distance (extractedLandmarks [j],
							landmarkDB [extractedLandmarks [j].id]);

							if (temp < leastDistance)
							{
								leastDistance = temp;
								uniqueLandmarks [uniquelmrks] = extractedLandmarks [j];
							}
						}
					}
				}

				if (leastDistance != 99999)
				{
					uniquelmrks++;
				}
			}

			matched = new bool[uniquelmrks];
			id = new int[uniquelmrks];
			ranges = new double[uniquelmrks];
			bearings = new double[uniquelmrks];
			lmrks = new double[uniquelmrks, 2];
			exlmrks = new double[uniquelmrks, 2];

			for (int i = 0; i < uniquelmrks; i++)
			{
				matched [i] = true;
				id [i] = uniqueLandmarks [i].id;
				ranges [i] = uniqueLandmarks [i].range;
				bearings [i] = uniqueLandmarks [i].bearing;
				lmrks [i, 0] = landmarkDB [uniqueLandmarks [i].id].pos [0];
				lmrks [i, 1] = landmarkDB [uniqueLandmarks [i].id].pos [1];
				exlmrks [i, 0] = uniqueLandmarks [i].pos [0];
				exlmrks [i, 1] = uniqueLandmarks [i].pos [1];
			}

			return 0;
		}

		public int AddToDB (Landmark lm)
		{
			if (DBSize + 1 < landmarkDB.Length)
			{
				((Landmark)landmarkDB [DBSize]).pos [0] = lm.pos [0]; // Set landmark coordinates.
				((Landmark)landmarkDB [DBSize]).pos [1] = lm.pos [1]; // Set landmark coordinates.
				((Landmark)landmarkDB [DBSize]).life = LIFE; // Set landmark life counter.
				((Landmark)landmarkDB [DBSize]).id = DBSize; // Set landmark id.
				((Landmark)landmarkDB [DBSize]).totalTimesObserved = 1; // Initialise number of times 
																		// we've seen the landmark.
				((Landmark)landmarkDB [DBSize]).bearing = lm.bearing; // Set last bearing was seen at.
				((Landmark)landmarkDB [DBSize]).range = lm.range; // Set last range was seen at.
				((Landmark)landmarkDB [DBSize]).a = lm.a; // Store landmarks wall equation.
				((Landmark)landmarkDB [DBSize]).b = lm.b; // Store landmarks wall equation.
				((Landmark)landmarkDB [DBSize]).x1y1[0] = lm.x1y1[0]; 
				((Landmark)landmarkDB [DBSize]).x1y1[1] = lm.x1y1[1]; 
				((Landmark)landmarkDB [DBSize]).x2y2[0] = lm.x2y2[0]; 
				((Landmark)landmarkDB [DBSize]).x2y2[1] = lm.x2y2[1]; 

				DBSize++;

				return (DBSize - 1);
			}

			return -1;
		}

		#endregion

		#region Private Methods

		private Landmark UpdateLandmark (bool matched, int id, double distance, double readingNo, 
		                                double[] robotPosition)
		{
			Landmark lm;

			if (matched)
			{
				// EKF matched landmark so increase times landmark has been observed.
				landmarkDB [id].totalTimesObserved++;
				lm = landmarkDB [id];
			}
			else
			{
				// EKF failed to match landmark so add it to DB as new landmark.
				lm = new Landmark ();

				// Convert landmark to map coordinate.
				lm.pos [0] = Math.Cos ((readingNo * degreesPerScan * conv) + (robotPosition [2] * 
					Math.PI / 180)
				) * distance;

				lm.pos [1] = Math.Sin ((readingNo * degreesPerScan * conv) + (robotPosition [2] * 
					Math.PI / 180)
				) * distance;

				lm.pos [0] += robotPosition [0]; // Add robot position.
				lm.pos [1] += robotPosition [1]; // Add robot position.
				lm.bearing = readingNo;
				lm.range = distance;
				id = AddToDB (lm);
				lm.id = id;
			}

			// Return landmarks.
			return lm;
		}

		private Landmark UpdateLandmark (Landmark lm)
		{
			// Try to do data-association on landmark.
			int id = GetAssociation (lm);

			// If we failed to associate landmark, then add it to DB.
			if (id == -1)
			{
				id = AddToDB (lm);
			}

			lm.id = id;

			// Return landmarks.
			return lm;
		}

		private void LeastSquaresLineEstimate (double[] laserdata, double[] robotPosition, 
			int[] SelectedPoints, int arraySize, ref double a, ref double b, 
			ref double x1, ref double y1, ref double x2, ref double y2)
		{
			double y; // y coordinate.
			double x; // x coordinate.
			double sumY = 0; // Sum of y coordinates.
			double sumYY = 0; // Sum of y^2 for each coordinate.
			double sumX = 0; // Sum of x coordinates.
			double sumXX = 0; // Sum of x^2 for each coordinate.
			double sumYX = 0; // Sum of y*x for each point.

			for (int i = 0; i < arraySize; i++)
			{
				// Convert ranges and bearing to coordinates.
				x = (Math.Cos ((SelectedPoints [i] * degreesPerScan * conv) + robotPosition [2] * conv) *
					laserdata [SelectedPoints [i]]) + robotPosition [0];

				y = (Math.Sin ((SelectedPoints [i] * degreesPerScan * conv) + robotPosition [2] * conv) *
					laserdata [SelectedPoints [i]]) + robotPosition [1];

				if (i == 0)
				{
					x1 = x;
					y1 = y;
				}
				else if (i == arraySize - 1)
				{
					x2 = x;
					y2 = y;
				}

				sumY += y;
				sumYY += Math.Pow (y, 2);

				sumX += x;
				sumXX += Math.Pow (x, 2);

				sumYX += y * x;
			}

			b = (sumY * sumXX - sumX * sumYX) / (arraySize * sumXX - Math.Pow (sumX, 2));
			a = (arraySize * sumYX - sumX * sumY) / (arraySize * sumXX - Math.Pow (sumX, 2));
		}

		private double DistanceToLine (double x, double y, double a, double b)
		{
			// Our goal is to calculate point on line closest to x, y
			// then use this to calculate distance between them.
			// calculate line perpendicular to input line. a * ao = -1.
			double ao = -1.0 / a;

			//y = aox + bo => bo = y - aox
			double bo = y - ao * x;
			 
			// Get intersection between y = ax + b and y = aox + bo
			// so aox + bo = ax + b => aox - ax = b - bo => x = (b - bo)/(ao - a), 
			// y = ao*(b - bo)/(ao - a) + bo
			double px = (b - bo) / (ao - a);
			double py = ((ao * (b - bo)) / (ao - a)) + bo;

			return Distance (x, y, px, py);
		}

		private Landmark GetLandmark (double range, int readingNo, double[] robotPosition)
		{
			Landmark lm = new Landmark ();

			// Convert landmark to map coordinate.
			lm.pos [0] = Math.Cos ((readingNo * degreesPerScan * conv) + 
				(robotPosition [2] * Math.PI / 180)
			) * range;

			lm.pos [1] = Math.Sin ((readingNo * degreesPerScan * conv) + 
				(robotPosition [2] * Math.PI / 180)
			) * range;

			lm.pos [0] += robotPosition [0]; // Add robot position.
			lm.pos [1] += robotPosition [1]; // Add robot position.
			lm.range = range;
			lm.bearing = readingNo;

			// Associate landmark to closest landmark.
			int id = -1;
			int totalTimesObserved = 0;
			GetClosestAssociation (lm, ref id, ref totalTimesObserved);
			lm.id = id;

			// Return landmarks.
			return lm;
		}

		private Landmark GetLineLandmark (double a, double b, double[] robotPosition,
			double x1, double y1, double x2, double y2)
		{
			// our goal is to calculate point on line closest to origin (0,0)
			// calculate line perpendicular to input line. a * ao = -1.
			double ao = -1.0 / a;

			// Landmark position.
			double x = b / (ao - a);
			double y = (ao * b) / (ao - a);
			double range = Math.Sqrt (Math.Pow (x - robotPosition [0], 2) + Math.Pow (y - robotPosition [1], 2));
			double bearing = Math.Atan ((y - robotPosition [1]) / (x - robotPosition [0])) - robotPosition [2];

			// Now do same calculation but get point on wall closest to robot instead:
			// y = aox + bo => bo = y - aox
			double bo = robotPosition [1] - ao * robotPosition [0];

			// Get intersection between y = ax + b and y = aox + bo
			// so: aox + bo = ax + b => aox - ax = b - bo => x = (b - bo)/(ao - a), 
			// y = ao*(b - bo)/(ao - a) +bo
			double px = (b - bo) / (ao - a);
			double py = ((ao * (b - bo)) / (ao - a)) + bo;
			double rangeError = Distance (robotPosition [0], robotPosition [1], px, py);
			double bearingError = Math.Atan ((py - robotPosition [1]) / 
				(px - robotPosition [0])
			) - robotPosition [2]; // Do you subtract or add robot bearing? I am not sure!
			Landmark lm = new Landmark ();

			// Convert landmark to map coordinate.
			lm.pos [0] = x;
			lm.pos [1] = y;
			lm.x1y1 [0] = x1;
			lm.x1y1 [1] = y1;
			lm.x2y2 [0] = x2;
			lm.x2y2 [1] = y2;
			lm.range = range;
			lm.bearing = bearing;
			lm.a = a;
			lm.b = b;
			lm.rangeError = rangeError;
			lm.bearingError = bearingError;

			// Associate landmark to closest landmark.
			int id = 0;
			int totalTimesObserved = 0;
			GetClosestAssociation (lm, ref id, ref totalTimesObserved);
			lm.id = id;
			lm.totalTimesObserved = totalTimesObserved;

			// Return landmarks
			return lm;
		}

		private Landmark GetLine (double a, double b)
		{
			// Our goal is to calculate point on line closest to origin (0, 0)
			// calculate line perpendicular to input line. a * ao = -1
			double ao = -1.0 / a;

			// Get intersection between y = ax + b and y = aox
			// so: aox = ax + b => aox - ax = b => x = b/(ao - a), y = ao*b/(ao - a)
			double x = b / (ao - a);
			double y = (ao * b) / (ao - a);
			Landmark lm = new Landmark ();

			// Convert landmark to map coordinate.
			lm.pos [0] = x;
			lm.pos [1] = y;
			lm.range = -1;
			lm.bearing = -1;
			lm.a = a;
			lm.b = b;

			// Associate landmark to closest landmark.
			int id = -1;
			int totalTimesObserved = 0;
			GetClosestAssociation (lm, ref id, ref totalTimesObserved);
			lm.id = id;

			// Return landmarks.
			return lm;
		}

		private Landmark GetOrigin ()
		{
			Landmark lm = new Landmark ();

			// Convert landmark to map coordinate.
			lm.pos [0] = 0;
			lm.pos [1] = 0;
			lm.range = -1;
			lm.bearing = -1;

			// Associate landmark to closest landmark.
			int id = -1;
			int totalTimesObserved = 0;
			GetClosestAssociation (lm, ref id, ref totalTimesObserved);
			lm.id = id;

			// Return landmarks.
			return lm;
		}

		private void GetClosestAssociation (Landmark lm, ref int id, ref int totalTimesObserved)
		{
			// Given a landmark we find the closest landmark in DB.
			int closestLandmark = 0;
			double temp;
			double leastDistance = 99999; //99999m is least initial distance, its big.

			for (int i = 0; i < DBSize; i++)
			{
				// Only associate to landmarks we have seen more than MINOBSERVATIONS times.
				if (landmarkDB [i].totalTimesObserved > MINOBSERVATIONS)
				{
					temp = Distance (lm, landmarkDB [i]);

					if (temp < leastDistance)
					{
						leastDistance = temp;
						closestLandmark = landmarkDB [i].id;
					}
				}
			}

			if (leastDistance == 99999)
			{
				id = -1;
			}
			else
			{
				id = landmarkDB [closestLandmark].id;
				totalTimesObserved = landmarkDB [closestLandmark].totalTimesObserved;
			}
		}

		private int GetAssociation (Landmark lm)
		{
			// This method needs to be improved so we use innovation as a validation gate
			// currently we just check if a landmark is within some predetermined distance 
			// of a landmark in DB.
			for (int i = 0; i < DBSize; i++)
			{
				if (Distance (lm, landmarkDB [i]) < MAXERROR && ((Landmark)landmarkDB [i]).id != -1)
				{
					((Landmark)landmarkDB [i]).life = LIFE;

					// Landmark seen so reset its life counter.
					((Landmark)landmarkDB [i]).totalTimesObserved++; // Increase number of times we seen landmark.
					((Landmark)landmarkDB [i]).bearing = lm.bearing; // Set last bearing seen at.
					((Landmark)landmarkDB [i]).range = lm.range; // Set last range seen at.

					return ((Landmark)landmarkDB [i]).id;
				}
			}

			return -1;
		}

		private double Distance (double x1, double y1, double x2, double y2)
		{
			return Math.Sqrt (Math.Pow (x1 - x2, 2) + Math.Pow (y1 - y2, 2));
		}

		private double Distance (Landmark lm1, Landmark lm2)
		{
			return Math.Sqrt (Math.Pow (lm1.pos [0] - lm2.pos [0], 2) + Math.Pow (lm1.pos [1] - lm2.pos [1], 2));
		}

		#endregion
	}
}

