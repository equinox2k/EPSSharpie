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

	internal sealed class DictOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"dict", "where", "undef", "known", "begin", "end", "currentdict", "maxlength", "cleardictstack", "dictstack"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(DictOp));
			ip.installOp(new DictEndOp());
			ip.installOp(new DefOp());
			ip.installOp(new StoreOp());
			ip.installOp(new LoadOp());
		}

		internal static void dict(Interpreter ip)
		{
			int n = ip.ostack.popInteger();
			if (n < 0)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			ip.ostack.pushRef(new DictType(ip.vm, n));
		}

		internal sealed class DictEndOp : OperatorType
		{

			internal DictEndOp() : base(">>")
			{
			}

			public override void exec(Interpreter ip)
			{
				int n = ip.ostack.counttomark();
				if ((n & 1) != 0)
				{
					throw new Stop(Stoppable_Fields.RANGECHECK);
				}
				DictType dict = new DictType(ip.vm, n, ip.ostack);
				ip.ostack.remove(n + 1);
				ip.ostack.pushRef(dict);
			}

		}

		internal sealed class DefOp : OperatorType
		{

			internal DefOp() : base("def")
			{
			}

			public override void exec(Interpreter ip)
			{
				Any val = ip.ostack.pop();
				Any key = ip.ostack.pop();
				ip.dstack.def(ip.vm, key, val);
			}

		}

		internal sealed class StoreOp : OperatorType
		{

			internal StoreOp() : base("store")
			{
			}

			public override void exec(Interpreter ip)
			{
				Any val = ip.ostack.pop();
				Any key = ip.ostack.pop();
				ip.dstack.store(ip.vm, key, val);
			}

		}

		internal sealed class LoadOp : OperatorType
		{

			internal LoadOp() : base("load")
			{
			}

			public override void exec(Interpreter ip)
			{
				Any key = ip.ostack.pop(Types_Fields.ANY & ~Types_Fields.NULL);
				Any val = ip.dstack.load(key);
				if (val == null)
				{
					throw new Stop(Stoppable_Fields.UNDEFINED);
				}
				ip.ostack.push(val);
				ip.ostack.LineNo = key;
			}

		}

		internal static void where(Interpreter ip)
		{
			DictType dict = (DictType) ip.dstack.where(ip.ostack.pop(Types_Fields.ANY & ~Types_Fields.NULL));
			if (dict != null)
			{
				ip.ostack.push(dict);
				ip.ostack.push(BoolType.TRUE);
			}
			else
			{
				ip.ostack.push(BoolType.FALSE);
			}
		}

		internal static void undef(Interpreter ip)
		{
			Any key = ip.ostack.pop();
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			dict.remove(ip.vm, key);
		}

		internal static void known(Interpreter ip)
		{
			Any key = ip.ostack.pop();
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			ip.ostack.pushRef(new BoolType(dict.known(key)));
		}

		internal static void begin(Interpreter ip)
		{
			ip.dstack.pushRef(ip.ostack.pop(Types_Fields.DICT));
		}

		internal static void end(Interpreter ip)
		{
			ip.dstack.pop();
		}

		internal static void currentdict(Interpreter ip)
		{
			ip.ostack.push(ip.dstack.currentdict());
		}

		internal static void maxlength(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(((DictType) ip.ostack.pop(Types_Fields.DICT)).maxlength()));
		}

		internal static void cleardictstack(Interpreter ip)
		{
			ip.dstack.clear();
		}

		internal static void dictstack(Interpreter ip)
		{
			ArrayType a = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			for (int i = 0; i < ip.dstack.count(); i++)
			{
				a.put(ip.vm, i, (Any) ip.dstack.elementAt(i));
			}
			ip.ostack.pushRef(a);
		}

	}

}