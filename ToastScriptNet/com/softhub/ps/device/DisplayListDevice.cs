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

	using DisplayList = com.softhub.ps.graphics.DisplayList;
	using Reusable = com.softhub.ps.graphics.Reusable;

	public abstract class DisplayListDevice : AbstractDevice
	{

		/// <summary>
		/// The current display list.
		/// </summary>
		protected internal DisplayList displayList;

		/// <summary>
		/// Initialize the device.
		/// </summary>
		public override void init()
		{
			this.displayList = createDisplayList();
			base.init();
		}

		/// <summary>
		/// Save the device state. </summary>
		/// <param name="gstate"> the graphics state </param>
		public override object save()
		{
			return new DisplayState(this);
		}

		/// <summary>
		/// Restore the device state. </summary>
		/// <param name="gstate"> the graphics state </param>
		public override void restore(object state)
		{
			((DisplayState) state).restore();
		}

		/// <summary>
		/// Initialize the clip bounds.
		/// </summary>
		public override void initclip()
		{
			base.initclip();
			displayList.initclip();
		}

		/// <summary>
		/// Clip to some path boundary. </summary>
		/// <param name="path"> the clip path </param>
		public override void clip(Shape shape)
		{
			base.clip(shape);
			displayList.clip(shape);
		}

		/// <summary>
		/// Set the current color. </summary>
		/// <param name="color"> the new color </param>
		public override Color Color
		{
			set
			{
				displayList.Color = value;
			}
		}

		/// <summary>
		/// Set the current line stroke. </summary>
		/// <param name="stroke"> the pen to use </param>
		public override Stroke Stroke
		{
			set
			{
				displayList.Stroke = value;
			}
		}

		/// <summary>
		/// Set the current paint. </summary>
		/// <param name="paint"> the paint to use </param>
		public override Paint Paint
		{
			set
			{
				displayList.Paint = value;
			}
		}

		/// <summary>
		/// Show some cached object. </summary>
		/// <param name="obj"> the reusable object </param>
		/// <param name="xform"> the tranformation matrix </param>
		public override void show(Reusable obj, AffineTransform xform)
		{
			displayList.show(obj, xform);
		}

		/// <summary>
		/// Draw an image. </summary>
		/// <param name="bitmap"> the bitmap to draw </param>
		/// <param name="xform"> the image transformation </param>
		public override void image(Bitmap bitmap, AffineTransform xform)
		{
			displayList.image(bitmap, xform);
		}

		/// <summary>
		/// Fill a shape. </summary>
		/// <param name="shape"> the shape to fill </param>
		public override void fill(Shape shape)
		{
			displayList.fill(shape);
		}

		/// <summary>
		/// Stoke a shape using the settings of the current stroke. </summary>
		/// <param name="shape"> the shape to stroke </param>
		public override void stroke(Shape shape)
		{
			displayList.stroke(shape);
		}

		/// <returns> a newly created display list </returns>
		protected internal virtual DisplayList createDisplayList()
		{
			return new DisplayList();
		}

		protected internal class DisplayState : State
		{
			private readonly DisplayListDevice outerInstance;


			internal Color color;
			internal Paint paint;
			internal Stroke stroke;

			protected internal DisplayState(DisplayListDevice outerInstance) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				color = outerInstance.displayList.Color;
				paint = outerInstance.displayList.Paint;
				stroke = outerInstance.displayList.Stroke;
			}

			protected internal override void restore()
			{
				if (color != null)
				{
					outerInstance.displayList.Color = color;
				}
				if (paint != null)
				{
					outerInstance.displayList.Paint = paint;
				}
				if (stroke != null)
				{
					outerInstance.displayList.Stroke = stroke;
				}
				outerInstance.displayList.clip(clipShape);
				base.restore();
			}

		}

	}

}