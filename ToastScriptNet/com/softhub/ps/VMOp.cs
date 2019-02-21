using System;

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

	internal sealed class VMOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"save", "restore", "setglobal", "currentglobal", "gcheck", "vmstatus", "defineuserobject", "undefineuserobject", "execuserobject", "vmreclaim", "setvmthreshold"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(VMOp));
		}

		internal static void save(Interpreter ip)
		{
			ip.ostack.pushRef(ip.vm.save(ip));
		}

		internal static void restore(Interpreter ip)
		{
			ip.vm.restore(ip, (SaveType) ip.ostack.pop(Types_Fields.SAVE));
		}

		internal static void setglobal(Interpreter ip)
		{
			ip.vm.Global = ip.ostack.popBoolean();
		}

		internal static void currentglobal(Interpreter ip)
		{
			ip.ostack.pushRef(new BoolType(ip.vm.Global));
		}

		internal static void gcheck(Interpreter ip)
		{
			ip.ostack.pushRef(new BoolType(ip.ostack.pop().Global));
		}

		internal static void vmstatus(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.vm.SaveLevel));
			ip.ostack.pushRef(new IntegerType(ip.vm.Usage));
			ip.ostack.pushRef(new IntegerType(ip.vm.MaxUsage * 4096));
		}

		internal static void vmreclaim(Interpreter ip)
		{
			int code = ip.ostack.popInteger();
			// TODO: implement vmreclaim
		}

		internal static void setvmthreshold(Interpreter ip)
		{
			int code = ip.ostack.popInteger();
			// TODO: implement setvmthreshold
		}

		internal static void defineuserobject(Interpreter ip)
		{
			Any any = ip.ostack.pop();
			int index = ip.ostack.popInteger();
			DictType userdict = (DictType) ip.systemdict.get("userdict");
			Any userobjects = (ArrayType) userdict.get("UserObjects");
			bool global = ip.vm.Global;
			ip.vm.Global = false;
			ArrayType array;
			if (userobjects == null || !(userobjects is ArrayType))
			{
				array = new ArrayType(ip.vm, Math.Max(10, index + 10));
				userdict.put(ip.vm, "UserObjects", array);
			}
			else
			{
				ArrayType oldArray = (ArrayType) userobjects;
				if (oldArray.length() <= index)
				{
					array = new ArrayType(ip.vm, index + 10);
					array.putinterval(ip.vm, 0, oldArray);
					userdict.put(ip.vm, "UserObjects", array);
				}
				else
				{
					array = oldArray;
				}
			}
			array.put(ip.vm, index, any);
			ip.vm.Global = global;
		}

		internal static void undefineuserobject(Interpreter ip)
		{
			int index = ip.ostack.popInteger();
			DictType userdict = (DictType) ip.systemdict.get("userdict");
			Any userobjects = userdict.get("UserObjects");
			if (userobjects != null && userobjects is ArrayType)
			{
				ArrayType array = (ArrayType) userobjects;
				array.put(ip.vm, index, new NullType());
			}
		}

		internal static void execuserobject(Interpreter ip)
		{
			int index = ip.ostack.popInteger();
			DictType userdict = (DictType) ip.systemdict.get("userdict");
			Any userobjects = userdict.get("UserObjects");
			if (userobjects == null)
			{
				throw new Stop(Stoppable_Fields.UNDEFINED);
			}
			if (!(userobjects is ArrayType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "UserObjects");
			}
			ArrayType array = (ArrayType) userobjects;
			ip.estack.push(array.get(index));
		}

	}

}