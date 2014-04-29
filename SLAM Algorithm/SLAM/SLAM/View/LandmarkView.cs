using System;
using Gtk;
using Cairo;
using System.Collections.ObjectModel;

namespace SLAM
{
	/// <summary>
	/// Landmark view.
	/// </summary>
	public class LandmarkView
	{
		private ReadOnlyCollection<Landmark> landmarks; // A reference to all the landmarks on the map.

		#region Public Properties

		/// <summary>
		/// Gets or sets the landmarks.
		/// </summary>
		/// <value>The landmarks.</value>
		public ReadOnlyCollection<Landmark> Landmarks
		{
			get
			{
				return landmarks;
			}
		}

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SLAM.LandmarkView"/> class.
		/// </summary>
		/// <param name="mapLandmarks">Map landmarks list.</param>
		public LandmarkView (ReadOnlyCollection<Landmark> mapLandmarks)
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
		public void Draw (Cairo.Context cairoContext, int centerX, int centerY, double scale/*, Landmark[] a*/)
		{
			cairoContext.SetSourceRGB (0, 255, 0);

			cairoContext.LineWidth = 1.0;
			cairoContext.LineCap = LineCap.Butt;

			/*for (int i = 0; i < a.Length; i++) {
				// Draw the slope.
				cairoContext.MoveTo (centerX + ((a[i].a / -a[i].b) * 100), centerY);
				cairoContext.LineTo (centerX, centerY - (a[i].b * 100));

				// Draw the line.
				//cairoContext.MoveTo (centerX - (landmark.x1y1[0] * 100), centerY - (landmark.x1y1[1] * 100));
				//cairoContext.LineTo (centerX - (landmark.x2y2[0] * 100), centerY - (landmark.x2y2[1] * 100));

				cairoContext.Stroke ();

			}*/

			foreach (Landmark landmark in landmarks)
			{

				// Draw the slope.
				//cairoContext.MoveTo (centerX + (slope * 100), centerY);
				//cairoContext.LineTo (centerX, centerY - (landmark.b * 100));

				// Draw the real line.
				double x1 = (((-centerY) / 100) - landmark.b) / landmark.a;
				double x2 = (((centerY) / 100) - landmark.b) / landmark.a;

				cairoContext.MoveTo (centerX + (-x1 * 100), centerY * 2);
				cairoContext.LineTo (centerX - (x2 * 100), 0);

				// Draw the length of the line.
				//cairoContext.MoveTo (centerX - (landmark.x1y1[0] * 100), centerY - (landmark.x1y1[1] * 100));
				//cairoContext.LineTo (centerX - (landmark.x2y2[0] * 100), centerY - (landmark.x2y2[1] * 100));

				cairoContext.Stroke ();
			} 
		}

		#endregion
	}
}

