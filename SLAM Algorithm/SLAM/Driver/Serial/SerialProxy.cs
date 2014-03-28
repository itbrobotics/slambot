using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using Driver;

/// <summary>
/// Serial proxy, singleton class.
/// </summary>
public class SerialProxy
{
	// Singleton instance.
	private static SerialProxy proxy = new SerialProxy ();
	private SerialPort serialPort;
	private Thread readThread;

	public event EventHandler<OdometryUpdateEventArgs> OdometryUpdated;
	public event EventHandler<ScanEventArgs> Scanned;

	#region Public Properties

	public static SerialProxy GetInstance
	{
		get
		{
			return proxy;
		}
	}

	#endregion

	#region Private Constructors

	private SerialProxy ()
	{
		try
		{
			// Create a new SerialPort object with default settings.
			// Assuming that serial port will always be /dev/ttyACM0 here!
			serialPort = new SerialPort ("/dev/ttyACM0", 9600);
			serialPort.ReadTimeout = 500;
			serialPort.WriteTimeout = 500;
			serialPort.Open ();
		}
		catch (IOException)
		{
			Console.WriteLine ("Cannot open port: " + serialPort.PortName);
		}
	}

	#endregion

	#region Public Methods

	public void Start ()
	{
		readThread = new Thread (this.Read);
		readThread.Start ();
	}

	public void Release ()
	{
		readThread.Abort ();
		serialPort.Close ();
	}

	public void Send (char[] commands)
	{
		if (serialPort.IsOpen)
		{
			try
			{
				serialPort.Write (commands, 0, commands.Length);
			}
			catch (IOException)
			{
				Console.WriteLine ("IOException on Send!");
			}
		}
		else
		{
			Console.WriteLine ("SerialProxy: No serial port open.");
		}
	}

	#endregion

	#region Protected Event Handlers

	protected virtual void OnOdometryUpdate (OdometryUpdateEventArgs e)
	{
		if (OdometryUpdated != null)
		{
			OdometryUpdated (this, e);
		}
	}

	protected virtual void OnScanPerformed (ScanEventArgs e)
	{
		if (Scanned != null)
		{
			Scanned (this, e);
		}
	}

	#endregion

	#region Private Methods

	private void Read ()
	{
		while (serialPort.IsOpen)
		{
			try
			{
				string message = serialPort.ReadLine ();

				switch (message [0])
				{
				case 'o':
					string[] parameters = message.Split (',');

					OdometryUpdateEventArgs args = new OdometryUpdateEventArgs (
						                               Int32.Parse (parameters [1]), 
						                               Int32.Parse (parameters [2]), 
						                               Double.Parse (parameters [3]));

					this.OnOdometryUpdate (args);
					break;
				case 's':
					string[] stringReadings = message.Split (',');
					List<double> readings = new List<double> ();

					// Somewhat inefficient. Start from 1 to skip first character
					// which is the message header.
					for (int i = 1; i < stringReadings.Length; i++)
					{
						readings.Add (Double.Parse (stringReadings [i]));
					}

					ScanEventArgs args2 = new ScanEventArgs (readings);

					OnScanPerformed (args2);
					break;
				default:
					break;
				}	
			}
			catch (TimeoutException)
			{ 
				// Do nothing.
			}
			catch (FormatException)
			{
				// Do nothing for now.
			}
			catch (IndexOutOfRangeException)
			{
				// Do nothing for now.
			}
		}
	}
	// note not yet tested, get access to arduino
	private void Write (String toRover)
	{
		if (serialPort.IsOpen)
		{
			try
			{
				serialPort.Write (toRover);

			}
			catch (Exception)
			{
				// figure out something later
			}
		}
	}

	#endregion

}
	