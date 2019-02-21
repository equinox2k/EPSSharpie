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

	public class SaveType : Any
	{

		private int level;
		private int vmindex;
		private bool allocmode;
		private bool packing;

		public SaveType(Interpreter ip, int level, int index)
		{
			this.level = level;
			vmindex = index;
			allocmode = ip.vm.Global;
			packing = ip.arraypacking;
		}

		public virtual int Level
		{
			get
			{
				return level;
			}
		}

		public virtual int VMIndex
		{
			get
			{
				return vmindex;
			}
		}

		public virtual bool AllocationModeGlobal
		{
			get
			{
				return allocmode;
			}
		}

		public virtual bool PackingMode
		{
			get
			{
				return packing;
			}
		}

		public override int typeCode()
		{
			return Types_Fields.SAVE;
		}

		public override string typeName()
		{
			return "savetype";
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
			return obj is SaveType && ((SaveType) obj).level == level;
		}

		public override string ToString()
		{
			return "save<" + level + ">";
		}

		public override int GetHashCode()
		{
			return 73 + level;
		}

	}

}