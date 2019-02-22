using ToastScriptNet;

namespace com.softhub.ps.util
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


	public class CharWidth
	{

		// Side bearing of current character.
		private float sx, sy;

		// Width of current character.
		private float wx, wy;

		public CharWidth()
		{
		}

		public CharWidth(float wx)
		{
			this.wx = wx;
		}

		public CharWidth(float sx, float wx)
		{
			this.sx = sx;
			this.wx = wx;
		}

		public CharWidth(float sx, float sy, float wx, float wy)
		{
			this.sx = sx;
			this.sy = sy;
			this.wx = wx;
			this.wy = wy;
		}

		public CharWidth(CharWidth cw)
		{
			this.sx = cw.sx;
			this.sy = cw.sy;
			this.wx = cw.wx;
			this.wy = cw.wy;
		}

		public virtual void init()
		{
			this.sx = 0;
			this.sy = 0;
			this.wx = 0;
			this.wy = 0;
		}

		public virtual float SideBearingX
		{
			get
			{
				return sx;
			}
		}

		public virtual float SideBearingY
		{
			get
			{
				return sy;
			}
		}

		public virtual float DeltaX
		{
			get
			{
				return wx;
			}
		}

		public virtual float DeltaY
		{
			get
			{
				return wy;
			}
		}

		public virtual float Width
		{
			set
			{
				this.sx = value;
			}
		}

		public virtual void setWidth(float sx, float wx)
		{
			this.sx = sx;
			this.wx = wx;
		}

		public virtual void setWidth(float sx, float sy, float wx, float wy)
		{
			this.sx = sx;
			this.sy = sy;
			this.wx = wx;
			this.wy = wy;
		}

		public virtual CharWidth Width
		{
			set
			{
				this.sx = value.sx;
				this.sy = value.sy;
				this.wx = value.wx;
				this.wy = value.wy;
			}
		}

		public virtual CharWidth transform(AffineTransform xform)
		{
			Point2D spt = xform.deltaTransform(new Point2D.Float(sx, sy), null);
			Point2D wpt = xform.deltaTransform(new Point2D.Float(wx, wy), null);
			return new CharWidth((float) spt.X, (float) spt.Y, (float) wpt.X, (float) wpt.Y);
		}

		public override string ToString()
		{
			return "[" + sx + ", " + sy + ", " + wx + ", " + wy + "]";
		}

	}

}