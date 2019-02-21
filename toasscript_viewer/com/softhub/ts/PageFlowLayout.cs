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


	public class PageFlowLayout : LayoutManager
	{

		private int vgap = 12;

		public PageFlowLayout()
		{
		}

		public virtual int GapV
		{
			set
			{
				this.vgap = value;
			}
			get
			{
				return vgap;
			}
		}


		public virtual void addLayoutComponent(string name, Component comp)
		{
		}

		public virtual void removeLayoutComponent(Component comp)
		{
		}

		public virtual Dimension preferredLayoutSize(Container target)
		{
			return new Dimension(80, 80);
		}

		public virtual Dimension minimumLayoutSize(Container target)
		{
			return new Dimension(120, 120);
		}

		public virtual void layoutContainer(Container target)
		{
			lock (target.TreeLock)
			{
				Component[] comp = target.Components;
				int i, w = 0, h = 0, n = comp.Length;
				for (i = 0; i < n; i++)
				{
					Component c = comp[i];
					Dimension d = c.Size;
					c.setLocation(0, h);
					w = Math.Max(w, d.width);
					h += d.height + vgap;
				}
				target.setSize(w, h);
			}
		}

	}

}