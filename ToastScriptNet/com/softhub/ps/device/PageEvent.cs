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

	public class PageEvent
	{

		public const int ERROR = 0;
		public const int BEGINJOB = 1;
		public const int ENDJOB = 2;
		public const int RESIZE = 3;
		public const int SHOWPAGE = 4;
		public const int COPYPAGE = 5;
		public const int ERASEPAGE = 6;

		private int type;
		private PageDevice device;

		public PageEvent(int type, PageDevice device)
		{
			this.type = type;
			this.device = device;
		}

		public virtual int Type
		{
			get
			{
				return type;
			}
		}

		public virtual PageDevice PageDevice
		{
			get
			{
				return device;
			}
		}

		public override string ToString()
		{
			switch (type)
			{
			case ERROR:
				return "ERROR";
			case BEGINJOB:
				return "BEGINJOB";
			case ENDJOB:
				return "ENDJOB";
			case RESIZE:
				return "RESIZE";
			case SHOWPAGE:
				return "SHOWPAGE";
			case COPYPAGE:
				return "COPYPAGE";
			case ERASEPAGE:
				return "ERASEPAGE";
			default:
				return base.ToString();
			}
		}

	}

}