using Gtk;
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
			Application.Init ();
			new MapView (new Map ());
			Application.Run ();
		}
	}
}

