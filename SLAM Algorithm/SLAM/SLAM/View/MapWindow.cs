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
			mapView.MapModel.MapUpdated += new EventHandler<MapUpdateEventArgs> (Map_Update);
			mapView.RobotView.Robot.RobotUpdated += new EventHandler<RobotUpdateEventArgs> (Robot_Update);

			SetPosition (WindowPosition.Center);
			Resizable = false; 

			DeleteEvent += delegate
			{
				Application.Quit ();
			};

			TextBuffer textBuffer = new TextBuffer (new TextTagTable ());

			textView = new TextView ();
			textView.Indent = 10;
			textView.Editable = false;
			textView.Buffer = textBuffer;
			textView.CursorVisible = false;
			textView.SetSizeRequest (mapView.ViewWidth, 150);

			foreach (Landmark landmark in mapView.MapModel.Landmarks)
			{
				this.textView.Buffer.Text += landmark.ToString ();
			}

			ScrolledWindow scrolledWindow = new ScrolledWindow ();
			scrolledWindow.Add (textView);

			VBox vbox = new VBox (false, 0);
			vbox.Add (this.mapView);
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
			// Do nothing for now.
		}

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

