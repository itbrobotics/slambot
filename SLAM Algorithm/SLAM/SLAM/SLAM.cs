using Gtk;
using System;

namespace SLAM
{
	public class SLAM
	{
		public static void Main (String[] args)
		{
			Application.Init ();
			new MapView (new Map ());
			Application.Run ();
		}
	}
}

