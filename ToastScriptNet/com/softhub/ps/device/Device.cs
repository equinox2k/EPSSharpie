namespace com.softhub.ps.device
{
    using ToastScriptNet;
    /// <summary>
    /// Copyright 1998 by Christian Lehner.
    /// 
    /// This file is part of ToastScript.
    /// 
    /// ToastScript is free software; you can redistribute it and/or modify
    /// it under the terms of the GNU General Public License as published by
    /// the Free Software Foundation; either version 2 of the License, or
    /// (at your option) any later version.
    /// 
    /// ToastScript is distributed in the hope that it will be useful,
    /// but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    /// GNU General Public License for more details.
    /// 
    /// You should have received a copy of the GNU General Public License
    /// along with ToastScript; if not, write to the Free Software
    /// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
    /// </summary>

    using Reusable = com.softhub.ps.graphics.Reusable;

	public interface Device
	{

		/// <summary>
		/// Initialize the device. This method is called
		/// by the initgraphics operator.
		/// </summary>
		void init();

		/// <returns> the name of the device </returns>
		string Name {get;}

		/// <summary>
		/// Save the state of the device. This method is
		/// called by the save operator. </summary>
		/// <returns> the state object </returns>
		object save();

		/// <summary>
		/// Restore the device state from the state object
		/// returned by the save method. </summary>
		/// <param name="state"> the device state </param>
		void restore(object state);

		/// <summary>
		/// Create color object. </summary>
		/// <param name="r"> the red component </param>
		/// <param name="g"> the green component </param>
		/// <param name="b"> the blue component </param>
		/// <returns> the RGB color </returns>
		Color createColor(float r, float g, float b);

		/// <summary>
		/// Create stroke object. </summary>
		/// <param name="width"> the width of the stroke </param>
		/// <param name="cap"> the cap code </param>
		/// <param name="join"> the join code </param>
		/// <param name="miter"> the miter limit </param>
		/// <param name="array"> the dash array </param>
		/// <param name="phase"> the phase of the dash </param>
		/// <returns> the stroke </returns>
		Stroke createStroke(float width, int cap, int join, float miter, float[] array, float phase);

		/// <summary>
		/// Create bitmap object. </summary>
		/// <param name="width"> the width of the bitmap </param>
		/// <param name="height"> the height of the bitmap </param>
		/// <returns> the bitmap </returns>
		Bitmap createBitmap(int width, int height);

		/// <returns> the device matrix </returns>
		AffineTransform DefaultMatrix {get;}

		/// <returns> the device resolution </returns>
		float Resolution {get;}

		/// <returns> the device scale factor </returns>
		float Scale {get;}

		/// <returns> the device dimensions in pixels </returns>
		Dimension Size {get;}

		/// <param name="color"> the current color </param>
		Color Color {set;}

		/// <param name="stroke"> the current stroke </param>
		Stroke Stroke {set;}

		/// <param name="paint"> the current paint </param>
		Paint Paint {set;}

		/// <param name="font"> the current font </param>
		FontInfo Font {set;}

		/// <param name="obj"> the cached character representation </param>
		/// <param name="xform"> the transformation matrix </param>
		void show(Reusable obj, AffineTransform xform);

		/// <param name="bitmap"> the image representation </param>
		/// <param name="xform"> the transformation matrix </param>
		void image(Bitmap bitmap, AffineTransform xform);

		/// <param name="shape"> the shape to fill </param>
		void fill(Shape shape);

		/// <param name="shape"> the shape to stroke </param>
		void stroke(Shape shape);

		/// <summary>
		/// Initialize the clip rectangle to the device
		/// boundaries.
		/// </summary>
		void initclip();

		/// <param name="shape"> the shape to clip to </param>
		void clip(Shape shape);

		/// <returns> the current clip shape </returns>
		Shape clippath();

		/// <summary>
		/// Show the current page and erase it.
		/// </summary>
		void showpage();

		/// <summary>
		/// Copy and show the current page.
		/// </summary>
		void copypage();

		/// <summary>
		/// Erase the current page.
		/// </summary>
		void erasepage();

		/// <summary>
		/// Called by jobserver to indicate that
		/// a save/restore encapsulated job begins.
		/// </summary>
		void beginJob();

		/// <summary>
		/// Called by jobserver to indicate that
		/// the current job is done.
		/// </summary>
		void endJob();

		/// <summary>
		/// Error in job. </summary>
		/// <param name="msg"> the error message </param>
		void error(string msg);

		/// <summary>
		/// Convert device property. </summary>
		/// <param name="name"> the name of the property </param>
		/// <param name="value"> the value of the property </param>
		object convertType(string name, object value);

	}

}