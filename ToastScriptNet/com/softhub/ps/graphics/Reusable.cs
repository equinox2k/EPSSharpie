namespace com.softhub.ps.graphics
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

    using CharWidth = com.softhub.ps.util.CharWidth;

	public interface Reusable
	{

		/// <summary>
		/// Draw the object. </summary>
		/// <param name="g"> the graphics context </param>
		void draw(Graphics2D g);

		/// <summary>
		/// Normalize the object. </summary>
		/// <param name="xform"> a transformation matrix </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void normalize(java.awt.geom.AffineTransform xform) throws java.awt.geom.NoninvertibleTransformException;
		void normalize(AffineTransform xform);

		/// <returns> true if this object is resolution dependent </returns>
		bool ResolutionDependent {get;}

		/// <returns> the bounding box </returns>
		Rectangle2D Bounds2D {get;}

		/// <returns> the character code or null if not a character </returns>
		string CharCode {get;}

		/// <returns> the character width or null if not a character </returns>
		CharWidth CharWidth {get;}

	}

}