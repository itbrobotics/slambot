using System;
using System.Collections.Generic;
using Gtk;
using Cairo;

namespace SLAM
{
	/// <summary>
	/// Landmark view.
	/// </summary>
	public class LandmarkView
	{
		private List<Landmark> landmarks; // A reference to all the landmarks on the map.

		#region Public Properties

		/// <summary>
		/// Gets or sets the landmarks.
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

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.LandmarkView"/> class.
		/// </summary>
		/// <param name="mapLandmarks">Map landmarks array.</param>
		public LandmarkView (Landmark[] mapLandmarks)
		{
			landmarks = new List<Landmark> ();
			landmarks.AddRange (mapLandmarks);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.LandmarkView"/> class.
		/// </summary>
		/// <param name="mapLandmarks">Map landmarks list.</param>
		public LandmarkView (List<Landmark> mapLandmarks)
		{
			landmarks = mapLandmarks;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Draw every landmark contained within this class.
		/// </summary>
		/// <param name="cairoContext">Cairo context.</param>
		/// <param name="centerX">Center x.</param>
		/// <param name="centerY">Center y.</param>
		/// <param name="scale">Scale currently unused.</param>
		public void Draw (Cairo.Context cairoContext, int centerX, int centerY, double scale)
		{
			cairoContext.SetSourceRGB (0, 255, 0);

			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt;

			foreach (Landmark landmark in landmarks)
			{
				cairoContext.MoveTo (centerX + ((landmark.b / -landmark.a) * 100), centerY);
				cairoContext.LineTo (centerX, centerY - (landmark.b * 100));
				cairoContext.Stroke ();
			} 
		}

		#endregion
	}
}

