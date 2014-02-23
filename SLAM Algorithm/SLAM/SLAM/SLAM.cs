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

//			bool continueCommunication = true;
//			string message;
//			StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
//
//			SerialProxy proxy = new SerialProxy ();
//			Observer observer = new Observer (proxy);
//
//			Console.WriteLine ("Type QUIT to exit");
//
//			while (continueCommunication)
//			{
//				message = Console.ReadLine ();
//
//				if (stringComparer.Equals ("quit", message))
//				{
//					continueCommunication = false;
//				}
//			}
//
//			proxy.Release ();
		}
	}
}

