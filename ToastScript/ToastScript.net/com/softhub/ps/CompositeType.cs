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

	public abstract class CompositeType : Any
	{

		public CompositeType() : this(true)
		{
		}

		public CompositeType(bool global) : base()
		{
			if (global)
			{
				setGlobal();
			}
		}

		public CompositeType(CompositeType comp) : this(comp, false)
		{
		}

		public CompositeType(CompositeType comp, bool global) : base(comp)
		{
			if (global)
			{
				setGlobal();
			}
		}

		public abstract int SaveLevel {get;}

		internal class Node
		{

			internal int createlevel;
			internal int currentlevel;
			internal Node prev;

			protected internal Node(VM vm)
			{
				createlevel = currentlevel = vm.SaveLevel;
			}

			internal Node(VM vm, Node node)
			{
				createlevel = node.createlevel;
				currentlevel = node.currentlevel;
				prev = node.prev;
				node.currentlevel = vm.SaveLevel;
				node.prev = this;
				vm.add(node);
			}

			internal virtual void copy(Node node)
			{
				node.currentlevel = currentlevel;
				node.prev = prev;
			}

			internal virtual int SaveLevel
			{
				get
				{
					return createlevel;
				}
			}

			internal virtual bool checkLevel(VM vm)
			{
				return vm != null && !vm.Global && vm.SaveLevel > currentlevel;
			}

			internal virtual void restoreLevel(VM vm, int level)
			{
				Node node = this;
				while (node != null && node.currentlevel > level)
				{
					node = node.prev;
				}
				if (node != null && node != this)
				{
					node.copy(this);
				}
			}

		}

	}

}