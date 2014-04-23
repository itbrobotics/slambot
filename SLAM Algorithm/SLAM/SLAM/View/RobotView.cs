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
		private double relativeRotation; // Rotation relative to the view.
		private double lastRotation; // Last reported rotation of the robot.



		#region Public Properties

		public Robot Robot
		{
			get
			{
				return robot;
			}
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.RobotView"/> class.
		/// </summary>
		/// <param name="robotModel">Robot model that this view represents.</param>
		public RobotView (Robot robotModel)
		{
			robot = robotModel;
			relativeRotation = 0.0;
			lastRotation = -7.0;
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
			int x = (int)(robot.X * 100);
			int y = (int)(robot.Y * 100);

			// Set a red colour.
			cairoContext.SetSourceRGB(255, 0, 0);

			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt; 

			cairoContext.Translate (centerX + x, centerY - y);
			cairoContext.Rotate (relativeRotation); // Rotate the robot based on its orientation in radians.

			// Draw the robot as a triangle.
			cairoContext.MoveTo (0, -height / 2);
			cairoContext.LineTo (-width / 2, height / 2);
			cairoContext.LineTo (width / 2, height / 2);
			cairoContext.LineTo (0, -height / 2);
			cairoContext.Stroke ();

			// Reset the drawing context.
			cairoContext.Rotate (-relativeRotation);
			cairoContext.Translate (-(centerX + x), -(centerY - y));
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
			// On the first update.
			if (lastRotation == -7.0)
			{
				lastRotation = e.Robot.Heading;
			}
			else
			{
				relativeRotation += e.Robot.Heading - lastRotation;
				lastRotation = e.Robot.Heading;
			}
		}

		#endregion
	}
}

