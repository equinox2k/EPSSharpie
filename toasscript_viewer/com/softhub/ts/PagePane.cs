using System;

namespace com.softhub.ts
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


	public class PagePane : JPanel
	{

		private BorderLayout layout = new BorderLayout();
		private PageBorder border = new PageBorder();
		private PageCanvas canvas = new PageCanvas();
		private float width, height;

		public PagePane()
		{
			try
			{
				jbInit();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void jbInit() throws Exception
		private void jbInit()
		{
			Layout = layout;
			border.BackgroundColor = Color.lightGray;
			Border = border;
			add(canvas, BorderLayout.CENTER);
		}

		public virtual bool updatePage(Rectangle viewBounds)
		{
			Rectangle bounds = Bounds;
			Rectangle r = viewBounds.intersection(bounds);
			int areaA = r.width * r.height;
			int areaB = viewBounds.width * viewBounds.height;
			canvas.activate(areaA > 0);
			return areaA * 2 > areaB;
		}

		public virtual void updatePageSize(float width, float height, float scale)
		{
			this.width = width;
			this.height = height;
			Insets insets = Insets;
			float dpi = Toolkit.ScreenResolution;
			float factor = scale * dpi / 72;
			int w = (int)Math.Round(width * factor, MidpointRounding.AwayFromZero) + insets.left + insets.right;
			int h = (int)Math.Round(height * factor, MidpointRounding.AwayFromZero) + insets.top + insets.bottom;
			setSize(w, h);
			canvas.Scale = scale;
		}

		public virtual float PageWidth
		{
			get
			{
				return width;
			}
		}

		public virtual float PageHeight
		{
			get
			{
				return height;
			}
		}

		public virtual void updatePageScale(float scale)
		{
			updatePageSize(width, height, scale);
		}

		public virtual PageCanvas PageCanvas
		{
			get
			{
				return canvas;
			}
		}

	}

}