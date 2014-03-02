using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using Driver;

public class SerialProxy
{
	private SerialPort serialPort;
	private Thread readThread;

	public event EventHandler<OdometryUpdateEventArgs> OdometryUpdated;
	public event EventHandler<ScanEventArgs> Scanned;

	#region Public Constructors

	public SerialProxy ()
	{
		// Create a new SerialPort object with default settings.
		this.serialPort = new SerialPort ("/dev/ttyACM0", 9600);
		this.serialPort.ReadTimeout = 500;
		this.serialPort.WriteTimeout = 500;
		this.serialPort.Open ();

		this.readThread = new Thread (this.Read);
		this.readThread.Start ();
	}

	#endregion

	#region Public Methods

	public void Release ()
	{
		this.readThread.Abort ();
		this.serialPort.Close ();
	}

	#endregion

	#region Private Event Handlers

	protected virtual void OnOdometryUpdate (OdometryUpdateEventArgs e)
	{
		if (this.OdometryUpdated != null)
		{
			this.OdometryUpdated (this, e);
		}
	}

	protected virtual void OnScanPerformed (ScanEventArgs e)
	{
		if (this.Scanned != null)
		{
			this.Scanned (this, e);
		}
	}

	#endregion

	#region Private Methods

	private void Read ()
	{
		while (this.serialPort.IsOpen)
		{
			try
			{
				string message = this.serialPort.ReadLine ();

				switch (message [0])
				{
				case 'o':
					// TODO: FormatException can be thrown here if one of the parameters
					// returned is garbage. IndexOutOfRangeException can also be thrown
					// if the message is corrupt.
					string[] parameters = message.Split (',');

					OdometryUpdateEventArgs args = new OdometryUpdateEventArgs (
						                               Int32.Parse (parameters [1]), 
						                               Int32.Parse (parameters [2]), 
						                               Double.Parse (parameters [3]));

					this.OnOdometryUpdate (args);
					break;
				case 's':
					// TODO: FormatException can be thrown here if one of the parameters
					// returned is garbage. IndexOutOfRangeException can also be thrown
					// if the message is corrupt.
					string[] stringReadings = message.Split(',');
					List<double> readings = new List<double> ();

					// Somewhat inefficient. Start from 1 to skip first character
					// which is the message header.
					for (int i = 1; i < stringReadings.Length; i++)
					{
						readings.Add (Double.Parse(stringReadings[i]));
					}

					ScanEventArgs args2 = new ScanEventArgs (readings);

					this.OnScanPerformed (args2);
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
		if (this.serialPort.IsOpen)
		{
			try
			{
				this.serialPort.Write(toRover);

			}
			catch (Exception) {
				// figure out something later
			}
		}
	}
	#endregion

}
	