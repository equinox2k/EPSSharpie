using System;

namespace com.softhub.ps.graphics
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

	using Bitmap = com.softhub.ps.device.Bitmap;

	[Serializable]
	public class DisplayList : Drawable, ICloneable
	{
		public static bool debug = false;

		private const int INITIAL_SIZE = 256;

		protected internal int index;
		protected internal Command[] buffer;
		private bool interrupted;
		private bool resolutionDependent;
		private Color currentColor;
		private Stroke currentStroke;
		private Paint currentPaint;
		private Rectangle cliprect;
		private float scale = 1;

		public DisplayList() : this(INITIAL_SIZE)
		{
		}

		public DisplayList(int size)
		{
			buffer = new Command[Math.Max(1, size)];
		}

		public virtual DisplayList copy()
		{
			try
			{
				return (DisplayList) MemberwiseClone();
			}
			catch (CloneNotSupportedException)
			{
				throw new InternalError();
			}
		}

		public virtual void trimToSize()
		{
			if (index < buffer.Length)
			{
				Command[] buf = new Command[Math.Max(1, index)];
				Array.Copy(buffer, 0, buf, 0, index);
				buffer = buf;
			}
		}

		public virtual int Size
		{
			get
			{
				return buffer.Length;
			}
		}

		private void append(Command cmd)
		{
			if (index >= buffer.Length)
			{
				Command[] tmp = buffer;
				int n = buffer.Length * 2;
				buffer = new Command[n];
				Array.Copy(tmp, 0, buffer, 0, index);
			}
			buffer[index++] = cmd;
		}

		public virtual Rectangle ClipBounds
		{
			get
			{
				return cliprect;
			}
		}

		public virtual void draw(Graphics2D g)
		{
			cliprect = g.ClipBounds;
			for (int i = 0; i < index && !interrupted; i++)
			{
				buffer[i].exec(g);
			}
			interrupted = false;
		}

		public virtual void interrupt()
		{
			interrupted = true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void normalize(java.awt.geom.AffineTransform xform) throws java.awt.geom.NoninvertibleTransformException
		public virtual void normalize(AffineTransform xform)
		{
			AffineTransform ix = xform.createInverse();
			for (int i = 0; i < index; i++)
			{
				buffer[i].transform(ix);
			}
		}

		public virtual bool ResolutionDependent
		{
			get
			{
				return resolutionDependent;
			}
		}

		public virtual void show(Reusable obj, AffineTransform xform)
		{
			append(new ReusedObject(obj, xform));
		}

		public virtual void fill(Shape shape)
		{
			append(new FillCommand(shape));
		}

		public virtual void stroke(Shape shape)
		{
			append(new StrokeCommand(shape));
		}

		public virtual void initclip()
		{
			append(new ClipInit(this));
		}

		public virtual void clip(Shape shape)
		{
			append(new ClipShape(shape, this));
		}

		public virtual void image(Bitmap bitmap, AffineTransform xform)
		{
			append(new ImageCommand(bitmap, xform));
			resolutionDependent = true;
		}

		public virtual Color Color
		{
			set
			{
				if (!value.Equals(currentColor))
				{
					append(new ColorCommand(value));
					currentColor = value;
				}
			}
			get
			{
				return currentColor;
			}
		}


		public virtual Stroke Stroke
		{
			set
			{
				if (!value.Equals(currentStroke))
				{
					append(new PenCommand(value));
					currentStroke = value;
				}
			}
			get
			{
				return currentStroke;
			}
		}


		public virtual Paint Paint
		{
			set
			{
				if (!value.Equals(currentPaint))
				{
					append(new PaintCommand(value));
					currentPaint = value;
				}
			}
			get
			{
				return currentPaint;
			}
		}


		public override string ToString()
		{
			return "display-list<" + index + ">";
		}

	}

}