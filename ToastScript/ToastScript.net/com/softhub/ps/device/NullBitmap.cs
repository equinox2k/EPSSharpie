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


	public class NullBitmap : Bitmap
	{

		private int width;
		private int height;

		public NullBitmap(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public virtual int Width
		{
			get
			{
				return width;
			}
		}

		public virtual int Height
		{
			get
			{
				return height;
			}
		}

		public virtual void draw(int x, int y, int color)
		{
		}

		public virtual void draw(int x, int y, Color color)
		{
		}

		public virtual void drawImage(Graphics2D g, AffineTransform xform)
		{
		}

		public virtual Image Image
		{
			get
			{
				return null;
			}
		}

	}

}