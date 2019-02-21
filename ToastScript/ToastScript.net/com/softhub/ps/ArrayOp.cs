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

	internal sealed class ArrayOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"bind", "array", "packedarray", "setpacking", "currentpacking", "aload", "astore"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(ArrayOp));
			ip.installOp(new MarkOp("["));
			ip.installOp(new MarkOp("<<"));
			ip.installOp(new MarkOp("mark"));
			ip.installOp(new ArrayEndOp());
		}

		internal static void bind(Interpreter ip)
		{
			ArrayType array = (ArrayType) ip.ostack.top(Types_Fields.ARRAY);
			array.bind(ip);
		}

		internal sealed class MarkOp : OperatorType
		{

			internal MarkOp(string name) : base(name)
			{
			}

			public override void exec(Interpreter ip)
			{
				ip.ostack.pushRef(new MarkType());
			}

		}

		internal sealed class ArrayEndOp : OperatorType
		{

			internal ArrayEndOp() : base("]")
			{
			}

			public override void exec(Interpreter ip)
			{
				int n = ip.ostack.counttomark();
				ArrayType array = new ArrayType(ip.vm, n, ip.ostack);
				ip.ostack.remove(n + 1);
				ip.ostack.pushRef(array);
			}

		}

		internal static void array(Interpreter ip)
		{
			ip.ostack.pushRef(new ArrayType(ip.vm, ip.ostack.popInteger()));
		}

		internal static void packedarray(Interpreter ip)
		{
			int count = ip.ostack.popInteger();
			ArrayType array = new ArrayType(ip.vm, count, ip.ostack);
			array.Packed = true;
			ip.ostack.remove(count);
			ip.ostack.pushRef(array);
		}

		internal static void setpacking(Interpreter ip)
		{
			ip.arraypacking = ip.ostack.popBoolean();
		}

		internal static void currentpacking(Interpreter ip)
		{
			ip.ostack.pushRef(new BoolType(ip.arraypacking));
		}

		internal static void aload(Interpreter ip)
		{
			ArrayType a = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			System.Collections.IEnumerator e = a.elements();
			while (e.MoveNext())
			{
				ip.ostack.push((Any) e.Current);
			}
			ip.ostack.pushRef(a);
		}

		internal static void astore(Interpreter ip)
		{
			ArrayType a = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			for (int i = a.length() - 1; i >= 0; i--)
			{
				a.put(ip.vm, i, ip.ostack.pop());
			}
			ip.ostack.pushRef(a);
		}

	}

}