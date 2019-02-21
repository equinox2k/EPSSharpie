namespace com.softhub.ps.device
{
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

	using Drawable = com.softhub.ps.graphics.Drawable;

	public interface PageDevice : Device
	{

		/// <summary>
		/// Set the page size. </summary>
		/// <param name="w"> the width in 1/72 of an inch </param>
		/// <param name="h"> the height in 1/72 of an inch </param>
		float[] PageSize {set;get;}


		/// <returns> the page width in 1/72 inch </returns>
		float PageWidth {get;}

		/// <returns> the page height in 1/72 inch </returns>
		float PageHeight {get;}

		/// <summary>
		/// Set the page orientation. </summary>
		/// <param name="mode"> [0..3] </param>
		int Orientation {set;get;}


		/// <summary>
		/// Set the scale factor. </summary>
		/// <param name="the"> absolute scale factor </param>
		float Scale {set;get;}


		/// <summary>
		/// Set the number of copies for showpage to print. </summary>
		/// <param name="the"> number of copies </param>
		int NumCopies {set;get;}


		/// <returns> the current page </returns>
		Drawable Content {get;}

		/// <summary>
		/// Add a page event listener. </summary>
		/// <param name="the"> listener to add </param>
		void addPageEventListener(PageEventListener listener);

		/// <summary>
		/// Remove a page event listener. </summary>
		/// <param name="the"> listener to remove </param>
		void removePageEventListener(PageEventListener listener);

	}

}