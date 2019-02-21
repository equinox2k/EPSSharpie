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

	internal sealed class TypeOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"type", "cvlit", "cvx", "xcheck", "cvn", "cvi", "cvr", "cvrs", "cvs", "executeonly", "noaccess", "readonly", "rcheck", "wcheck"};

		private const string NOSTRINGVAL = "--nostringval--";

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(TypeOp));
		}

		internal static void type(Interpreter ip)
		{
			ip.ostack.pushRef(new NameType(ip.ostack.pop().typeName()));
		}

		internal static void cvlit(Interpreter ip)
		{
			ip.ostack.top().cvlit();
		}

		internal static void cvx(Interpreter ip)
		{
			ip.ostack.top().cvx();
		}

		internal static void xcheck(Interpreter ip)
		{
			Any any = ip.ostack.pop();
			ip.ostack.pushRef(new BoolType(any.Executable));
		}

		internal static void cvn(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ip.ostack.pushRef(new NameType(s));
		}

		internal static void cvi(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.STRING);
			int result = 0;
			if (any is StringType)
			{
				double val = 0;
				try
				{
					val = Convert.ToDouble(new string(((StringType) any).toCharArray()));
				}
				catch (System.FormatException)
				{
					// TODO: other NumberType formats
					throw new Stop(Stoppable_Fields.TYPECHECK, "cvi");
				}
				if (val < int.MinValue || val > int.MaxValue)
				{
					throw new Stop(Stoppable_Fields.RANGECHECK);
				}
				result = (int) val;
			}
			else
			{
				result = ((NumberType) any).intValue();
			}
			ip.ostack.pushRef(new IntegerType(result));
		}

		internal static void cvr(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.STRING);
			double result = 0;
			if (any is StringType)
			{
				StringType s = (StringType) any;
				bool moreTokens = s.token(ip);
				if (!moreTokens)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, any.ToString());
				}
				Any token = ip.ostack.pop();
				Any post = ip.ostack.pop();
				if (!(token is NumberType))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "cvr " + any);
				}
				result = ((NumberType) token).realValue();
			}
			else
			{
				result = ((NumberType) any).realValue();
			}
			ip.ostack.pushRef(new RealType(result));
		}

		internal static void cvrs(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			int radix = ip.ostack.popInteger();
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			string result;
			if (radix == 10)
			{
				result = num.ToString();
			}
			else
			{
				result = Convert.ToString(num.intValue(), radix);
			}
			ip.ostack.pushRef(new StringType(ip.vm, @string, result.ToCharArray()));
		}

		internal static void cvs(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			if (@string.length() <= 0)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			Any any = ip.ostack.pop();
			char[] val;
			switch (any.typeID())
			{
			case Types_Fields.INTEGER:
			case Types_Fields.REAL:
			case Types_Fields.BOOLEAN:
			case Types_Fields.NAME:
			case Types_Fields.OPERATOR:
			case Types_Fields.STRING:
				val = convert(any, @string.length());
				break;
			default:
				val = convert(NOSTRINGVAL, NOSTRINGVAL.Length);
				break;
			}
			ip.ostack.pushRef(new StringType(ip.vm, @string, val));
		}

		private static char[] convert(object obj, int n)
		{
			string sval = obj.ToString();
			char[] array = new char[Math.Min(n, sval.Length)];
			sval.CopyTo(0, array, 0, array.Length - 0);
			return array;
		}

		internal static void executeonly(Interpreter ip)
		{
			Any any = ip.ostack.top(Types_Fields.ARRAY | Types_Fields.FILE | Types_Fields.STRING);
			any.executeonly();
		}

		internal static void noaccess(Interpreter ip)
		{
			Any any = ip.ostack.top(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.FILE | Types_Fields.STRING);
			any.noaccess();
		}

		internal static void @readonly(Interpreter ip)
		{
			Any any = ip.ostack.top(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.FILE | Types_Fields.STRING);
			any.@readonly();
		}

		internal static void rcheck(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.FILE | Types_Fields.STRING);
			ip.ostack.pushRef(new BoolType(any.rcheck()));
		}

		internal static void wcheck(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.FILE | Types_Fields.STRING);
			ip.ostack.pushRef(new BoolType(any.wcheck()));
		}

	}

}