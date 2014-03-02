using System;
using System.Collections.Generic;

namespace SLAM
{
	/// <summary>
	/// Map.
	/// </summary>
	public class Map
	{
		private Robot robot; // The robot roaming around on this map.
		private List<Landmark> landmarks; // All the landmarks found so far.

		// Width and height of the map area in meters.
		private double width;
		private double height;

		// Event raised whenever there is a change on the map model, any observers can 
		// choose to act upon the changes.
		public event EventHandler<MapUpdateEventArgs> MapUpdated;

		#region Public Properties

		/// <summary>
		/// Get the robot on this map.
		/// </summary>
		/// <value>The robot.</value>
		public Robot Robot
		{
			get
			{
				return robot;
			}
		}

		/// <summary>
		/// Gets or sets the landmarks on this map.
		/// </summary>
		/// <value>The landmarks.</value>
		public List<Landmark> Landmarks
		{
			get
			{
				return landmarks;
			}
			set
			{
				landmarks = value;
			}
		}

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width in meters.</value>
		public double Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height in meters.</value>
		public double Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.Map"/> class.
		/// </summary>
		/// <param name="robotModel">Robot roaming on this map.</param>
		public Map (Robot robotModel, double mapWidth, double mapHeight)
		{
			robot = robotModel;
			landmarks = new List<Landmark>();
			width = mapWidth;
			height = mapHeight;
		}

		#endregion

		#region Protected Event Handlers

		/// <summary>
		/// Raises the map update event.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnMapUpdate (MapUpdateEventArgs e)
		{
			if (MapUpdated != null)
			{
				MapUpdated (this, e);
			}
		}

		#endregion

		#region Private Event Handlers

		/// <summary>
		/// Raises the map update event.
		/// </summary>
		private void RaiseMapUpdate ()
		{
			MapUpdateEventArgs args = new MapUpdateEventArgs (this);
			OnMapUpdate (args);
		}

		#endregion
	}
}

