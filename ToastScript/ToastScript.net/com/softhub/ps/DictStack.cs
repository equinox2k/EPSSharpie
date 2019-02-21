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

	public class DictStack : System.Collections.Stack
	{

		public DictStack(int size) : base(size)
		{
		}

		public virtual Any load(Any key)
		{
			Any val = null;
			for (int i = count_Renamed - 1; val == null && i >= 0; i--)
			{
				val = ((DictType) array[i]).get(key);
			}
			return val;
		}

		public virtual Any load(string s)
		{
			return load(new NameType(s));
		}

		public virtual DictType where(Any any)
		{
			for (int i = count_Renamed - 1; i >= 0; i--)
			{
				DictType dict = (DictType) array[i];
				if (dict.known(any))
				{
					return dict;
				}
			}
			return null;
		}

		public virtual void def(VM vm, Any key, Any val)
		{
			((DictType) array[count_Renamed - 1]).put(vm, key, val);
		}

		public virtual void store(VM vm, Any key, Any val)
		{
			DictType dict = where(key);
			if (dict == null)
			{
				((DictType) array[count_Renamed - 1]).put(vm, key, val);
			}
			else
			{
				dict.put(vm, key, val);
			}
		}

		public virtual DictType currentdict()
		{
			return (DictType) array[count_Renamed - 1];
		}

		protected internal override int overflow()
		{
			return Stoppable_Fields.DICTSTACKOVERFLOW;
		}

		protected internal override int underflow()
		{
			return Stoppable_Fields.DICTSTACKUNDERFLOW;
		}

	}

}