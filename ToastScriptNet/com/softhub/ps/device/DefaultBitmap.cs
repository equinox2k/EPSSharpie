using System;
using ToastScriptNet;

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


	public class DefaultBitmap : Bitmap
	{

		private BufferedImage image;


		public DefaultBitmap(int w, int h)
		{
			image = new BufferedImage(Math.Max(1, w), Math.Max(1, h));
		}

		public virtual int Width
		{
			get
			{
				return image.Width;
			}
		}

		public virtual int Height
		{
			get
			{
				return image.Height;
			}
		}

		public virtual void draw(int x, int y, int color)
		{
            throw new NotImplementedException();
			//image.setRGB(x, y, color);
		}

		public virtual void draw(int x, int y, Color color)
		{
			draw(x, y, color);
		}

		public virtual void drawImage(Graphics2D g, AffineTransform xform)
		{
			g.drawImage(image, xform);
		}

		public virtual Image Image
		{
			get
			{
				return (Image)image;
			}
		}

	}

}