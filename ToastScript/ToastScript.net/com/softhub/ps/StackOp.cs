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

	internal sealed class StackOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"clear", "count", "countexecstack", "countdictstack", "counttomark", "cleartomark", "index", "roll"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(StackOp));
			ip.installOp(new PopOp());
			ip.installOp(new DupOp());
			ip.installOp(new ExchOp());
		}

		internal static void clear(Interpreter ip)
		{
			ip.ostack.clear();
		}

		internal static void count(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.ostack.count()));
		}

		internal static void countexecstack(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.estack.count()));
		}

		internal static void countdictstack(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.dstack.count()));
		}

		internal static void counttomark(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.ostack.counttomark()));
		}

		internal static void cleartomark(Interpreter ip)
		{
			ip.ostack.cleartomark();
		}

		internal static void index(Interpreter ip)
		{
			ip.ostack.push(ip.ostack.index(ip.ostack.popInteger()));
		}

		internal static void roll(Interpreter ip)
		{
			int j = ip.ostack.popInteger();
			int n = ip.ostack.popInteger();
			ip.ostack.roll(n, j);
		}

		internal class PopOp : OperatorType
		{

			internal PopOp() : base("pop")
			{
			}

			public override void exec(Interpreter ip)
			{
				ip.ostack.pop();
			}

		}

		internal class DupOp : OperatorType
		{

			internal DupOp() : base("dup")
			{
			}

			public override void exec(Interpreter ip)
			{
				ip.ostack.dup();
			}

		}

		internal class ExchOp : OperatorType
		{

			internal ExchOp() : base("exch")
			{
			}

			public override void exec(Interpreter ip)
			{
				ip.ostack.exch();
			}

		}

	}

}