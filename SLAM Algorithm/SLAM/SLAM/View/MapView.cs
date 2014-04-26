using System;
using Cairo;
using Gtk;

namespace SLAM
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class MapView : DrawingArea
	{
		private SlamMap mapModel; // Map model that this view represents.
		private RobotView robotView;
		private LandmarkView landmarkView;
		private PathView pathView;
		/*private EkfSlam currSlam;*/

		// Some dimensions and coordinates related to drawing the map.
		private int viewWidth;
		private int viewHeight;
		private int cellWidth;
		private int cellHeight;
		private int centerX;
		private int centerY;

		#region Public Properties

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public SlamMap MapModel
		{
			get
			{
				return mapModel;
			}
			set
			{
				mapModel = value;
			}
		}

		/// <summary>
		/// Gets or sets the robot view.
		/// </summary>
		/// <value>The robot view.</value>
		public RobotView RobotView
		{
			get
			{
				return robotView;
			}
			set
			{
				robotView = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the view.
		/// </summary>
		/// <value>The width of the view.</value>
		public int ViewWidth
		{
			get
			{
				return viewWidth;
			}
			set
			{
				viewWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of the view.
		/// </summary>
		/// <value>The height of the view.</value>
		public int ViewHeight
		{
			get
			{
				return viewHeight;
			}
			set
			{
				viewHeight = value;
			}
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.MapView"/> class.
		/// </summary>
		/// <param name="mapModel">Map model.</param>
		public MapView (SlamMap mapModel/*/, EkfSlam SlamInstance*/)
		{
			this.mapModel = mapModel;
			//currSlam = SlamInstance;
			//mapModel.Landmarks = SlamInstance.GetDB ();
			robotView = new RobotView (mapModel.Robot);
			landmarkView = new LandmarkView (mapModel.Landmarks);
			pathView = new PathView (mapModel.Robot);

			// Subscribe to events.
			mapModel.MapUpdated += new EventHandler<MapUpdateEventArgs> (Map_Update);
			robotView.Robot.RobotUpdated += new EventHandler<RobotUpdateEventArgs> (Robot_Update);
			ExposeEvent += OnExpose;
		
			// Map width and height is specified in meters, and we are using
			// 1 pixel to every centimeter, so scale up by 100.
			viewWidth = (int)(mapModel.Width * 100);
			viewHeight = (int)(mapModel.Height * 100);
			cellWidth = (int)(SlamMap.CellSize * 100);
			cellHeight = (int)(SlamMap.CellSize * 100);
			centerX = (int)(viewWidth / 2);
			centerY = (int)(viewHeight / 2);

			SetSizeRequest (viewWidth, viewHeight);
		}

		#endregion

		#region Private Event Handlers

		/// <summary>
		/// Handles the map update event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Map_Update (object sender, MapUpdateEventArgs e)
		{
			QueueDraw ();
		}

		/// <summary>
		/// Handles the robot update event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Robot_Update (object sender, RobotUpdateEventArgs e)
		{
			Console.Write (e.ToString ());
			QueueDraw ();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Raises the expose event and draws the map.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		private void OnExpose (object sender, ExposeEventArgs args)
		{
			DrawingArea area = (DrawingArea)sender;
			Cairo.Context cairoContext = Gdk.CairoHelper.Create (area.GdkWindow);

			// Draw the background.
			cairoContext.Rectangle (0, 0, viewWidth, viewHeight);
			cairoContext.SetSourceRGB(255, 255, 255);
			cairoContext.StrokePreserve ();
			cairoContext.Fill ();

			// Draw the grid.
			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt;
			cairoContext.SetSourceRGB(0, 0, 0);

			// Columns.
			int position = cellWidth;

			for (; position <= viewWidth - cellWidth; position += cellWidth)
			{
				// We have to add 0.5 to the x position otherwise Cairo will 
				// smear it across 2 pixels.
				cairoContext.MoveTo (position + 0.5, 0.0);
				cairoContext.LineTo (position + 0.5, viewHeight);
				cairoContext.Stroke ();
			}

			// Rows.
			position = cellHeight;

			for (; position <= viewHeight - cellHeight; position += cellHeight)
			{
				// We have to add 0.5 to the y position otherwise Cairo will 
				// smear it across 2 pixels.
				cairoContext.MoveTo (0.0, position + 0.5);
				cairoContext.LineTo (viewWidth, position + 0.5);
				cairoContext.Stroke ();
			}
			/*Landmark[] a = currSlam.GetDB ();*/
			robotView.Draw (cairoContext, centerX, centerY, 1.0);
			landmarkView.Draw (cairoContext, centerX, centerY, 1.0/*, a*/ );
		    
			if (robotView.Robot.PathPointList.Count > 1)
				pathView.Draw (cairoContext, centerX, centerY, 1.0);


			((IDisposable)cairoContext.GetTarget()).Dispose ();                                      
			((IDisposable)cairoContext).Dispose ();
		}

		#endregion
	}
}

