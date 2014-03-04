using System;
using Driver;
using System.Threading;

namespace Driver
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool continueCommunication = true;
			string message;
			StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

			SerialProxy proxy = new SerialProxy ();
			Observer observer = new Observer (proxy);

			Console.WriteLine ("Type QUIT to exit");
		
			Thread.Sleep (2000);
			proxy.GoForward ();

			while (continueCommunication)
			{
				message = Console.ReadLine ();

				if (stringComparer.Equals ("quit", message))
				{
					continueCommunication = false;
				}
			}

			proxy.Release ();
		}
	}
}
