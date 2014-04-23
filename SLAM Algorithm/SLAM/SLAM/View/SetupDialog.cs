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

		public SetupDialog () : base ("Setup Map")
		{
			cellSizeSpin = new SpinButton (0.1, 1.0, 0.1);
			cellSizeSpin.Value = 0.3;
			cellSizeSpin.Changed += CellSizeSpin_Changed;

			widthSpin = new SpinButton (3.0, 10.0, 0.3);
			heightSpin = new SpinButton (3.0, 10.0, 0.3);

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

			HBox createHbox = new HBox (false, 0);
			createHbox.Add (createButton);

			VBox vBox = new VBox (false, 10);
			vBox.Add (widthHBox);
			vBox.Add (heightHBox);
			vBox.Add (cellHBox);
			vBox.Add (createHbox);

			Add (vBox);

			SetPosition (WindowPosition.Center);
			Resizable = false; 

			DeleteEvent += delegate
			{
				Application.Quit ();
			};
		}

		#region Private Event Handlers

		private void CellSizeSpin_Changed (object sender, EventArgs args)
		{
			widthSpin.SetIncrements (cellSizeSpin.Value, cellSizeSpin.Value);
			heightSpin.SetIncrements (cellSizeSpin.Value, cellSizeSpin.Value);
		}

		private void CreateButton_Clicked (object sender, EventArgs args)
		{
			SLAM slam = new SLAM (widthSpin.Value, heightSpin.Value);
			slam.Start ();
			Destroy ();
		}

		#endregion
	}
}

