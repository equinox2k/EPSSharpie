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


	public class PageBorder : AbstractBorder
	{

		private int shadowWidth = 4;
		private Color shadowColor = Color.black;
		private Color backgroundColor = Color.white;

		public PageBorder()
		{
		}

		public virtual int ShadowWidth
		{
			set
			{
				this.shadowWidth = value;
			}
			get
			{
				return shadowWidth;
			}
		}


		public virtual Color ShadowColor
		{
			set
			{
				this.shadowColor = value;
			}
			get
			{
				return shadowColor;
			}
		}


		public virtual Color BackgroundColor
		{
			set
			{
				this.backgroundColor = value;
			}
			get
			{
				return backgroundColor;
			}
		}


		public virtual void paintBorder(Component c, Graphics g, int x, int y, int width, int height)
		{
			Color currentColor = g.Color;
			int rx = x + width - shadowWidth;
			int ry = y + height - shadowWidth;
			g.Color = backgroundColor;
			g.fillRect(rx, y, shadowWidth, shadowWidth);
			g.fillRect(0, ry, shadowWidth, shadowWidth);
			g.Color = shadowColor;
			g.drawLine(x, 0, rx, 0);
			g.drawLine(0, y, 0, ry);
			g.drawLine(rx, y, rx, y + shadowWidth);
			g.drawLine(0, ry, shadowWidth, ry);
			g.fillRect(rx, shadowWidth, shadowWidth, height);
			g.fillRect(shadowWidth, ry, width, shadowWidth);
			g.Color = currentColor;
		}

		public virtual Insets getBorderInsets(Component c)
		{
			return new Insets(1, 1, shadowWidth + 1, shadowWidth + 1);
		}

		public virtual Insets getBorderInsets(Component c, Insets insets)
		{
			insets.left = 1;
			insets.top = 1;
			insets.right = shadowWidth + 1;
			insets.bottom = shadowWidth + 1;
			return insets;
		}

	}

}