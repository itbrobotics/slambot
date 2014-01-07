using Gtk;
using Cairo;
using System;
using System.Collections.Generic;

namespace SLAM
{
	public class MapView : Window
	{
		private const int MapWidth = 600;
		private const int MapHeight = 500;
		private const int TrueX = 0;
		private const int TrueY = 0;
		private const int CenterX = MapWidth / 2;
		private const int CenterY = MapHeight / 2;
		private const int RobotWidth = 18;
		private const int RobotHeight = 23;
		private const int WheelWidth = 3;
		private const int WheelHeight = 6;

		private Map slamMap;

		private TextView textView;

		/************************************************************
		 * Public Properties
		 ***********************************************************/

		public Map SlamMap
		{
			get
			{
				return this.slamMap;
			}
			set
			{
				this.slamMap = value;
			}
		}

		/************************************************************
		 * Public Constructors
		 ***********************************************************/

		public MapView (Map map) : base("Map")
		{
			this.slamMap = map;

			this.SetDefaultSize (MapWidth, MapHeight + 150);
			this.SetPosition (WindowPosition.Center);
			//this.Resizable = false; Figure out how to disable maximizing/minimizing.

			this.DeleteEvent += delegate
			{
				Application.Quit ();
			};

			DrawingArea drawingArea = new DrawingArea ();
			drawingArea.ExposeEvent += OnExpose;
			drawingArea.SetSizeRequest (MapWidth, MapHeight - 130);

			TextBuffer textBuffer = new TextBuffer (new TextTagTable ());

			this.textView = new TextView ();
			this.textView.Editable = false;
			this.textView.Buffer = textBuffer;
			this.textView.CursorVisible = false;
			this.textView.Indent = 10;

			foreach (Landmark landmark in slamMap.SlamLandmarks)
			{
				this.textView.Buffer.Text += landmark.ToString ();
			}

			ScrolledWindow scrolledWindow = new ScrolledWindow ();
			scrolledWindow.Add (this.textView);

			VBox vbox = new VBox (false, 0);
			vbox.Add (drawingArea);
			vbox.Add (scrolledWindow);

			this.Add (vbox);
			this.ShowAll ();
		}

		/************************************************************
		 * Private Methods
		 ***********************************************************/	

		private void OnExpose (object sender, ExposeEventArgs args)
		{
			DrawingArea area = (DrawingArea)sender;
			Cairo.Context cairoContext = Gdk.CairoHelper.Create (area.GdkWindow);

			// Draw the Map.
			this.DrawBackground (cairoContext);
			this.DrawGrid (cairoContext);
			this.DrawRobot (cairoContext, this.slamMap.RobotPosition);

			// Don't bother attempting to draw landmarks if there
			// are none.
			if (this.slamMap.SlamLandmarks.Count != 0)
			{
				this.DrawLandmarks (cairoContext, this.slamMap.SlamLandmarks);
			}

			((IDisposable)cairoContext.Target).Dispose ();                                      
			((IDisposable)cairoContext).Dispose ();
		}

		private void DrawBackground (Cairo.Context cairoContext)
		{
			Color backgroundColor = new Color (255, 255, 255);

			// Draw the background.
			cairoContext.Rectangle (TrueX, TrueY, MapWidth, MapHeight);
			cairoContext.Color = backgroundColor;
			cairoContext.StrokePreserve ();
			cairoContext.Fill ();
		}

		private void DrawGrid (Cairo.Context cairoContext)
		{
			Color gridColor = new Color (0, 0, 0);

			// Draw the grid Axis.
			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt;

			int width = MapWidth; 
			int height = MapHeight;

			// Y axis.
			cairoContext.Color = gridColor;
			cairoContext.MoveTo (width / 2, 0.0);
			cairoContext.LineTo (width / 2, height);
			cairoContext.Stroke ();

			// TODO: These for loops aren't very intelligent, 
			// they break based on the scaling.
			for (int i = 50; i < height; i += 100)
			{
				if (i == height / 2)
				{
					continue;
				}

				cairoContext.MoveTo (width / 2, i);
				cairoContext.LineTo (width / 2 - 20.0, i);
				cairoContext.Stroke ();
			}

			for (int i = 100; i < height; i += 100)
			{
				if (i == height / 2)
				{
					continue;
				}

				cairoContext.MoveTo (width / 2, i);
				cairoContext.LineTo (width / 2 - 10.0, i);
				cairoContext.Stroke ();
			}

			// X axis.
			cairoContext.MoveTo (0.0, height / 2);
			cairoContext.LineTo (width, height / 2);
			cairoContext.Stroke ();

			// TODO: These for loops aren't very intelligent, 
			// they break based on the scaling.
			for (int i = 100; i < width; i += 100)
			{
				if (i == width / 2)
				{
					continue;
				}

				cairoContext.MoveTo (i, height / 2);
				cairoContext.LineTo (i, height / 2 + 20.0);
				cairoContext.Stroke ();
			}

			for (int i = 50; i < width; i += 100)
			{
				if (i == width / 2)
				{
					continue;
				}

				cairoContext.MoveTo (i, height / 2);
				cairoContext.LineTo (i, height / 2 + 10.0);
				cairoContext.Stroke ();
			}
		}

		private void DrawRobot (Cairo.Context cairoContext, double[] robotPosition)
		{
			Color robotColor = new Color (255, 0, 0);
			Color wheelColor = new Color (0, 0, 0);

			cairoContext.Color = robotColor;
			cairoContext.Translate (CenterX + robotPosition [0], CenterY - robotPosition [1]);
			cairoContext.Rotate (robotPosition [2]); // Rotate the robot based on its orientation in radians.

			// Draw the body.
			cairoContext.Rectangle (-(RobotWidth / 2), -(RobotHeight / 2), 
			                        RobotWidth, RobotHeight);
			cairoContext.StrokePreserve ();
			cairoContext.Fill ();

			cairoContext.Color = wheelColor;

			// Front indicator.
			cairoContext.Rectangle (-((RobotWidth / 2) - 8.0), -((RobotHeight / 2) - 5.0), 
			                        WheelWidth, WheelHeight);

			// Top left wheel.
			cairoContext.Rectangle (-((RobotWidth / 2) + 4.0), -(RobotHeight / 2), 
			                        WheelWidth, WheelHeight);

			// Top right wheel.
			cairoContext.Rectangle (((RobotWidth / 2) + 1.0), -(RobotHeight / 2), 
			                        WheelWidth, WheelHeight);

			// Bottom left wheel.
			cairoContext.Rectangle (-((RobotWidth / 2) + 4.0), -((RobotHeight / 2) - 17.0), 
			                        WheelWidth, WheelHeight);

			// Bottom right wheel.
			cairoContext.Rectangle (((RobotWidth / 2) + 1.0), -((RobotHeight / 2) - 17.0), 
			                        WheelWidth, WheelHeight);

			cairoContext.StrokePreserve ();
			cairoContext.Fill ();

			// Reset the drawing context.
			cairoContext.Rotate (-robotPosition [2]);
			cairoContext.Translate (-(CenterX + robotPosition [0]), -(CenterY - robotPosition [1]));
		}

		private void DrawLandmarks (Cairo.Context cairoContext, List<Landmark> landmarks)
		{
			// Draw the actual point of the landmark.
			Color landmarkColor = new Color (0, 0, 255);

			foreach (Landmark landmark in landmarks)
			{
				// From our virtual center move to the landmark's position.
				// Also scale up landmark positions from metres to centimetres.
				cairoContext.Translate (CenterX + (landmark.pos [0] * 100), 
				                        CenterY - (landmark.pos [1] * 100));

				cairoContext.Color = landmarkColor;
				cairoContext.Arc (0, 0, 15, 0, 2 * Math.PI);
				cairoContext.StrokePreserve ();
				cairoContext.Fill ();

				// Return to our virtual center after drawing the landmark.
				cairoContext.Translate (-(CenterX + (landmark.pos [0] * 100)), 
				                        -(CenterY - (landmark.pos [1] * 100)));
			}

			// Draw the landmark id in a seperate loop to avoid any weird
			// tearing behaviour.
			Color textColor = new Color (255, 255, 255);
			cairoContext.Color = textColor;

			cairoContext.SelectFontFace ("Sans Serif", FontSlant.Normal, FontWeight.Normal);
			cairoContext.SetFontSize (12);

			foreach (Landmark landmark in landmarks)
			{
				// Center the text inside the landmark.
				TextExtents textExtents = cairoContext.TextExtents (landmark.id.ToString ());

				cairoContext.MoveTo (CenterX + (landmark.pos [0] * 100) - (textExtents.Width / 2), 
				                     CenterY - (landmark.pos [1] * 100) + (textExtents.Height / 2));

				cairoContext.ShowText (landmark.id.ToString ());
			}

			// Draw the equation of a line for each landmark.
			Color lineColor = new Color (0, 255, 0);
			cairoContext.Color = lineColor;

			foreach (Landmark landmark in landmarks)
			{
				cairoContext.MoveTo (CenterX + ((landmark.b / -landmark.a) * 100), CenterY);
				cairoContext.LineTo (CenterX, CenterY - (landmark.b * 100));
				cairoContext.Stroke ();

				Console.WriteLine ("X= " +  ((landmark.b / -landmark.a) * 100));
				Console.WriteLine ("Y= " + (landmark.b * 100));
			}
		}
	}
}

