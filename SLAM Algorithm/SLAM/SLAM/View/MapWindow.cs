using Gtk;
using Cairo;
using System;
using System.Collections.Generic;

namespace SLAM
{
	/// <summary>
	/// Map window.
	/// </summary>
	public class MapWindow : Window
	{
		DrawingArea drawingArea;

		private MapView mapView;
		private TextView textView; // Textview to hold landmark information.

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.MapWindow"/> class.
		/// </summary>
		/// <param name="mapView">Map view contained in this window.</param>
		public MapWindow (MapView mapView) : base("Map")
		{
			this.mapView = mapView;

			// Subscribe to events.
			mapView.Map.MapUpdated += new EventHandler<MapUpdateEventArgs> (Map_Update);
			mapView.RobotView.Robot.RobotUpdated += new EventHandler<RobotUpdateEventArgs> (Robot_Update);

			SetPosition (WindowPosition.Center);
			Resizable = false; 

			DeleteEvent += delegate
			{
				Application.Quit ();
			};

			drawingArea = new DrawingArea ();
			drawingArea.SetSizeRequest (mapView.ViewWidth, mapView.ViewHeight);
			drawingArea.ExposeEvent += OnExpose;
			drawingArea.SetSizeRequest (mapView.ViewWidth, mapView.ViewHeight);

			TextBuffer textBuffer = new TextBuffer (new TextTagTable ());

			textView = new TextView ();
			textView.Editable = false;
			textView.Buffer = textBuffer;
			textView.CursorVisible = false;
			textView.Indent = 10;

			foreach (Landmark landmark in mapView.Map.CopyLandmarks ())
			{
				this.textView.Buffer.Text += landmark.ToString ();
			}

			ScrolledWindow scrolledWindow = new ScrolledWindow ();
			scrolledWindow.Add (textView);

			VBox vbox = new VBox (false, 0);
			vbox.Add (drawingArea);
			vbox.Add (scrolledWindow);

			Add (vbox);
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
			// Redraw the map.
			drawingArea.QueueDraw ();
		}

		/// <summary>
		/// Handles the robot update event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Robot_Update (object sender, RobotUpdateEventArgs e)
		{
			// Redraw the map.
			drawingArea.QueueDraw ();
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

			// Draw the Map.
			mapView.Draw (cairoContext, 0, 0);

			//this.DrawGrid (cairoContext);

			((IDisposable)cairoContext.GetTarget()).Dispose ();                                      
			((IDisposable)cairoContext).Dispose ();
		}

		#endregion

//		private void DrawGrid (Cairo.Context cairoContext)
//		{
//			// Draw the grid Axis.
//			cairoContext.LineWidth = 1.0;
//			cairoContext.LineCap = LineCap.Butt;
//
//			int width = MapWidth; 
//			int height = MapHeight;
//
//			// Y axis.
//			cairoContext.SetSourceRGB(0, 0, 0);
//			cairoContext.MoveTo (width / 2, 0.0);
//			cairoContext.LineTo (width / 2, height);
//			cairoContext.Stroke ();
//
//			// TODO: These for loops aren't very intelligent, 
//			// they break based on the scaling.
//			for (int i = 50; i < height; i += 100)
//			{
//				if (i == height / 2)
//				{
//					continue;
//				}
//
//				cairoContext.MoveTo (width / 2, i);
//				cairoContext.LineTo (width / 2 - 20.0, i);
//				cairoContext.Stroke ();
//			}
//
//			for (int i = 100; i < height; i += 100)
//			{
//				if (i == height / 2)
//				{
//					continue;
//				}
//
//				cairoContext.MoveTo (width / 2, i);
//				cairoContext.LineTo (width / 2 - 10.0, i);
//				cairoContext.Stroke ();
//			}
//
//			// X axis.
//			cairoContext.MoveTo (0.0, height / 2);
//			cairoContext.LineTo (width, height / 2);
//			cairoContext.Stroke ();
//
//			// TODO: These for loops aren't very intelligent, 
//			// they break based on the scaling.
//			for (int i = 100; i < width; i += 100)
//			{
//				if (i == width / 2)
//				{
//					continue;
//				}
//
//				cairoContext.MoveTo (i, height / 2);
//				cairoContext.LineTo (i, height / 2 + 20.0);
//				cairoContext.Stroke ();
//			}
//
//			for (int i = 50; i < width; i += 100)
//			{
//				if (i == width / 2)
//				{
//					continue;
//				}
//
//				cairoContext.MoveTo (i, height / 2);
//				cairoContext.LineTo (i, height / 2 + 10.0);
//				cairoContext.Stroke ();
//			}
//		}
	}
}

