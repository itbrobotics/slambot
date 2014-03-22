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

			SerialProxy proxy = SerialProxy.GetInstance;
			Observer observer = new Observer (proxy);

			Console.WriteLine ("Type QUIT to exit");

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
