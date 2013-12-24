using System;

namespace SLAM
{
	/// <summary>
	/// Summary description for Landmarks.
	/// </summary>
	public class Landmarks
	{
		double conv = Math.PI / 180.0; // Convert to radians
		const int MAXLANDMARKS = 3000;
		const double MAXERROR = 0.5; // if a landmark is within 20 cm of another landmark its the same landmark
		public int MINOBSERVATIONS = 15; // Number of times a landmark must be observed to be recognized as a landmark
		const int LIFE = 40;
		const double MAX_RANGE = 1;
		const int MAXTRIALS = 1000; //RANSAC: max times to run algorithm
		const int MAXSAMPLE = 10; //RANSAC: randomly select X points
		const int MINLINEPOINTS = 30; //RANSAC: if less than 40 points left don't bother trying to find consensus (stop algorithm)
		const double RANSAC_TOLERANCE = 0.05; //RANSAC: if point is within x distance of line its part of line
		const int RANSAC_CONSENSUS = 30; //RANSAC: at least 30 votes required to determine if a line
		double degreesPerScan = 0.5;

		public class landmark
		{
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

			public landmark()
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

		landmark[] landmarkDB = new landmark[MAXLANDMARKS];
		int DBSize = 0;
		int[,] IDtoID = new int[MAXLANDMARKS,2];
		int EKFLandmarks = 0;

		public int GetSlamID(int id)
		{
			for(int i=0; i<EKFLandmarks; i++)
			{
				if(IDtoID[i, 0] == id)
					return IDtoID[i,1];
			}

			return -1;
		}

		public int AddSlamID(int landmarkID, int slamID)
		{
			IDtoID[EKFLandmarks, 0] = landmarkID;
			IDtoID[EKFLandmarks, 1] = slamID;
			EKFLandmarks++;
			return 0;
		}

		public Landmarks(double degreesPerScan)
		{
			this.degreesPerScan = degreesPerScan;
			for(int i=0; i<landmarkDB.Length; i++)
			{
				landmarkDB[i] = new landmark();
			}
		}

		public int RemoveBadLandmarks(double[] laserdata, double[] robotPosition)
		{
			double maxrange = 0;

			for(int i=1; i<laserdata.Length-1; i++)
			{
				//distance further away than 8.1m we assume are failed returns
				//we get the laser data with max range
			if (laserdata[i-1] < 8.1)
				if (laserdata[i+1] < 8.1)
					if (laserdata[i] > maxrange)
						maxrange = laserdata[i];
			}

			maxrange = MAX_RANGE;
			double[] Xbounds = new double[4];
			double[] Ybounds = new double[4];

			//get bounds of rectangular box to remove bad landmarks from
			Xbounds[0] =Math.Cos((1 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[0];
			Ybounds[0] =Math.Sin((1 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[1];
			Xbounds[1] =Xbounds[0]+Math.Cos((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Ybounds[1] =Ybounds[0]+Math.Sin((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Xbounds[2] =Math.Cos((359 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[0];
			Ybounds[2] =Math.Sin((359 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[1];
			Xbounds[3] =Xbounds[2]+Math.Cos((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Ybounds[3] =Ybounds[2]+Math.Sin((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;

			/*
			// the below code starts the box 1 meter in front of robot
			Xbounds[0] =Math.Cos((0 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[0];
			Xbounds[1] =Xbounds[0]+Math.Cos((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Xbounds[0] =Xbounds[0]+Math.Cos((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180));
			//make box start 1 meter ahead of robot
			Ybounds[0] =Math.Sin((0 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[1];
			Ybounds[1] =Ybounds[0]+Math.Sin((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Ybounds[0] =Ybounds[0]+Math.Sin((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180));
			//make box start 1 meter ahead of robot
			Xbounds[2] =Math.Cos((360 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[0];
			Xbounds[3] =Xbounds[2]+Math.Cos((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Xbounds[2] =Xbounds[2]+Math.Cos((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180));
			//make box start 1 meter ahead of robot
			Ybounds[2] =Math.Sin((360 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange+robotPosition[1];
			Ybounds[3] =Ybounds[2]+Math.Sin((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
			maxrange;
			Ybounds[2] =Ybounds[2]+Math.Sin((180 * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180));
			//make box start 1 meter ahead of robot
			*/

			//now check DB for landmarks that are within this box
			//decrease life of all landmarks in box. If the life reaches zero, remove landmark
			double pntx, pnty;

			for(int k=0; k<DBSize+1; k++)
			{
				pntx = landmarkDB[k].pos[0];
				pnty = landmarkDB[k].pos[1];
				int i = 0;
				int j = 0;
				bool inRectangle;

				//if(robotPosition[2]>0 && robotPosition[2]<180)

				if(robotPosition[0]<0 || robotPosition[1]<0)
					inRectangle = false;
				else
					inRectangle = true;

				for(i=0; i < 4; i++)
				{
					if (((((Ybounds[i] <= pnty) && (pnty < Ybounds[j])) || ((Ybounds[j] <= pnty) && (pnty
						< Ybounds[i]))) && (pntx < (Xbounds[j] - Xbounds[i]) * (pnty - Ybounds[i]) / (Ybounds[j] - Ybounds[i]) +
						Xbounds[i])))
					{
						if(inRectangle==false)
							inRectangle=true;
						else
							inRectangle=false;
					}
					j = i;
					i = i + 1;
				}

				if(inRectangle)
				{
					//in rectangle so decrease life and maybe remove
					landmarkDB[k].life--;
					if(landmarkDB[k].life<=0)
					{
						for(int kk=k; kk<DBSize; kk++)
						//remove landmark by copying down rest of DB
						{
							if(kk==DBSize-1)
							{
							landmarkDB[kk].pos[0] = landmarkDB[kk+1].pos[0];
							landmarkDB[kk].pos[1] = landmarkDB[kk+1].pos[1];
							landmarkDB[kk].life = landmarkDB[kk+1].life;
							landmarkDB[kk].id = landmarkDB[kk+1].id;
							landmarkDB[kk].totalTimesObserved =
							landmarkDB[kk+1].totalTimesObserved;
							}
							else
							{
							landmarkDB[kk+1].id--;
							landmarkDB[kk].pos[0] = landmarkDB[kk+1].pos[0];
							landmarkDB[kk].pos[1] = landmarkDB[kk+1].pos[1];
							landmarkDB[kk].life = landmarkDB[kk+1].life;
							landmarkDB[kk].id = landmarkDB[kk+1].id;
							landmarkDB[kk].totalTimesObserved =
							landmarkDB[kk+1].totalTimesObserved;
							}
						}
					DBSize--;

					}
				}
			}
			return 0;
		}

		public landmark[] UpdateAndAddLineLandmarks(landmark[] extractedLandmarks)
		{
			//returns the found landmarks
			landmark[] tempLandmarks = new landmark[extractedLandmarks.Length];

			for(int i=0; i<extractedLandmarks.Length; i++)
				tempLandmarks[i] = UpdateLandmark(extractedLandmarks[i]);

			return tempLandmarks;
		}

		/*
		public landmark[] UpdateAndAddLandmarks(double[] laserdata, double[] robotPosition)
		{
		//have a large array to keep track of found landmarks
		landmark[] tempLandmarks = new landmark[400];
		for(int i=0; i<tempLandmarks.Length;i++)
		tempLandmarks[i]= new landmark();
		int totalFound = 0;
		double val = laserdata[0];
		for (int i = 1; i < laserdata.Length - 1; i++)
		{
		// Check for error measurement in laser data
		if (laserdata[i-1] < 8.1)
		if (laserdata[i+1] < 8.1)
		if ((laserdata[i-1] - laserdata[i]) + (laserdata[i+1] - laserdata[i]) > 0.5)
		tempLandmarks[i] = UpdateLandmark(laserdata[i], i, robotPosition);
		else
		if((laserdata[i-1] - laserdata[i]) > 0.3)
		tempLandmarks[i] = UpdateLandmark(laserdata[i], i, robotPosition);
		else if (laserdata[i+1] < 8.1)
		if((laserdata[i+1] - laserdata[i]) > 0.3)
		tempLandmarks[i] = UpdateLandmark(laserdata[i], i, robotPosition);
		}
		//get total found landmarks so you can return array of correct dimensions
		for(int i=0; i<tempLandmarks.Length;i++)
		if(((int)tempLandmarks[i].id) !=-1)
		totalFound++;
		//now return found landmarks in an array of correct dimensions
		landmark[] foundLandmarks = new landmark[totalFound];
		//copy landmarks into array of correct dimensions
		int j = 0;
		for(int i=0; i<((landmark[])tempLandmarks).Length;i++)
		if(((landmark)tempLandmarks[i]).id !=-1)
		{
		foundLandmarks[j] = (landmark)tempLandmarks[i];
		j++;
		}
		return foundLandmarks;
		}
		*/

		public landmark[] UpdateAndAddLandmarksUsingEKFResults(bool[] matched, int[] id, double[] ranges,
		double[] bearings, double[] robotPosition)
		{
			landmark[] foundLandmarks = new landmark[matched.Length];

			for(int i=0; i< matched.Length; i++)
			{
				foundLandmarks[i] = UpdateLandmark(matched[i], id[i], ranges[i], bearings[i],
				robotPosition);
			}

			return foundLandmarks;
		}

		private landmark UpdateLandmark(bool matched, int id, double distance, double readingNo, double[]
		robotPosition)
		{
			landmark lm;
			if(matched)
			{
				//EKF matched landmark so increase times landmark has been observed
				landmarkDB[id].totalTimesObserved++;
				lm = landmarkDB[id];
			}
			else
			{
				//EKF failed to match landmark so add it to DB as new landmark
				lm = new landmark();
				//convert landmark to map coordinate
				lm.pos[0] =Math.Cos((readingNo * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
				distance;
				lm.pos[1] =Math.Sin((readingNo * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) *
				distance;
				lm.pos[0]+=robotPosition[0]; //add robot position
				lm.pos[1]+=robotPosition[1]; //add robot position
				lm.bearing = readingNo;
				lm.range = distance;
				id = AddToDB(lm);
				lm.id = id;
			}

			//return landmarks
			return lm;
		}

		private landmark UpdateLandmark(landmark lm)
		{
			//try to do data-association on landmark.
			int id = GetAssociation(lm);

			//if we failed to associate landmark, then add it to DB
			if(id==-1)
				id = AddToDB(lm);

			lm.id = id;

			//return landmarks
			return lm;
		}

		public int UpdateLineLandmark(landmark lm)
		{
			//try to do data-association on landmark.
			int id = GetAssociation(lm);

			//if we failed to associate landmark, then add it to DB
			if(id==-1)
				id = AddToDB(lm);

			return id;
		}	

		public landmark[] ExtractLineLandmarks(double[] laserdata, double[] robotPosition)
		{
			//two arrays corresponding to found lines
			double[] la = new double[100];
			double[] lb = new double[100];
			int totalLines = 0;

			//array of laser data points corresponding to the seen lines
			int[] linepoints = new int[laserdata.Length];
			int totalLinepoints = 0;

			//have a large array to keep track of found landmarks
			landmark[] tempLandmarks = new landmark[400];

			for(int i=0; i<tempLandmarks.Length;i++)
				tempLandmarks[i]= new landmark();

			int totalFound = 0;
			double val = laserdata[0];
			double lastreading = laserdata[2];
			double lastlastreading = laserdata[2];

			/*
			//removes worst outliers (points which for sure aren't on any lines)
			for (int i = 2; i < laserdata.Length - 1; i++)
			{
			// Check for error measurement in laser data
			if (laserdata[i] < 8.1)
			if(Math.Abs(laserdata[i]-lastreading)+Math.Abs(lastreading-lastlastreading) < 0.2)
			{
			linepoints[totalLinepoints] = i;
			totalLinepoints++;
			//tempLandmarks[i] = GetLandmark(laserdata[i], i, robotPosition);
			lastreading = laserdata[i];
			lastreading = laserdata[i-1];
			}
			else
			{
			lastreading = laserdata[i];
			lastlastreading = laserdata[i-1];
			}
			}
			*/

			//FIXME - OR RATHER REMOVE ME SOMEHOW...
			for (int i = 0; i < laserdata.Length - 1; i++)
			{
			linepoints[totalLinepoints] = i;
			totalLinepoints++;
			}

			#region RANSAC
			//RANSAC ALGORITHM
			int noTrials = 0;
			Random rnd = new Random();

			while(noTrials<MAXTRIALS && totalLinepoints > MINLINEPOINTS)
			{
				int[] rndSelectedPoints = new int[MAXSAMPLE];
				int temp = 0;
				bool newpoint;

				//– Randomly select a subset S1 of n data points and
				//compute the model M1
				//Initial version chooses entirely randomly. Now choose
				//one point randomly and then sample from neighbours within some defined
				//radius
				int centerPoint = rnd.Next(MAXSAMPLE, totalLinepoints-1);
				rndSelectedPoints[0] = centerPoint;

				for(int i = 1; i<MAXSAMPLE; i++)
				{
					newpoint = false;

					while(!newpoint)
					{
						temp = centerPoint + (rnd.Next(2)-1)*rnd.Next(0, MAXSAMPLE);

						for(int j = 0; j<i; j++)
						{
							if(rndSelectedPoints[j] == temp)
								break;

							//point has already been selected
							if(j>=i-1)
								newpoint = true;

							//point has not already been selected
						}
					}
					rndSelectedPoints[i] = temp;
				}

				//compute model M1
				double a=0;
				double b=0;
				//y = a+ bx
				LeastSquaresLineEstimate(laserdata, robotPosition, rndSelectedPoints, MAXSAMPLE, ref a, ref
				b);

				//– Determine the consensus set S1* of points is P
				//compatible with M1 (within some error tolerance)
				int[] consensusPoints = new int[laserdata.Length];
				int totalConsensusPoints = 0;
				int[] newLinePoints = new int[laserdata.Length];
				int totalNewLinePoints = 0;
				double x = 0, y =0;
				double d = 0;

				for(int i=0; i<totalLinepoints; i++)
				{
					//convert ranges and bearing to coordinates
					x =(Math.Cos((linepoints[i] * degreesPerScan * conv) + robotPosition[2]*conv) *
					laserdata[linepoints[i]])+ robotPosition[0];
					y =(Math.Sin((linepoints[i] * degreesPerScan * conv) + robotPosition[2]*conv) *
					laserdata[linepoints[i]])+ robotPosition[1];

					//x =(Math.Cos((linepoints[i] * degreesPerScan * conv)) * laserdata[linepoints[i]]);//+robotPosition[0];
					//y =(Math.Sin((linepoints[i] * degreesPerScan * conv)) * laserdata[linepoints[i]]);//+robotPosition[1];

					d = DistanceToLine(x, y, a, b);

					if (d<RANSAC_TOLERANCE)
					{
						//add points which are close to line
						consensusPoints[totalConsensusPoints] = linepoints[i];
						totalConsensusPoints++;
					}
					else
					{
						//add points which are not close to line
						newLinePoints[totalNewLinePoints] = linepoints[i];
						totalNewLinePoints++;
					}
				}

				//– If #(S1*) > t, use S1* to compute (maybe using least
				//squares) a new model M1*
				if(totalConsensusPoints>RANSAC_CONSENSUS)
				{
					//Calculate updated line equation based on consensus points
					LeastSquaresLineEstimate(laserdata, robotPosition, consensusPoints,
					totalConsensusPoints, ref a, ref b);

					//for now add points associated to line as landmarks to see results
					for(int i=0; i<totalConsensusPoints; i++)
					{
						//tempLandmarks[consensusPoints[i]] = GetLandmark(laserdata[consensusPoints[i]],consensusPoints[i], robotPosition);
						//Remove points that have now been associated to this line
						newLinePoints.CopyTo(linepoints, 0);
						totalLinepoints = totalNewLinePoints;
					}

					//add line to found lines
					la[totalLines] = a;
					lb[totalLines] = b;
					totalLines++;

					//restart search since we found a line
					//noTrials = MAXTRIALS;
					//when maxtrials = debugging
					noTrials = 0;
					}
					else
					//DEBUG add point that we chose as middle value
					//tempLandmarks[centerPoint] = GetLandmark(laserdata[centerPoint], centerPoint, robotPosition);
					//– If #(S1*) < t, randomly select another subset S2 and
					//repeat
					//– If, after some predetermined number of trials there is
					//no consensus set with t points, return with failure
					noTrials++;
			}
			#endregion

			//for each line we found:
			//calculate the point on line closest to origin (0,0)
			//add this point as a landmark
			for(int i=0; i < totalLines; i++)
			{
				tempLandmarks[i] = GetLineLandmark(la[i], lb[i], robotPosition);
				//tempLandmarks[i+1] = GetLine(la[i], lb[i]);
			}

			//for debug add origin as landmark
			//tempLandmarks[totalLines+1] = GetOrigin();
			//tempLandmarks[i] = GetLandmark(laserdata[i], i, robotPosition);
			//now return found landmarks in an array of correct dimensions
			landmark[] foundLandmarks = new landmark[totalLines];

			//copy landmarks into array of correct dimensions
			for(int i=0; i<foundLandmarks.Length;i++)
			{
				foundLandmarks[i] = (landmark)tempLandmarks[i];
			}

			return foundLandmarks;
		}

		private void LeastSquaresLineEstimate(double[] laserdata, double[] robotPosition, int[] SelectedPoints,
		int arraySize, ref double a, ref double b)
		{
			double y; //y coordinate
			double x; //x coordinate
			double sumY=0; //sum of y coordinates
			double sumYY=0; //sum of y^2 for each coordinate
			double sumX=0; //sum of x coordinates
			double sumXX=0; //sum of x^2 for each coordinate
			double sumYX=0; //sum of y*x for each point

			//DEBUG
			/*
			double[] testX = {0, 1};
			double[] testY = {1, 1};
			for(int i = 0; i < 2; i++)
			{
			//convert ranges and bearing to coordinates
			x = testX[i];
			y = testY[i];
			sumY
			+= y;
			sumYY += Math.Pow(y,2);
			sumX
			+= x;
			sumXX += Math.Pow(x,2);
			sumYX += y*x;
			}
			a = (sumY*sumXX-sumX*sumYX)/(testX.Length*sumXX-Math.Pow(sumX, 2));
			b = (testX.Length*sumYX-sumX*sumY)/(testX.Length*sumXX-Math.Pow(sumX, 2));
			*/

			for(int i = 0; i < arraySize; i++)
			{
				//convert ranges and bearing to coordinates
				x =(Math.Cos((SelectedPoints[i] * degreesPerScan * conv) + robotPosition[2]*conv) *
				laserdata[SelectedPoints[i]])+robotPosition[0];
				y =(Math.Sin((SelectedPoints[i] * degreesPerScan * conv) + robotPosition[2]*conv) *
				laserdata[SelectedPoints[i]])+robotPosition[1];

				//x =(Math.Cos((rndSelectedPoints[i] * degreesPerScan * conv)) * laserdata[rndSelectedPoints[i]]);//+robotPosition[0];
				//y =(Math.Sin((rndSelectedPoints[i] * degreesPerScan * conv)) * laserdata[rndSelectedPoints[i]]);//+robotPosition[1];

				sumY
				+= y;
				sumYY += Math.Pow(y,2);
				sumX
				+= x;
				sumXX += Math.Pow(x,2);
				sumYX += y*x;
			}

			b = (sumY*sumXX-sumX*sumYX)/(arraySize*sumXX-Math.Pow(sumX, 2));
			a = (arraySize*sumYX-sumX*sumY)/(arraySize*sumXX-Math.Pow(sumX, 2));
		}

		private double DistanceToLine(double x,double y,double a,double b)
		{
			/*
			//y = ax + b
			//0 = ax + b - y
			double d = Math.Abs((a*x - y + b)/(Math.Sqrt(Math.Pow(a,2)+ Math.Pow(b,2))));
			*/

			//our goal is to calculate point on line closest to x,y
			//then use this to calculate distance between them.
			//calculate line perpendicular to input line. a*ao = -1
			double ao = -1.0/a;

			//y = aox + bo => bo = y - aox
			double bo = y - ao*x;

			//get intersection between y = ax + b and y = aox + bo
			//so aox + bo = ax + b => aox - ax = b - bo => x = (b - bo)/(ao - a), y = ao*(b - bo)/(ao - a) +bo
			double px = (b - bo)/(ao - a);
			double py = ((ao*(b - bo))/(ao - a)) + bo;
			return Distance(x, y, px, py);
		}

		public landmark[] ExtractSpikeLandmarks(double[] laserdata, double[] robotPosition)
		{
			//have a large array to keep track of found landmarks
			landmark[] tempLandmarks = new landmark[400];

			for(int i=0; i<tempLandmarks.Length;i++)
				tempLandmarks[i]= new landmark();

			int totalFound = 0;
			double val = laserdata[0];

			for (int i = 1; i < laserdata.Length - 1; i++)
			{
				// Check for error measurement in laser data
				if (laserdata[i-1] < 8.1)
					if (laserdata[i+1] < 8.1)
						if ((laserdata[i-1] - laserdata[i]) + (laserdata[i+1] - laserdata[i]) > 0.5)
							tempLandmarks[i] = GetLandmark(laserdata[i], i, robotPosition);
						else
							if((laserdata[i-1] - laserdata[i]) > 0.3)
								tempLandmarks[i] = GetLandmark(laserdata[i], i, robotPosition);
							else if (laserdata[i+1] < 8.1)
								if((laserdata[i+1] - laserdata[i]) > 0.3)
									tempLandmarks[i] = GetLandmark(laserdata[i], i, robotPosition);
			}

			//get total found landmarks so you can return array of correct dimensions
			for(int i=0; i<tempLandmarks.Length;i++)
				if(((int)tempLandmarks[i].id) !=-1)
					totalFound++;

			//now return found landmarks in an array of correct dimensions
			landmark[] foundLandmarks = new landmark[totalFound];

			//copy landmarks into array of correct dimensions
			int j = 0;

			for(int i=0; i<((landmark[])tempLandmarks).Length;i++)
				if(((landmark)tempLandmarks[i]).id !=-1)
				{
					foundLandmarks[j] = (landmark)tempLandmarks[i];
					j++;
				}

			return foundLandmarks;
		}

		private landmark GetLandmark(double range, int readingNo, double[] robotPosition)
		{
			landmark lm = new landmark();

			//convert landmark to map coordinate
			lm.pos[0] =Math.Cos((readingNo * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) * range;
			lm.pos[1] =Math.Sin((readingNo * degreesPerScan * conv)+(robotPosition[2]*Math.PI/180)) * range;
			lm.pos[0]+=robotPosition[0]; //add robot position
			lm.pos[1]+=robotPosition[1]; //add robot position
			lm.range = range;
			lm.bearing = readingNo;

			//associate landmark to closest landmark.
			int id = -1;
			int totalTimesObserved =0;
			GetClosestAssociation(lm, ref id, ref totalTimesObserved);
			lm.id = id;

			//return landmarks
			return lm;
		}

		private landmark GetLineLandmark(double a, double b, double[] robotPosition)
		{
			//our goal is to calculate point on line closest to origin (0,0)
			//calculate line perpendicular to input line. a*ao = -1
			double ao = -1.0/a;

			//landmark position
			double x = b/(ao - a);
			double y = (ao*b)/(ao - a);
			double range = Math.Sqrt(Math.Pow(x-robotPosition[0],2) + Math.Pow(y-robotPosition[1],2));
			double bearing = Math.Atan((y-robotPosition[1])/(x-robotPosition[0])) - robotPosition[2];

			//now do same calculation but get point on wall closest to robot instead
			//y = aox + bo => bo = y - aox
			double bo = robotPosition[1] - ao*robotPosition[0];

			//get intersection between y = ax + b and y = aox + bo
			//so aox + bo = ax + b => aox - ax = b - bo => x = (b - bo)/(ao - a), y = ao*(b - bo)/(ao - a) +bo
			double px = (b - bo)/(ao - a);
			double py = ((ao*(b - bo))/(ao - a)) + bo;
			double rangeError = Distance(robotPosition[0], robotPosition[1], px, py);
			double bearingError = Math.Atan((py - robotPosition[1])/(px - robotPosition[0])) -
			robotPosition[2]; //do you subtract or add robot bearing? I am not sure!
			landmark lm = new landmark();

			//convert landmark to map coordinate
			lm.pos[0] =x;
			lm.pos[1] =y;
			lm.range = range;
			lm.bearing = bearing;
			lm.a = a;
			lm.b = b;
			lm.rangeError = rangeError;
			lm.bearingError = bearingError;

			//associate landmark to closest landmark.
			int id = 0;
			int totalTimesObserved = 0;
			GetClosestAssociation(lm, ref id, ref totalTimesObserved);
			lm.id = id;
			lm.totalTimesObserved = totalTimesObserved;

			//return landmarks
			return lm;
		}

		private landmark GetLine(double a, double b)
		{
			//our goal is to calculate point on line closest to origin (0,0)
			//calculate line perpendicular to input line. a*ao = -1
			double ao = -1.0/a;

			//get intersection between y = ax + b and y = aox
			//so aox = ax + b => aox - ax = b => x = b/(ao - a), y = ao*b/(ao - a)
			double x = b/(ao - a);
			double y = (ao*b)/(ao - a);
			landmark lm = new landmark();

			//convert landmark to map coordinate
			lm.pos[0] =x;
			lm.pos[1] =y;
			lm.range = -1;
			lm.bearing = -1;
			lm.a = a;
			lm.b = b;

			//associate landmark to closest landmark.
			int id = -1;
			int totalTimesObserved = 0;
			GetClosestAssociation(lm, ref id, ref totalTimesObserved);
			lm.id = id;

			//return landmarks
			return lm;
		}

		private landmark GetOrigin()
		{
			landmark lm = new landmark();

			//convert landmark to map coordinate
			lm.pos[0] =0;
			lm.pos[1] =0;
			lm.range = -1;
			lm.bearing = -1;

			//associate landmark to closest landmark.
			int id = -1;
			int totalTimesObserved = 0;
			GetClosestAssociation(lm, ref id, ref totalTimesObserved);
			lm.id = id;

			//return landmarks
			return lm;
		}

		private void GetClosestAssociation(landmark lm, ref int id, ref int totalTimesObserved)
		{
			//given a landmark we find the closest landmark in DB
			int closestLandmark = 0;
			double temp;
			double leastDistance = 99999; //99999m is least initial distance, its big

			for(int i=0; i<DBSize; i++)
			{
				//only associate to landmarks we have seen more than MINOBSERVATIONS times
				if(landmarkDB[i].totalTimesObserved>MINOBSERVATIONS)
				{
					temp = Distance(lm, landmarkDB[i]);

					if(temp<leastDistance)
					{
						leastDistance = temp;
						closestLandmark = landmarkDB[i].id;
					}
				}
			}

			if(leastDistance == 99999)
				id = -1;
			else
			{
				id = landmarkDB[closestLandmark].id;
				totalTimesObserved = landmarkDB[closestLandmark].totalTimesObserved;
			}
		}

		private int GetAssociation(landmark lm)
		{
			//this method needs to be improved so we use innovation as a validation gate
			//currently we just check if a landmark is within some predetermined distance of a landmark in DB
			for(int i=0; i<DBSize; i++)
			{
				if(Distance(lm, landmarkDB[i])<MAXERROR
				&& ((landmark)landmarkDB[i]).id != -1)
				{
					((landmark)landmarkDB[i]).life=LIFE;

					//landmark seen so reset its life counter
					((landmark)landmarkDB[i]).totalTimesObserved++; //increase number of times we seen landmark
					((landmark)landmarkDB[i]).bearing = lm.bearing; //set last bearing seen at
					((landmark)landmarkDB[i]).range = lm.range; //set last range seen at

					return ((landmark)landmarkDB[i]).id;
				}
			}

			return -1;
		}

		public landmark[] RemoveDoubles(landmark[] extractedLandmarks)
		{
			int uniquelmrks = 0;
			double leastDistance = 99999;
			double temp;
			landmark[] uniqueLandmarks = new landmark[100];

			for(int i=0; i<extractedLandmarks.Length; i++)
			{
				//remove landmarks that didn't get associated and also pass
				//landmarks through our temporary landmark validation gate
				if(extractedLandmarks[i].id != -1
				&& GetAssociation(extractedLandmarks[i]) != -1)
				{
					leastDistance = 99999;

					//remove doubles in extractedLandmarks
					//if two observations match same landmark, take closest landmark
					for(int j=0; j<extractedLandmarks.Length; j++)
					{
						if(extractedLandmarks[i].id == extractedLandmarks[j].id)
						{
							if (j < i)
								break;
							temp = Distance(extractedLandmarks[j],
							landmarkDB[extractedLandmarks[j].id]);

							if(temp<leastDistance)
							{
								leastDistance = temp;
								uniqueLandmarks[uniquelmrks] = extractedLandmarks[j];
							}
						}
					}
				}

				if (leastDistance != 99999)
					uniquelmrks++;
			}

			//copy landmarks over into an array of correct dimensions
			extractedLandmarks = new landmark[uniquelmrks];

			for(int i=0; i<uniquelmrks; i++)
			{
				extractedLandmarks[i] = uniqueLandmarks[i];
			}

			return extractedLandmarks;
		}

		public int AlignLandmarkData(landmark[] extractedLandmarks,ref bool[] matched, ref int[] id, ref
		double[] ranges, ref double[] bearings, ref double[,] lmrks, ref double[,] exlmrks)
		{
			int uniquelmrks = 0;
			double leastDistance = 99999;
			double temp;
			landmark[] uniqueLandmarks = new landmark[100];

			for(int i=0; i<extractedLandmarks.Length; i++)
			{
				if(extractedLandmarks[i].id != -1)
				{
					leastDistance = 99999;

					//remove doubles in extractedLandmarks
					//if two observations match same landmark, take closest landmark
					for(int j=0; j<extractedLandmarks.Length; j++)
					{
						if(extractedLandmarks[i].id == extractedLandmarks[j].id)
						{
							if (j < i)
								break;
							temp = Distance(extractedLandmarks[j],
							landmarkDB[extractedLandmarks[j].id]);

							if(temp<leastDistance)
							{
								leastDistance = temp;
								uniqueLandmarks[uniquelmrks] = extractedLandmarks[j];
							}
						}
					}
				}

				if (leastDistance != 99999)
					uniquelmrks++;
			}

			matched = new bool[uniquelmrks];
			id = new int[uniquelmrks];
			ranges = new double[uniquelmrks];
			bearings = new double[uniquelmrks];
			lmrks = new double[uniquelmrks,2];
			exlmrks = new double[uniquelmrks,2];

			for(int i=0; i<uniquelmrks; i++)
			{
				matched[i] = true;
				id[i] = uniqueLandmarks[i].id;
				ranges[i] = uniqueLandmarks[i].range;
				bearings[i] = uniqueLandmarks[i].bearing;
				lmrks[i,0] = landmarkDB[uniqueLandmarks[i].id].pos[0];
				lmrks[i,1] = landmarkDB[uniqueLandmarks[i].id].pos[1];
				exlmrks[i,0] = uniqueLandmarks[i].pos[0];
				exlmrks[i,1] = uniqueLandmarks[i].pos[1];
			}

			return 0;
		}

		public int AddToDB(landmark lm)
		{
			if(DBSize+1 < landmarkDB.Length)
			{
				//for(int i=0; i<DBSize+1; i++)
				//{
				//if(((landmark)landmarkDB[i]).id != i)//if(((landmark)landmarkDB[i]).id == -1||((landmark)landmarkDB[i]).life <= 0)
				//{
				((landmark)landmarkDB[DBSize]).pos[0] = lm.pos[0]; //set landmark coordinates
				((landmark)landmarkDB[DBSize]).pos[1] = lm.pos[1]; //set landmark coordinates
				((landmark)landmarkDB[DBSize]).life = LIFE; //set landmark life counter
				((landmark)landmarkDB[DBSize]).id = DBSize; //set landmark id
				((landmark)landmarkDB[DBSize]).totalTimesObserved = 1; //initialise number of times we'veseen landmark
				((landmark)landmarkDB[DBSize]).bearing = lm.bearing; //set last bearing was seen at
				((landmark)landmarkDB[DBSize]).range = lm.range; //set last range was seen at
				((landmark)landmarkDB[DBSize]).a = lm.a; //store landmarks wall equation
				((landmark)landmarkDB[DBSize]).b= lm.b; //store landmarks wall equation
				DBSize++;

				return (DBSize-1);
			}

			return -1;
		}

		public int GetDBSize()
		{
			return DBSize;
		}

		public landmark[] GetDB()
		{
			landmark[] temp = new landmark[DBSize];

			for(int i=0; i<DBSize; i++)
			{
				temp[i] = landmarkDB[i];
			}

			return temp;
		}
		private double Distance(double x1, double y1, double x2, double y2)
		{
			return Math.Sqrt(Math.Pow(x1-x2,2)+Math.Pow(y1-y2, 2));
		}

		private double Distance(landmark lm1, landmark lm2)
		{
			return Math.Sqrt(Math.Pow(lm1.pos[0]-lm2.pos[0],2)+Math.Pow(lm1.pos[1]-lm2.pos[1], 2));
		}
	}
}

