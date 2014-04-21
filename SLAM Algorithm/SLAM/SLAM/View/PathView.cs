using System;
using Gtk;
using Cairo;
using System.Collections.ObjectModel;

namespace SLAM
{
	public class PathView
	{
		Robot robot;

		#region Public Properties

		#endregion

		#region Public Constructors

		public PathView (Robot robotModel)
		{
			robot = robotModel;

			robot.RobotUpdated += new EventHandler<RobotUpdateEventArgs> (Path_Update);
		}

		#endregion

		#region Public Methods

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

		private void Path_Update (object sender, RobotUpdateEventArgs e)
		{
			// On the first update.
		}
	}
}

