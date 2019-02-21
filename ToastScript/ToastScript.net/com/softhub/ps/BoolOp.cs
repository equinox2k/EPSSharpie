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

	internal sealed class BoolOp : OperatorType
	{

		private const int AND = 1;
		private const int OR = 2;
		private const int XOR = 3;
		private const int NOT = 4;
		private const int BITSHIFT = 5;
		private const int EQ = 6;
		private const int NE = 7;
		private const int LT = 8;
		private const int LE = 9;
		private const int GT = 10;
		private const int GE = 11;

		internal static void install(Interpreter ip)
		{
			ip.installOp(new BoolOp(AND));
			ip.installOp(new BoolOp(OR));
			ip.installOp(new BoolOp(XOR));
			ip.installOp(new BoolOp(NOT));
			ip.installOp(new BoolOp(BITSHIFT));
			ip.installOp(new BoolOp(EQ));
			ip.installOp(new BoolOp(NE));
			ip.installOp(new BoolOp(LT));
			ip.installOp(new BoolOp(LE));
			ip.installOp(new BoolOp(GT));
			ip.installOp(new BoolOp(GE));
		}

		private int opcode;

		public BoolOp(int opcode) : base(toOpName(opcode))
		{
			this.opcode = opcode;
		}

		public override void exec(Interpreter ip)
		{
			switch (opcode)
			{
			case AND:
				and(ip);
				break;
			case OR:
				or(ip);
				break;
			case XOR:
				xor(ip);
				break;
			case NOT:
				not(ip);
				break;
			case BITSHIFT:
				bitshift(ip);
				break;
			case EQ:
				eq(ip);
				break;
			case NE:
				ne(ip);
				break;
			case LT:
				lt(ip);
				break;
			case LE:
				le(ip);
				break;
			case GT:
				gt(ip);
				break;
			case GE:
				ge(ip);
				break;
			default:
				throw new System.ArgumentException();
			}
		}

		private static string toOpName(int opcode)
		{
			switch (opcode)
			{
			case AND:
				return "and";
			case OR:
				return "or";
			case XOR:
				return "xor";
			case NOT:
				return "not";
			case BITSHIFT:
				return "bitshift";
			case EQ:
				return "eq";
			case NE:
				return "ne";
			case LT:
				return "lt";
			case LE:
				return "le";
			case GT:
				return "gt";
			case GE:
				return "ge";
			default:
				throw new System.ArgumentException();
			}
		}

		private static void and(Interpreter ip)
		{
			Any b = ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			Any a = ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			if (a.typeCode() != b.typeCode())
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			int result = ((NumberType) a).intValue() & ((NumberType) b).intValue();
			ip.ostack.pushRef(a is BoolType ? ((Any) new BoolType(result)) : ((Any) new IntegerType(result)));
		}

		private static void or(Interpreter ip)
		{
			Any b = ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			Any a = ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			if (a.typeCode() != b.typeCode())
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			int result = ((NumberType) a).intValue() | ((NumberType) b).intValue();
			ip.ostack.pushRef(a is BoolType ? ((Any) new BoolType(result)) : ((Any) new IntegerType(result)));
		}

		private static void xor(Interpreter ip)
		{
			Any b = ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			Any a = ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			if (a.typeCode() != b.typeCode())
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			int result = ((NumberType) a).intValue() ^ ((NumberType) b).intValue();
			ip.ostack.pushRef(a is BoolType ? ((Any) new BoolType(result)) : ((Any) new IntegerType(result)));
		}

		private static void not(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.BOOLEAN | Types_Fields.INTEGER);
			ip.ostack.pushRef(num is BoolType ? ((Any) new BoolType(!((BoolType) num).booleanValue())) : ((Any) new IntegerType(~num.intValue())));
		}

		private static void bitshift(Interpreter ip)
		{
			int b = ip.ostack.popInteger();
			int a = ip.ostack.popInteger();
			ip.ostack.pushRef(new IntegerType(b >= 0 ? a << b : (int)((uint)a >> -b)));
		}

		private static void eq(Interpreter ip)
		{
			Any b = ip.ostack.pop();
			Any a = ip.ostack.pop();
			if ((a is NumberType) && (b is NumberType))
			{
				ip.ostack.pushRef(new BoolType(((NumberType) a).realValue() == ((NumberType) b).realValue()));
			}
			else if ((a is StringType) && (b is StringType))
			{
				ip.ostack.pushRef(new BoolType(a.Equals(b)));
			}
			else
			{
				ip.ostack.pushRef(new BoolType(a.Equals(b)));
			}
		}

		private static void ne(Interpreter ip)
		{
			Any b = ip.ostack.pop();
			Any a = ip.ostack.pop();
			if ((a is NumberType) && (b is NumberType))
			{
				ip.ostack.pushRef(new BoolType(((NumberType) a).realValue() != ((NumberType) b).realValue()));
			}
			else if ((a is StringType) && (b is StringType))
			{
				ip.ostack.pushRef(new BoolType(!a.Equals(b)));
			}
			else
			{
				ip.ostack.pushRef(new BoolType(!a.Equals(b)));
			}
		}

		private static void lt(Interpreter ip)
		{
			Any b = ip.ostack.pop();
			Any a = ip.ostack.pop();
			if ((a is NumberType) && (b is NumberType))
			{
				ip.ostack.pushRef(new BoolType(((NumberType) a).realValue() < ((NumberType) b).realValue()));
			}
			else if ((a is StringType) && (b is StringType))
			{
				ip.ostack.pushRef(new BoolType(a.ToString().CompareTo(b.ToString()) < 0));
			}
			else
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
		}

		private static void le(Interpreter ip)
		{
			Any b = ip.ostack.pop();
			Any a = ip.ostack.pop();
			if ((a is NumberType) && (b is NumberType))
			{
				ip.ostack.pushRef(new BoolType(((NumberType) a).realValue() <= ((NumberType) b).realValue()));
			}
			else if ((a is StringType) && (b is StringType))
			{
				ip.ostack.pushRef(new BoolType(a.ToString().CompareTo(b.ToString()) <= 0));
			}
			else
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
		}

		private static void gt(Interpreter ip)
		{
			Any b = ip.ostack.pop();
			Any a = ip.ostack.pop();
			if ((a is NumberType) && (b is NumberType))
			{
				ip.ostack.pushRef(new BoolType(((NumberType) a).realValue() > ((NumberType) b).realValue()));
			}
			else if ((a is StringType) && (b is StringType))
			{
				ip.ostack.pushRef(new BoolType(a.ToString().CompareTo(b.ToString()) > 0));
			}
			else
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
		}

		private static void ge(Interpreter ip)
		{
			Any b = ip.ostack.pop();
			Any a = ip.ostack.pop();
			if ((a is NumberType) && (b is NumberType))
			{
				ip.ostack.pushRef(new BoolType(((NumberType) a).realValue() >= ((NumberType) b).realValue()));
			}
			else if ((a is StringType) && (b is StringType))
			{
				ip.ostack.pushRef(new BoolType(a.ToString().CompareTo(b.ToString()) >= 0));
			}
			else
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
		}

	}

}