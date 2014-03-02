using System;
using Gtk;
using Cairo;

namespace SLAM
{
	/// <summary>
	/// Robot view.
	/// </summary>
	public class RobotView
	{
		private Robot robot; // Robot model that this view represents.

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.RobotView"/> class.
		/// </summary>
		/// <param name="robotModel">Robot model that this view represents.</param>
		public RobotView (Robot robotModel)
		{
			robot = robotModel;
			robot.RobotUpdated += new EventHandler<RobotUpdateEventArgs> (Robot_Update);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Draw the robot taking into account the center x and y position of the map which
		/// will be different from the true center x and y positions on the drawing context.
		/// This method will result in a red wheeled robot with black tyres being drawn at
		/// the robots location on the map.
		/// 
		/// The scale value is currently unused but it could be useful if the map was scaled
		/// in some way for example a mini-map may be 10 times smaller than the original 
		/// results in 1:10 scale robot.
		/// </summary>
		/// <param name="cairoContext">Cairo context to draw to (assuming a map).</param>
		/// <param name="centerX">Center x position of map to draw onto.</param>
		/// <param name="centerY">Center y position of map to draw onto.</param>
		/// <param name="scale">Scale currently unused.</param>
		public void Draw (Cairo.Context cairoContext, int centerX, int centerY, double scale)
		{
			// Scale up to centimeters.
			int width = (int)(robot.Width * 100);
			int height = (int)(robot.Height * 100);

			// Set a red colour.
			cairoContext.SetSourceRGB(255, 0, 0);

			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt; 

			cairoContext.Translate (centerX + (robot.X * 100), centerY - (robot.Y * 100));
			cairoContext.Rotate (robot.Rotation); // Rotate the robot based on its orientation in radians.

			// Draw the robot as a triangle.
			cairoContext.MoveTo (0, 0);
			cairoContext.LineTo (-width / 2, height / 2);
			cairoContext.LineTo (width / 2, height / 2);
			cairoContext.LineTo (0, 0);
			cairoContext.Stroke ();

			// Reset the drawing context.
			cairoContext.Rotate (-robot.Rotation);
			cairoContext.Translate (-(centerX + robot.X), 
				-(centerY - robot.Y));
		}

		#endregion

		#region Private Event Handlers

		/// <summary>
		/// Handles the robot update event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Robot_Update (object sender, RobotUpdateEventArgs e)
		{
			// Do nothing for now.
		}

		#endregion
	}
}

