using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SLAM
{
	/// <summary>
	/// Map.
	/// </summary>
	public class SlamMap
	{
		// Cell size for the map, this could be a public property but for
		// simplicity it is like this for now.
		public const double CellSize = 0.5; // Meters, cells are square i.e. 0.5 x 0.5.

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

		public ReadOnlyCollection<Landmark> Landmarks
		{
			get
			{
				return landmarks.AsReadOnly ();
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
		public SlamMap (Robot robotModel, double mapWidth, double mapHeight)
		{
			robot = robotModel;
			landmarks = new List<Landmark>();
			width = mapWidth;
			height = mapHeight;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a single landmark to the map.
		/// </summary>
		/// <param name="landmark">Landmark.</param>
		public void AddLandmark (Landmark landmark)
		{
			landmarks.Add (landmark);
			RaiseMapUpdate ();
		}

		/// <summary>
		/// Adds a range of landmarks to the map.
		/// </summary>
		/// <param name="landmarkArray">Landmark array.</param>
		public void AddLandmarks (Landmark[] landmarkArray)
		{
			landmarks.AddRange (landmarkArray);
			RaiseMapUpdate ();
		}

		/// <summary>
		/// Adds a range of landmarks to the map.
		/// </summary>
		/// <param name="landmarkList">Landmark list.</param>
		public void AddLandmarks (List<Landmark> landmarkList)
		{
			landmarks.AddRange (landmarkList);
			RaiseMapUpdate ();
		}

		/// <summary>
		/// Updates the landmarks.
		/// </summary>
		/// <param name="landmarkArray">Landmark array.</param>
		public void UpdateLandmarks (Landmark[] landmarkArray)
		{
			landmarks.Clear ();
			landmarks.AddRange (landmarkArray);
			RaiseMapUpdate ();
		}

		/// <summary>
		/// Updates the landmarks.
		/// </summary>
		/// <param name="landmarkList">Landmark list.</param>
		public void UpdateLandmarks (List<Landmark> landmarkList)
		{
			landmarks.Clear ();
			landmarks.AddRange (landmarkList);
			RaiseMapUpdate ();
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

