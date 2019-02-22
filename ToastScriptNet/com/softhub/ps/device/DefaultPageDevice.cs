using System;
using System.Collections.Generic;
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

	using Drawable = com.softhub.ps.graphics.Drawable;

	public class DefaultPageDevice : DisplayListDevice, PageDevice
	{
		/// <summary>
		/// Vector of page event listeners.
		/// </summary>
		private List<object> listeners = new List<object>();

		/// <summary>
		/// The width of the page in 1/72 of an inch.
		/// </summary>
		private float width;

		/// <summary>
		/// The height of the page in 1/72 of an inch.
		/// </summary>
		private float height;

		/// <summary>
		/// The scale factor.
		/// </summary>
		private float scale;

		/// <summary>
		/// The screen resulution.
		/// </summary>
		private float dpi;

		/// <summary>
		/// The page orientation.
		/// </summary>
		private int orientation;

		/// <summary>
		/// The number of copies.
		/// </summary>
		private int copies = 1;

		/// <summary>
		/// The bitmap type.
		/// </summary>
		private int bitmapType = BufferedImage.TYPE_INT_ARGB;

		/// <summary>
		/// True if size has changed. Cleared in init method.
		/// </summary>
		private bool sizeHasChanged;

		/// <summary>
		/// Create a new page device.
		/// </summary>
		public DefaultPageDevice() : this(612, 792, 1, 90)
		{
		}

		/// <summary>
		/// Create a new page device. </summary>
		/// <param name="width"> the width in 1/72 inch </param>
		/// <param name="height"> the height in 1/72 inch </param>
		/// <param name="scale"> the scale factor </param>
		/// <param name="dpi"> the output resolution </param>
		public DefaultPageDevice(float width, float height, float scale, float dpi)
		{
			this.width = width;
			this.height = height;
			this.scale = scale;
			this.dpi = dpi;
		}

		/// <summary>
		/// Initialize the device.
		/// </summary>
		public override void init()
		{
			initMatrix();
			base.init();
			if (sizeHasChanged)
			{
				firePageEvent(PageEvent.RESIZE);
				sizeHasChanged = false;
			}
		}

		/// <summary>
		/// Initialize the device matrix.
		/// </summary>
		private void initMatrix()
		{
			float s = scale * dpi / 72;
			switch (orientation)
			{
			case 0:
				DefaultMatrix = new AffineTransform(s, 0, 0, -s, 0, height * s);
				break;
			case 1:
				DefaultMatrix = new AffineTransform(0, s, s, 0, 0, 0);
				break;
			}
		}

		/// <returns> the name of the device </returns>
		public override string Name
		{
			get
			{
				return "pagedevice";
			}
		}

		/// <returns> the resolution in dots per inch </returns>
		public override float Resolution
		{
			get
			{
				return dpi;
			}
		}

		/// <summary>
		/// Set the page size. </summary>
		/// <param name="size"> array of 2 elements {width, height} </param>
		public virtual float[] PageSize
		{
			set
			{
				width = value[0];
				height = value[1];
				sizeHasChanged = true;
			}
			get
			{
				float[] size = new float[2];
				size[0] = width;
				size[1] = height;
				return size;
			}
		}


		/// <summary>
		/// Set the scale factor. </summary>
		/// <param name="the"> absolute scale factor in percent </param>
		public virtual float Scale
		{
			set
			{
				this.scale = value;
				sizeHasChanged = true;
			}
			get
			{
				return scale;
			}
		}


		/// <summary>
		/// Set the number of copies for showpage to print. </summary>
		/// <param name="the"> number of copies </param>
		public virtual int NumCopies
		{
			set
			{
				this.copies = value;
			}
			get
			{
				return copies;
			}
		}


		/// <summary>
		/// Set the page orientation. </summary>
		/// <param name="mode"> [0..3] </param>
		public virtual int Orientation
		{
			set
			{
				this.orientation = value;
				sizeHasChanged = true;
			}
			get
			{
				return orientation;
			}
		}


		/// <summary>
		/// Set the type of bitmap used by this device.
		/// The constants are declared in BufferedImage.java </summary>
		/// <param name="the"> bitmap type </param>
		public virtual int BitmapType
		{
			set
			{
				this.bitmapType = value;
			}
			get
			{
				return bitmapType;
			}
		}


		/// <returns> the page width in 1/72 inch </returns>
		public virtual float PageWidth
		{
			get
			{
				return width;
			}
		}

		/// <returns> the page height in 1/72 inch </returns>
		public virtual float PageHeight
		{
			get
			{
				return height;
			}
		}

		/// <returns> the current page content </returns>
		public virtual Drawable Content
		{
			get
			{
				return displayList;
			}
		}

		/// <summary>
		/// Show the current page and erase it.
		/// </summary>
		public override void showpage()
		{
			displayList.trimToSize();
			firePageEvent(PageEvent.SHOWPAGE);
			displayList = createDisplayList();
		}

		/// <summary>
		/// Copy and show the current page.
		/// </summary>
		public override void copypage()
		{
			firePageEvent(PageEvent.COPYPAGE);
			displayList = displayList.copy();
		}

		/// <summary>
		/// Erase the current page.
		/// </summary>
		public override void erasepage()
		{
			firePageEvent(PageEvent.ERASEPAGE);
			displayList = createDisplayList();
		}

		/// <summary>
		/// Begin of job.
		/// </summary>
		public override void beginJob()
		{
			firePageEvent(PageEvent.BEGINJOB);
		}

		/// <summary>
		/// End of job.
		/// </summary>
		public override void endJob()
		{
			firePageEvent(PageEvent.ENDJOB);
		}

		/// <summary>
		/// Error in job. </summary>
		/// <param name="msg"> the error message </param>
		public override void error(string msg)
		{
			firePageEvent(PageEvent.ERROR);
		}

		/// <summary>
		/// Create a bitmap. </summary>
		/// <param name="wbits"> the width of the bitmap </param>
		/// <param name="hbits"> the height of the bitmap </param>
		/// <returns> a bitmap of specified size </returns>
		public override Bitmap createBitmap(int wbits, int hbits)
		{
			return new DefaultBitmap(wbits, hbits, bitmapType);
		}

		/// <summary>
		/// Get the default clip rect. </summary>
		/// <returns> the clip rect </returns>
		protected internal override Rectangle2D cliprect()
		{
			float s = scale * dpi / 72;
			return new Rectangle2D(0, 0, s * width, s * height);
		}

		/// <returns> the device size </returns>
		public override Dimension Size
		{
			get
			{
				double s = scale * dpi / 72;
				int wbits = (int) (long)Math.Round(s * width, MidpointRounding.AwayFromZero);
				int hbits = (int) (long)Math.Round(s * height, MidpointRounding.AwayFromZero);
				return new Dimension(wbits, hbits);
			}
		}

		/// <summary>
		/// The device is treated like a bean, which is (in this case) it's
		/// own bean descriptor. Any properties exposed here can be used in the
		/// dictionary parameter to setpagedevice. </summary>
		/// <returns> an array of properties </returns>
		public PropertyDescriptor[] PropertyDescriptors
		{
			get
			{
				try
				{
					PropertyDescriptor[] properties = new PropertyDescriptor[]
					{
						new PropertyDescriptor("PageSize", typeof(DefaultPageDevice)),
						new PropertyDescriptor("Scale", typeof(DefaultPageDevice)),
						new PropertyDescriptor("NumCopies", typeof(DefaultPageDevice)),
						new PropertyDescriptor("Orientation", typeof(DefaultPageDevice)),
						new PropertyDescriptor("BitmapType", typeof(DefaultPageDevice))
					};
					return properties;
				}
				catch (Exception ex)
				{
					System.Console.Error.WriteLine(ex);
				}
				return null;
			}
		}

		/// <summary>
		/// Convert the property. If the Java type of the property
		/// does not correpond to the PostScript type as required
		/// by the signature of the getter/setter method, we convert
		/// it here. </summary>
		/// <param name="name"> the name of the property </param>
		/// <param name="value"> the value of the property </param>
		public override object convertType(string name, object value)
		{
			if ("PageSize".Equals(name))
			{
				Number[] numbers = (Number[]) value;
				float[] array = new float[2];
				array[0] = numbers[0].floatValue();
				array[1] = numbers[1].floatValue();
				value = array;
			}
			else if ("Orientation".Equals(name))
			{
				Number number = (Number) value;
				value = new int?(number.intValue());
			}
			else if ("Scale".Equals(name))
			{
				Number number = (Number) value;
				value = new float?(number.floatValue());
			}
			return value;
		}

		/// <summary>
		/// Add a page event listener. </summary>
		/// <param name="the"> listener to add </param>
		public virtual void addPageEventListener(PageEventListener listener)
		{
			listeners.Add(listener);
		}

		/// <summary>
		/// Remove a page event listener. </summary>
		/// <param name="the"> listener to remove </param>
		public virtual void removePageEventListener(PageEventListener listener)
		{
			listeners.Remove(listener);
		}

		/// <summary>
		/// Send page event to all listeners.
		/// </summary>
		private void firePageEvent(int type)
		{
			PageEvent evt = new PageEvent(type, this);
			System.Collections.IEnumerator e = listeners.elements();
			while (e.MoveNext())
			{
				((PageEventListener) e.Current).pageDeviceChanged(evt);
			}
		}

	}

}