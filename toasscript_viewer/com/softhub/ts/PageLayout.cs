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


	public class PageLayout : LayoutManager
	{

		public const string CENTER = "Center";

		protected internal int hgap = 12;
		protected internal int vgap = 12;
		protected internal Component center;

		public PageLayout()
		{
		}

		public virtual int GapH
		{
			set
			{
				this.hgap = value;
			}
			get
			{
				return hgap;
			}
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
			if (CENTER.Equals(name))
			{
				center = comp;
			}
		}

		public virtual void removeLayoutComponent(Component comp)
		{
			if (center == comp)
			{
				center = null;
			}
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
				Dimension t = target.Size;
				if (center != null)
				{
					Dimension d = center.Size;
					int x = Math.Max((t.width - d.width) / 2, hgap);
					int y = Math.Max((t.height - d.height) / 2, vgap);
					center.setLocation(x, y);
					center.Size = d;
				}
			}
		}

	}

}