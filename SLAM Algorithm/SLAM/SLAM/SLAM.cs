using Gtk;
using System;

namespace SLAM
{
	public class SLAM
	{
		public static void Main (String[] args)
		{
			Application.Init ();
			SlamController controller = new SlamController ();
			//controller.StartSimulation ();
			Application.Run ();
			//controller.StopSimulation ();
		}
	}
}

