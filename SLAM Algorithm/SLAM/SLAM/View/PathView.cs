using System;
using Gtk;
using Cairo;
using System.Collections.ObjectModel;

namespace SLAM
{
	/// <summary>
	/// Path view. Keeps Track of where the robot has been visually on the map
	/// </summary>
	public class PathView
	{
		Robot robot;

		#region Public Properties

		#endregion

		#region Public Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.PathView"/> class.
		/// </summary>
		/// <param name="robotModel">The instance oif the robot that will be followed.</param>
		public PathView (Robot robotModel)
		{
			robot = robotModel;

			robot.RobotUpdated += new EventHandler<RobotUpdateEventArgs> (Path_Update);
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Draw the specified cairoContext, centerX, centerY and scale.
		/// </summary>
		/// <param name="cairoContext">Cairo context.</param>
		/// <param name="centerX">Center x.</param>
		/// <param name="centerY">Center y.</param>
		/// <param name="scale">Scale.</param>
		public void Draw (Cairo.Context cairoContext, int centerX, int centerY, double scale)
		{
 
			cairoContext.SetSourceRGB (0, 0, 200);
			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt;
			//cairoContext.MoveTo (OriginalX, -OriginalY);
			//int x = centerX;
			Console.WriteLine ("point: " + (robot.PathPointList [robot.PathPointList.Count-1] [1]*100));
			if (30 <= (robot.PathPointList [robot.PathPointList.Count - 1] [1] * 100)) {
				robot.Halt ();
				Console.WriteLine ("\n\n Has Gone 30cm \n\n");
			}
			//Console.WriteLine ("Hello");

			for (int i = 1; i < robot.PathPointList.Count; i++)
			{
				cairoContext.MoveTo (centerX - (robot.PathPointList [i - 1] [0] * 100), centerY - (robot.PathPointList [i - 1] [1] * 100));
				cairoContext.LineTo (centerX - (robot.PathPointList [i] [0] * 100), centerY - (robot.PathPointList [i] [1] * 100));
				//	Console.WriteLine (path[0]*100+" , "+ path[1]*100);
				cairoContext.Stroke ();

			}
			foreach (double[] path in robot.PathPointList)
			{
				//cairoContext.MoveTo (centerX - (path[0] * 100), centerY - (path[1] * 100));
			}
		}

		#endregion
		/// <summary>
		/// Path_s the update.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Path_Update (object sender, RobotUpdateEventArgs e)
		{
			// On the first update.
		}
	}
}

