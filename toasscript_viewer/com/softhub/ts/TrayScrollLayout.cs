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


	public class TrayScrollLayout : ScrollPaneLayout, TrayConstants
	{
		protected internal Component controlBar;

		public virtual void addLayoutComponent(string s, Component comp)
		{
			if (s.Equals(TrayConstants_Fields.CONTROL_BAR))
			{
				controlBar = addSingletonComponent(controlBar, comp);
			}
			else
			{
				base.addLayoutComponent(s, comp);
			}
		}

		public virtual void removeLayoutComponent(Component comp)
		{
			if (comp == controlBar)
			{
				controlBar = null;
			}
			else
			{
				base.removeLayoutComponent(comp);
			}
		}

		public virtual Component ControlBar
		{
			get
			{
				return controlBar;
			}
		}

		public virtual void layoutContainer(Container parent)
		{
			base.layoutContainer(parent);
			if (controlBar != null)
			{
				if (hsb != null && hsb.Visible)
				{
					Rectangle r = hsb.Bounds;
					// height of control bar controlled by its preferred size
					Dimension controlSize = controlBar.PreferredSize;
					int w2 = r.width / 2;
					int h = controlSize.height;
					int yc = r.y - (h - r.height);
					Rectangle leftR = new Rectangle(r.x, yc, w2, h);
					Rectangle rightR = new Rectangle(r.x + w2, r.y, w2, r.height);
					controlBar.Bounds = leftR;
					hsb.Bounds = rightR;
				}
				else if (viewport != null)
				{
					Rectangle r = viewport.Bounds;
					Dimension d = controlBar.PreferredSize;
					// doesn't seem to have effect anymore... was it necessary in old jre?
					viewport.setBounds(r.x, r.y, r.width, r.height - d.height);
					controlBar.setBounds(r.x, r.y + r.height - d.height, r.width, d.height);
				}
			}
		}

	}

}