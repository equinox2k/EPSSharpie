using System.Collections;
using System.Collections.Generic;

namespace com.softhub.ps.util
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


	public class Cache
	{

		private int size;
		private int maximumSize;
		private Hashtable table;
		private LinkedList list;

		public Cache(int maximumSize)
		{
			this.size = 0;
			this.maximumSize = maximumSize;
			this.table = new Hashtable(maximumSize);
			this.list = new LinkedList();
		}

		public virtual void put(object key, object val)
		{
			lock (this)
			{
				if (size < maximumSize)
				{
					table[key] = val;
					list.AddFirst(key);
					size++;
				}
				else
				{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
					if (!list.remove(key))
					{
						if (list.Count > 0)
						{
							table.Remove(list.RemoveLast());
						}
					}
					list.AddFirst(key);
					table[key] = val;
				}
			}
		}

		public virtual object get(object key)
		{
			lock (this)
			{
				return table[key];
			}
		}

		public virtual void clear()
		{
			lock (this)
			{
				table.Clear();
				list.Clear();
				size = 0;
			}
		}

		public virtual int Size
		{
			get
			{
				return size;
			}
		}

		public virtual int MaximumSize
		{
			get
			{
				return maximumSize;
			}
			set
			{
				maximumSize = value;
			}
		}


	}

}