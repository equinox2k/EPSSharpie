using System.Collections.Generic;

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

	public class VM
	{

		private const int INITIAL_SIZE = 1024;
		private const int INITIAL_SAVE_LEVEL = 1;

		private List<object> localMemory = new List<object>(INITIAL_SIZE);
		private List<object> globalMemory = new List<object>(INITIAL_SIZE);
		private int currentSaveLevel = INITIAL_SAVE_LEVEL;
		private bool global;
		private bool stringbug;

		public virtual int SaveLevel
		{
			get
			{
				return currentSaveLevel;
			}
		}

		public virtual bool InitialSaveLevel
		{
			get
			{
				return currentSaveLevel <= INITIAL_SAVE_LEVEL;
			}
		}

		public virtual int Usage
		{
			get
			{
				return localMemory.Count;
			}
		}

		public virtual int MaxUsage
		{
			get
			{
				return localMemory.capacity();
			}
		}

		public virtual SaveType save(Interpreter ip)
		{
			SaveType save = new SaveType(ip, currentSaveLevel++, localMemory.Count);
			ip.gsave();
			return save;
		}

		public virtual void restore(Interpreter ip, SaveType save)
		{
			int level = save.Level;
			ip.ostack.check(level);
			ip.estack.check(level);
			ip.dstack.check(level);
			int index = save.VMIndex;
			restoreLevel(level, index, localMemory);
			currentSaveLevel = level;
			ip.grestoreAll();
			ip.arraypacking = save.PackingMode;
			Global = save.AllocationModeGlobal;
		}

		private void restoreLevel(int level, int index, List<object> memory)
		{
			for (int i = memory.Count - 1; i >= index; i--)
			{
				CompositeType.Node node = (CompositeType.Node) memory[i];
				node.restoreLevel(this, level);
			}
			memory.Capacity = index;
		}

		public virtual void add(CompositeType.Node node)
		{
			List<object> memory = global ? globalMemory : localMemory;
			memory.Add(node);
		}

		public virtual bool Global
		{
			set
			{
				this.global = value;
			}
			get
			{
				return global;
			}
		}


		public virtual bool StringBug
		{
			set
			{
				stringbug = value;
			}
			get
			{
				return stringbug;
			}
		}


	}

}