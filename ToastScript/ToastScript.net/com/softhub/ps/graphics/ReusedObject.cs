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


	[Serializable]
	internal class ReusedObject : Command
	{

		private Reusable obj;
		private AffineTransform xform;

		internal ReusedObject(Reusable obj, AffineTransform xform)
		{
			this.obj = obj;
			this.xform = xform;
		}

		internal override void exec(Graphics2D g)
		{
			Graphics2D gt = (Graphics2D) g.create();
			gt.transform(xform);
			obj.draw(gt);
			gt.dispose();
		}

		internal override void transform(AffineTransform xform)
		{
			this.xform.concatenate(xform);
		}

		internal override Rectangle2D Bounds2D
		{
			get
			{
				return obj.Bounds2D;
			}
		}

		public override string ToString()
		{
			return obj + " " + xform;
		}

	}

}