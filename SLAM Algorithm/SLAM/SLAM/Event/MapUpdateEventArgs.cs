using System;

namespace SLAM
{
	/// <summary>
	/// An event to handle any updates to the map.
	/// </summary>
	public class MapUpdateEventArgs : EventArgs
	{
		private SlamMap map;

		#region Public Properties

		/// <summary>
		/// Gets the map.
		/// </summary>
		/// <value>The map.</value>
		public SlamMap Map
		{
			get
			{
				return map;
			}
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.MapUpdateEventArgs"/> class.
		/// </summary>
		/// <param name="updatedMap">Updated map.</param>
		public MapUpdateEventArgs (SlamMap updatedMap)
		{
			map = updatedMap;
		}

		#endregion
	}
}

