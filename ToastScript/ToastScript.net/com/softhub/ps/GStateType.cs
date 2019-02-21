namespace com.softhub.ps
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

	public abstract class GStateType : Any
	{

		private int savelevel;

		public abstract void save(int level);

		public abstract void restore(GStateType gstate);

		internal virtual int SaveLevel
		{
			set
			{
				this.savelevel = value;
			}
			get
			{
				return savelevel;
			}
		}


		public override int typeCode()
		{
			return Types_Fields.GSTATE;
		}

		public override string typeName()
		{
			return "gstatetype";
		}

		protected internal override bool Global
		{
			get
			{
				return true;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is GStateType && ((GStateType) obj).savelevel == savelevel;
		}

		public override string ToString()
		{
			return "gstate<" + savelevel + ">";
		}

		public override int GetHashCode()
		{
			return 83 + savelevel;
		}

	}

}