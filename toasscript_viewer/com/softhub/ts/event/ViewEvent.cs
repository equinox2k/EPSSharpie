namespace com.softhub.ts.@event
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

	public class ViewEvent : EventObject
	{

		public const int PAGE_CHANGE = 1;
		public const int PAGE_ADJUST = 2;
		public const int PAGE_RESIZE = 3;

		private int type;

		public ViewEvent(object source, int type) : base(source)
		{
			this.type = type;
		}

		public virtual int EventType
		{
			get
			{
				return type;
			}
		}

	}

}