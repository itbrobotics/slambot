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
		private Robot robot;

		private MapView mapView;
		private TextView textView; // Textview to hold landmark information.
		private Entry commandEntry;
		private Button sendButton;

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.MapWindow"/> class.
		/// </summary>
		/// <param name="mapView">Map view contained in this window.</param>
		public MapWindow (MapView mapView) : base("Map")
		{
			robot = mapView.RobotView.Robot;

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

			commandEntry = new Entry ();
			commandEntry.Activated += CommandEntry_OnActivated;

			sendButton = new Button ("Send");
			sendButton.Clicked += SendButton_OnClick;

			HBox hbox = new HBox (false, 0);
			hbox.Add (commandEntry);
			hbox.Add (sendButton);

			VBox vbox = new VBox (false, 0);
			vbox.Add (this.mapView);
			vbox.Add (scrolledWindow);
			vbox.Add (hbox);

			Add (vbox);
			Shown += OnShown;
		}

		#endregion

		#region Private Event Handlers

		private void OnShown (object sender, EventArgs args)
		{
			commandEntry.GrabFocus ();
		}

		private void SendButton_OnClick (object sender, EventArgs args)
		{
			if (commandEntry.Text.Length == 0)
				return;

			// Get the only character in lower case.
			char command = commandEntry.Text.ToLower () [0];

			switch (command)
			{
			case 'w':
				robot.GoFoward ();
				break;
			case 's':
				robot.GoBackward ();
				break;
			case 'a':
				robot.RotateLeft ();
				break;
			case 'd':
				robot.RotateRight ();
				break;
			case 'q':
				robot.Halt ();
				break;
			case 'e':
				robot.Scan ();
				break;
			default:
				// Ignore it for now.
				break;
			}

			commandEntry.DeleteText (0, commandEntry.Text.Length);
		}

		private void CommandEntry_OnActivated (object sender, EventArgs e)
		{
			SendButton_OnClick (sender, e);
		}

		/// <summary>
		/// Handles the map update event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Map_Update (object sender, MapUpdateEventArgs e)
		{
			textView.Buffer.Clear ();

			foreach (Landmark landmark in mapView.MapModel.Landmarks)
			{
				this.textView.Buffer.Text += landmark.ToString ();
			}
		}

		/// <summary>
		/// Handles the robot update event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Robot_Update (object sender, RobotUpdateEventArgs e)
		{
//			if (IsRealized)
//			{
//				textView.Buffer.Text += "Robot: x = " + e.Robot.X +
//				", y = " + e.Robot.Y +
//				", rotation = " + e.Robot.Rotation +
//				"\n";
//			}
		}

		#endregion

	}
}

