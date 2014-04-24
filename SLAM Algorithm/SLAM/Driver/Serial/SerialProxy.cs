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

	public string Port
	{
		get
		{
			return serialPort.PortName;
		}
		set
		{
			serialPort.PortName = value;
		}
	}

	public int BaudRate
	{
		get
		{
			return serialPort.BaudRate;
		}
		set
		{
			serialPort.BaudRate = value;
		}
	}

	public bool IsOpen
	{
		get
		{
			return serialPort.IsOpen;
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
			serialPort.ReadTimeout = 1000;
			serialPort.WriteTimeout = 1000;
		}
		catch (IOException ex)
		{
			Console.WriteLine (ex.ToString ());
		}
	}

	#endregion

	#region Public Methods

	public void Start ()
	{
		try
		{
			serialPort.Open ();

			readThread = new Thread (this.Read);
			readThread.Start ();
		}
		catch (IOException ex)
		{
			Console.WriteLine (ex.ToString ());
		}
	}

	public void Release ()
	{
		serialPort.Close ();
		readThread.Abort ();
	}

	public void Send (char[] commands)
	{
		if (serialPort.IsOpen)
		{
			try
			{
				serialPort.Write (commands, 0, commands.Length);
			}
			catch (IOException ex)
			{
				Console.WriteLine (ex.StackTrace);
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

					readings.Reverse ();

					ScanEventArgs args2 = new ScanEventArgs (readings);

					OnScanPerformed (args2);
					break;
				default:
					break;
				}	
			}
			catch (TimeoutException ex)
			{ 
				Console.WriteLine (ex.ToString ());
			}
			catch (FormatException ex)
			{
				Console.WriteLine (ex.ToString ());
			}
			catch (IndexOutOfRangeException ex)
			{
				Console.WriteLine (ex.ToString ());
			}
			catch (ThreadAbortException)
			{
				// Ignore it.
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.ToString ());
			}
		}
	}

	#endregion

}
	
