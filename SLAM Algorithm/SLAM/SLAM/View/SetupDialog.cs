using System;
using Gtk;

namespace SLAM
{
	public class SetupDialog : Gtk.Window
	{
		private SpinButton cellSizeSpin;
		private SpinButton widthSpin;
		private SpinButton heightSpin;

		private Button createButton;

		private Entry portEntry;

		public SetupDialog () : base ("Setup Map")
		{
			cellSizeSpin = new SpinButton (0.1, 1.0, 0.1);
			cellSizeSpin.Value = 0.3;
			cellSizeSpin.Changed += CellSizeSpin_Changed;

			widthSpin = new SpinButton (3.0, 10.0, 0.3);
			widthSpin.Value = 9.9;

			heightSpin = new SpinButton (3.0, 10.0, 0.3);
			heightSpin.Value = 4.5;

			portEntry = new Entry ("/dev/tty");
			portEntry.Activated += PortEntry_Activated;

			createButton = new Button ("Create");
			createButton.Clicked += CreateButton_Clicked;

			HBox widthHBox = new HBox (false, 60);
			widthHBox.Add (new Label ("Width: "));
			widthHBox.Add (widthSpin);

			HBox heightHBox = new HBox (false, 60);
			heightHBox.Add (new Label ("Height: "));
			heightHBox.Add (heightSpin);

			HBox cellHBox = new HBox (false, 60);
			cellHBox.Add (new Label ("Cell Size: "));
			cellHBox.Add (cellSizeSpin);

			HBox portHBox = new HBox (false, 60);
			portHBox.Add (new Label ("Port: "));
			portHBox.Add (portEntry);

			HBox createHbox = new HBox (false, 0);
			createHbox.Add (createButton);

			VBox vBox = new VBox (false, 10);
			vBox.BorderWidth = 10;
			vBox.Add (widthHBox);
			vBox.Add (heightHBox);
			vBox.Add (cellHBox);
			vBox.Add (portHBox);
			vBox.Add (createHbox);

			Add (vBox);

			SetPosition (WindowPosition.Center);
			Resizable = false; 

			DeleteEvent += delegate
			{
				Application.Quit ();
			};

			Shown += OnShown;
		}

		#region Private Event Handlers

		private void OnShown (object sender, EventArgs args)
		{
			portEntry.GrabFocus ();
			portEntry.Position = portEntry.Text.Length;
		}

		private void CellSizeSpin_Changed (object sender, EventArgs args)
		{
			widthSpin.SetIncrements (cellSizeSpin.Value, cellSizeSpin.Value);
			heightSpin.SetIncrements (cellSizeSpin.Value, cellSizeSpin.Value);
		}

		private void CreateButton_Clicked (object sender, EventArgs args)
		{
			SLAM slam = new SLAM (widthSpin.Value, heightSpin.Value, cellSizeSpin.Value, portEntry.Text);
			slam.Start ();
			Destroy ();
		}

		private void PortEntry_Activated (object sender, EventArgs args)
		{
			createButton.GrabFocus ();
		}

		#endregion
	}
}

