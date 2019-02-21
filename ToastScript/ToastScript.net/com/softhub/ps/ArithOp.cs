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
	/// 
	/// Arithmetic operators are used quite frequently, so we
	/// do some optimization on them. A switch block appears to
	/// be about 20% faster than Method.invoke but implementing
	/// each arithmetic operator in its own class would make it
	/// about 40% faster. Using one class for each operator
	/// increases the size of the jar file quite a bit. These are
	/// tradeoffs to be considered.
	/// </summary>

	internal sealed class ArithOp : OperatorType
	{

		private static readonly double LN10 = Math.Log(10);

		private static Random random = new Random(0);

		private const int ADD = 1;
		private const int SUB = 2;
		private const int MUL = 3;
		private const int DIV = 4;
		private const int IDIV = 5;
		private const int MOD = 6;
		private const int ABS = 7;
		private const int NEG = 8;
		private const int SQRT = 9;
		private const int CEILING = 10;
		private const int FLOOR = 11;
		private const int ROUND = 12;
		private const int TRUNCATE = 13;
		private const int EXP = 14;
		private const int LN = 15;
		private const int LOG = 16;
		private const int ATAN = 17;
		private const int SIN = 18;
		private const int COS = 19;
		private const int RAND = 20;
		private const int SRAND = 21;
		private const int RRAND = 22;

		internal static void install(Interpreter ip)
		{
			ip.installOp(new ArithOp(ADD));
			ip.installOp(new ArithOp(SUB));
			ip.installOp(new ArithOp(MUL));
			ip.installOp(new ArithOp(DIV));
			ip.installOp(new ArithOp(IDIV));
			ip.installOp(new ArithOp(MOD));
			ip.installOp(new ArithOp(ABS));
			ip.installOp(new ArithOp(NEG));
			ip.installOp(new ArithOp(SQRT));
			ip.installOp(new ArithOp(CEILING));
			ip.installOp(new ArithOp(FLOOR));
			ip.installOp(new ArithOp(ROUND));
			ip.installOp(new ArithOp(TRUNCATE));
			ip.installOp(new ArithOp(EXP));
			ip.installOp(new ArithOp(LN));
			ip.installOp(new ArithOp(LOG));
			ip.installOp(new ArithOp(ATAN));
			ip.installOp(new ArithOp(SIN));
			ip.installOp(new ArithOp(COS));
			ip.installOp(new ArithOp(RAND));
			ip.installOp(new ArithOp(SRAND));
			ip.installOp(new ArithOp(RRAND));
		}

		private int opcode;

		public ArithOp(int opcode) : base(toOpName(opcode))
		{
			this.opcode = opcode;
		}

		public override void exec(Interpreter ip)
		{
			switch (opcode)
			{
			case ADD:
				add(ip);
				break;
			case SUB:
				sub(ip);
				break;
			case MUL:
				mul(ip);
				break;
			case DIV:
				div(ip);
				break;
			case IDIV:
				idiv(ip);
				break;
			case MOD:
				mod(ip);
				break;
			case ABS:
				abs(ip);
				break;
			case NEG:
				neg(ip);
				break;
			case SQRT:
				sqrt(ip);
				break;
			case CEILING:
				ceiling(ip);
				break;
			case FLOOR:
				floor(ip);
				break;
			case ROUND:
				round(ip);
				break;
			case TRUNCATE:
				truncate(ip);
				break;
			case EXP:
				exp(ip);
				break;
			case LN:
				ln(ip);
				break;
			case LOG:
				log(ip);
				break;
			case ATAN:
				atan(ip);
				break;
			case SIN:
				sin(ip);
				break;
			case COS:
				cos(ip);
				break;
			case RAND:
				rand(ip);
				break;
			case SRAND:
				srand(ip);
				break;
			case RRAND:
				rrand(ip);
				break;
			default:
				throw new System.ArgumentException();
			}
		}

		private static string toOpName(int opcode)
		{
			switch (opcode)
			{
			case ADD:
				return "add";
			case SUB:
				return "sub";
			case MUL:
				return "mul";
			case DIV:
				return "div";
			case IDIV:
				return "idiv";
			case MOD:
				return "mod";
			case ABS:
				return "abs";
			case NEG:
				return "neg";
			case SQRT:
				return "sqrt";
			case CEILING:
				return "ceiling";
			case FLOOR:
				return "floor";
			case ROUND:
				return "round";
			case TRUNCATE:
				return "truncate";
			case EXP:
				return "exp";
			case LN:
				return "ln";
			case LOG:
				return "log";
			case ATAN:
				return "atan";
			case SIN:
				return "sin";
			case COS:
				return "cos";
			case RAND:
				return "rand";
			case SRAND:
				return "srand";
			case RRAND:
				return "rrand";
			default:
				throw new System.ArgumentException();
			}
		}

		internal static void add(Interpreter ip)
		{
			NumberType b = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			NumberType a = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			ip.ostack.pushRef(add(a, b));
		}

		internal static NumberType add(NumberType a, NumberType b)
		{
			if (a.Real || b.Real)
			{
				return new RealType(a.realValue() + b.realValue());
			}
			else
			{
				return new IntegerType(a.intValue() + b.intValue());
			}
		}

		internal static void sub(Interpreter ip)
		{
			NumberType b = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			NumberType a = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			ip.ostack.pushRef(sub(a, b));
		}

		internal static NumberType sub(NumberType a, NumberType b)
		{
			if (a.Real || b.Real)
			{
				return new RealType(a.realValue() - b.realValue());
			}
			else
			{
				return new IntegerType(a.intValue() - b.intValue());
			}
		}

		internal static void mul(Interpreter ip)
		{
			NumberType b = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			NumberType a = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			ip.ostack.pushRef(mul(a, b));
		}

		internal static NumberType mul(NumberType a, NumberType b)
		{
			if (a.Real || b.Real)
			{
				return new RealType(a.realValue() * b.realValue());
			}
			else
			{
				return new IntegerType(a.intValue() * b.intValue());
			}
		}

		internal static void div(Interpreter ip)
		{
			double b = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double a = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ip.ostack.pushRef(new RealType(a / b));
		}

		internal static void idiv(Interpreter ip)
		{
			int b = ip.ostack.popInteger();
			if (b == 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			int a = ip.ostack.popInteger();
			ip.ostack.pushRef(new IntegerType(a / b));
		}

		internal static void mod(Interpreter ip)
		{
			int b = ip.ostack.popInteger();
			if (b == 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			int a = ip.ostack.popInteger();
			ip.ostack.pushRef(new IntegerType(a % b));
		}

		internal static void abs(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			if (num.Real)
			{
				ip.ostack.pushRef(new RealType(Math.Abs(num.realValue())));
			}
			else
			{
				ip.ostack.pushRef(new IntegerType(Math.Abs(num.intValue())));
			}
		}

		internal static void neg(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			if (num.Real)
			{
				ip.ostack.pushRef(new RealType(-num.realValue()));
			}
			else
			{
				ip.ostack.pushRef(new IntegerType(-num.intValue()));
			}
		}

		internal static void sqrt(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			ip.ostack.pushRef(new RealType(Math.Sqrt(num.realValue())));
		}

		internal static void ceiling(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			if (num.Real)
			{
				ip.ostack.pushRef(new RealType(Math.Ceiling(num.realValue())));
			}
			else
			{
				ip.ostack.pushRef(num);
			}
		}

		internal static void floor(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			if (num.Real)
			{
				ip.ostack.pushRef(new RealType(Math.Floor(num.realValue())));
			}
			else
			{
				ip.ostack.pushRef(num);
			}
		}

		internal static void round(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			if (num.Real)
			{
				ip.ostack.pushRef(new RealType((long)Math.Round(num.realValue(), MidpointRounding.AwayFromZero)));
			}
			else
			{
				ip.ostack.pushRef(num);
			}
		}

		internal static void truncate(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			if (num.Real)
			{
				ip.ostack.pushRef(new RealType(Math.Round(num.realValue())));
			}
			else
			{
				ip.ostack.pushRef(num);
			}
		}

		internal static void exp(Interpreter ip)
		{
			double exponent = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double @base = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			try
			{
				ip.ostack.pushRef(new RealType(Math.Pow(@base, exponent)));
			}
			catch (ArithmeticException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
		}

		internal static void ln(Interpreter ip)
		{
			double num = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			if (num <= 0)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			ip.ostack.pushRef(new RealType(Math.Log(num)));
		}

		internal static void log(Interpreter ip)
		{
			double num = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			if (num <= 0)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			ip.ostack.pushRef(new RealType(Math.Log(num) / LN10));
		}

		internal static void atan(Interpreter ip)
		{
			double den = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double num = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			if (num == 0 && den == 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			ip.ostack.pushRef(new RealType(Math.Atan2(num, den) / Math.PI * 180));
		}

		internal static void sin(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			double val = num.realValue() * Math.PI / 180;
			ip.ostack.pushRef(new RealType(Math.Sin(val)));
		}

		internal static void cos(Interpreter ip)
		{
			NumberType num = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			double val = num.realValue() * Math.PI / 180;
			ip.ostack.pushRef(new RealType(Math.Cos(val)));
		}

		internal static void rand(Interpreter ip)
		{
			lock (typeof(ArithOp))
			{
				ip.ostack.pushRef(new IntegerType(Math.Abs(random.Next())));
			}
		}

		internal static void srand(Interpreter ip)
		{
			lock (typeof(ArithOp))
			{
				random.Seed = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).intValue();
			}
		}

		internal static void rrand(Interpreter ip)
		{
			lock (typeof(ArithOp))
			{
				ip.ostack.pushRef(new RealType(random.NextDouble()));
			}
		}

	}

}