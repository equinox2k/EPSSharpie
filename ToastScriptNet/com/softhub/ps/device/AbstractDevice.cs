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

	using Reusable = com.softhub.ps.graphics.Reusable;

	public abstract class AbstractDevice : Device, BeanInfo
	{
		public abstract Dimension Size {get;}
		public abstract float Scale {get;}
		public abstract float Resolution {get;}

		/// <summary>
		/// The device transformation.
		/// </summary>
		protected internal AffineTransform dtm = new AffineTransform(1,0,0,1,0,0);

		/// <summary>
		/// The graphics context for clipping.
		/// </summary>
		protected internal Graphics2D clipGraphics;

		/// <summary>
		/// Initialize the device.
		/// </summary>
		public virtual void init()
		{
			initClipGraphics();
		}

		protected internal virtual void initClipGraphics()
		{
			BufferedImage image = createClipImage();
			clipGraphics = image.createGraphics();
		}

		/// <summary>
		/// Save the device state. </summary>
		/// <param name="gstate"> the graphics state </param>
		public virtual object save()
		{
			return new State(this);
		}

		/// <summary>
		/// Restore the device state. </summary>
		/// <param name="gstate"> the graphics state </param>
		public virtual void restore(object state)
		{
			((State) state).restore();
		}

		/// <returns> the device name </returns>
		public virtual string Name
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return this.GetType().FullName;
			}
		}

		/// <summary>
		/// Create RGB color. </summary>
		/// <param name="r"> the red component </param>
		/// <param name="g"> the green component </param>
		/// <param name="b"> the blue component </param>
		/// <returns> the color </returns>
		public virtual Color createColor(float r, float g, float b)
		{
			return new Color(r, g, b);
		}

		/// <summary>
		/// Create stroke. </summary>
		/// <param name="width"> the stroke width </param>
		/// <param name="cap"> the stroke cap index </param>
		/// <param name="join"> the stroke join index </param>
		/// <param name="miter"> the miter limit </param>
		/// <param name="array"> the dash array </param>
		/// <param name="phase"> the dash phase </param>
		/// <returns> the stroke </returns>
		public virtual Stroke createStroke(float width, int cap, int join, float miter, float[] array, float phase)
		{
			float widthlimit = Math.Max(0.001f, width);
			float miterlimit = Math.Max(1, miter);
			return new BasicStroke(widthlimit, cap, join, miterlimit, array, phase);
		}

		/// <summary>
		/// Create a bitmap. </summary>
		/// <param name="width"> the width of the bitmap </param>
		/// <param name="height"> the height of the bitmap </param>
		/// <returns> a bitmap of specified size </returns>
		public virtual Bitmap createBitmap(int width, int height)
		{
			return new DefaultBitmap(width, height);
		}

		/// <returns> a buffered image the size of the device for clipping </returns>
		protected internal virtual Image createClipImage()
		{

			Dimension d = Size;
			int w = Math.Max(1, d.width);
			int h = Math.Max(1, d.height);
			return new Image(w, h, BufferedImage.TYPE_BYTE_BINARY);
		}

		protected internal virtual Shape Clip
		{
			get
			{
				Shape shape = clipGraphics.Clip;
				return shape != null ? shape : cliprect();
			}
		}

		/// <returns> the clipping boundary </returns>
		protected internal abstract Rectangle2D cliprect();

		/// <summary>
		/// Initialize the clip bounds.
		/// </summary>
		public virtual void initclip()
		{
			clipGraphics.Clip = cliprect();
		}

		/// <summary>
		/// Clip to some path boundary. </summary>
		/// <param name="path"> the clip path </param>
		public virtual void clip(Shape shape)
		{
			clipGraphics.clip(shape);
		}

		/// <returns> the current clip shape </returns>
		public virtual Shape clippath()
		{
			Shape shape = clipGraphics.Clip;
			return shape != null ? shape : cliprect();
		}

		/// <summary>
		/// Set the default matrix. </summary>
		/// <param name="xform"> the default matrix </param>
		public virtual AffineTransform DefaultMatrix
		{
			set
			{
				dtm.Transform = value;
			}
			get
			{
				return dtm;
			}
		}


		/// <summary>
		/// Set the current color. </summary>
		/// <param name="color"> the new color </param>
		public virtual Color Color
		{
			set
			{
			}
		}

		/// <summary>
		/// Set the current line stroke. </summary>
		/// <param name="stroke"> the pen to use </param>
		public virtual Stroke Stroke
		{
			set
			{
			}
		}

		/// <summary>
		/// Set the current paint. </summary>
		/// <param name="paint"> the paint to use </param>
		public virtual Paint Paint
		{
			set
			{
			}
		}

		/// <summary>
		/// Set the current font. </summary>
		/// <param name="info"> the font info </param>
		public virtual FontInfo Font
		{
			set
			{
			}
		}

		/// <summary>
		/// Show some cached object. </summary>
		/// <param name="obj"> the reusable object </param>
		/// <param name="xform"> the tranformation matrix </param>
		public virtual void show(Reusable obj, AffineTransform xform)
		{
		}

		/// <summary>
		/// Draw an image. </summary>
		/// <param name="bitmap"> the bitmap to draw </param>
		/// <param name="xform"> the image transformation </param>
		public virtual void image(Bitmap bitmap, AffineTransform xform)
		{
		}

		/// <summary>
		/// Fill a path. </summary>
		/// <param name="path"> the path to fill </param>
		public virtual void fill(Shape shape)
		{
		}

		/// <summary>
		/// Stoke a path using the settings of the current stroke. </summary>
		/// <param name="path"> the path to stroke </param>
		public virtual void stroke(Shape shape)
		{
		}

		/// <summary>
		/// Show the current page and erase it.
		/// </summary>
		public virtual void showpage()
		{
		}

		/// <summary>
		/// Copy and show the current page.
		/// </summary>
		public virtual void copypage()
		{
		}

		/// <summary>
		/// Erase the current page.
		/// </summary>
		public virtual void erasepage()
		{
		}

		/// <summary>
		/// Called by jobserver to indicate that
		/// a save/restore encapsulated job begins.
		/// </summary>
		public virtual void beginJob()
		{
		}

		/// <summary>
		/// Called by jobserver to indicate that
		/// the current job is done.
		/// </summary>
		public virtual void endJob()
		{
		}

		/// <summary>
		/// Error in job. </summary>
		/// <param name="msg"> the error message </param>
		public virtual void error(string msg)
		{
		}

		/// <summary>
		/// Convert device property. </summary>
		/// <param name="name"> the name of the property </param>
		/// <param name="value"> the value of the property </param>
		public virtual object convertType(string name, object value)
		{
			return value;
		}

		/// <returns> bean descriptor for this device </returns>
		public virtual BeanDescriptor BeanDescriptor
		{
			get
			{
				return null;
			}
		}

		/// <returns> property descriptors for this device </returns>
		public virtual PropertyDescriptor[] PropertyDescriptors
		{
			get
			{
				return null;
			}
		}

		public virtual int DefaultPropertyIndex
		{
			get
			{
				return -1;
			}
		}

		public virtual EventSetDescriptor[] EventSetDescriptors
		{
			get
			{
				return null;
			}
		}

		public virtual int DefaultEventIndex
		{
			get
			{
				return -1;
			}
		}

		public virtual MethodDescriptor[] MethodDescriptors
		{
			get
			{
				return null;
			}
		}

		public virtual BeanInfo[] AdditionalBeanInfo
		{
			get
			{
				return null;
			}
		}

		public virtual Image getIcon(int iconKind)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public java.awt.Image loadImage(final String resourceName)
		public virtual Image loadImage(string resourceName)
		{
			return null;
		}

		protected internal class State
		{
			private readonly AbstractDevice outerInstance;


			protected internal Shape clipShape;

			protected internal State(AbstractDevice outerInstance)
			{
				this.outerInstance = outerInstance;
				clipShape = outerInstance.Clip;
			}

			protected internal virtual void restore()
			{
				outerInstance.clipGraphics.Clip = clipShape;
			}

		}

	}

}