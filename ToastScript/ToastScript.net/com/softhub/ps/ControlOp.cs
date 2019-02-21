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

	using EExecCodecFactory = com.softhub.ps.filter.EExecCodecFactory;
	using Codec = com.softhub.ps.filter.Codec;

	internal sealed class ControlOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"stop", "exec", "eexec", "ifelse", "exit", "execstack"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(ControlOp));
			ip.installOp(new StoppedOp());
			ip.installOp(new IfOp());
			ip.installOp(new LoopOp());
			ip.installOp(new ForallOp());
			ip.installOp(new RepeatOp());
			ip.installOp(new ForOp());
		}

		internal static void stop(Interpreter ip)
		{
			int count = ip.estack.countto(Any.STOPPEDCONTEXT);
			if (count < ip.estack.count())
			{
				ip.estack.remove(count + 1);
				ip.ostack.push(BoolType.TRUE);
			}
			else
			{
				// this should never happen
				ip.estack.pushRef(new QuitOp());
			}
		}

		internal static void exec(Interpreter ip)
		{
			Any any = ip.ostack.pop();
			switch (any.typeID())
			{
			case Types_Fields.FILE:
			case Types_Fields.ARRAY:
			case Types_Fields.NAME:
			case Types_Fields.STRING:
			case Types_Fields.OPERATOR:
				if (any.Executable)
				{
					ip.estack.push(any);
					break;
				}
				// fall through
			default:
				ip.ostack.pushRef(any);
				break;
			}
		}

		internal static void eexec(Interpreter ip)
		{
			CharSequenceType cs = (CharSequenceType) ip.ostack.pop(Types_Fields.STRING | Types_Fields.FILE);
			if (cs is FilterType)
			{
				cs = ((FilterType) cs).SourceStream;
			}
			Codec codec;
			try
			{
				codec = EExecCodecFactory.createCodec(cs, FilterType.READ_MODE);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
			FilterType filter = new FilterType(ip.vm, cs, codec, FilterType.READ_MODE);
			filter.cvx();
			ip.estack.pushRef(new EExecActionOp(filter));
			ip.dstack.pushRef(ip.systemdict);
		}

		internal class EExecActionOp : OperatorType
		{

			internal FilterType filter;

			internal EExecActionOp(FilterType filter) : base("eexec")
			{
				this.filter = filter;
			}

			public override void exec(Interpreter ip)
			{
				if (filter.Closed)
				{
					if (ip.dstack.top() == ip.systemdict)
					{
						ip.dstack.pop();
					}
				}
				else
				{
					ip.estack.pushRef(this);
					ip.estack.pushRef(filter);
				}
			}

		}

		internal static void execstack(Interpreter ip)
		{
			ArrayType a = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			for (int i = 0; i < ip.estack.count(); i++)
			{
				a.put(ip.vm, i, (Any) ip.estack.elementAt(i));
			}
			ip.ostack.pushRef(a);
		}

		internal static void ifelse(Interpreter ip)
		{
			ArrayType b = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType a = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			if (a.Literal || b.Literal)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			bool cond = ip.ostack.popBoolean();
			ip.estack.pushRef(cond ? a : b);
		}

		internal static void exit(Interpreter ip)
		{
			int index = ip.estack.countto(Any.LOOPCONTEXT);
			if (index >= ip.estack.count())
			{
				throw new Stop(Stoppable_Fields.INVALIDEXIT);
			}
			Any context = ip.estack.index(index);
			if (context is StoppedOp)
			{
				ip.ostack.push(BoolType.TRUE);
			}
			ip.estack.remove(index + 1);
		}

		internal class StoppedOp : OperatorType
		{

			internal StoppedOp() : base("stopped")
			{
			}

			public override int typeCode()
			{
				return Types_Fields.OPERATOR | STOPPEDCONTEXT | LOOPCONTEXT;
			}

			public override void exec(Interpreter ip)
			{
				Any any = ip.ostack.pop();
				int type = any.typeCode();
				if (type == Types_Fields.ARRAY && any.Literal)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				ip.estack.pushRef(new StoppedActionOp());
				ip.estack.pushRef(any);
			}

		}

		internal sealed class StoppedActionOp : StoppedOp
		{

			internal StoppedActionOp() : base()
			{
			}

			public override void exec(Interpreter ip)
			{
				ip.ostack.push(BoolType.FALSE);
			}

		}

		internal class IfOp : OperatorType
		{

			internal IfOp() : base("if")
			{
			}

			public override void exec(Interpreter ip)
			{
				ArrayType a = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
				if (a.Literal)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				bool cond = ip.ostack.popBoolean();
				if (cond)
				{
					ip.estack.pushRef(a);
				}
			}

		}

		internal class LoopOp : OperatorType
		{

			internal LoopOp() : base("loop")
			{
			}

			public override int typeCode()
			{
				return Types_Fields.OPERATOR | LOOPCONTEXT;
			}

			public override void exec(Interpreter ip)
			{
				ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
				if (proc.Literal)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				ip.estack.pushRef(new LoopActionOp(proc));
			}

		}

		internal sealed class LoopActionOp : LoopOp
		{

			internal ArrayType proc;

			internal LoopActionOp(ArrayType proc) : base()
			{
				this.proc = proc;
			}

			public override void exec(Interpreter ip)
			{
				ip.estack.pushRef(this);
				ip.estack.push(proc);
			}

		}

		internal class ForallOp : OperatorType
		{

			internal ForallOp() : base("forall")
			{
			}

			public override int typeCode()
			{
				return Types_Fields.OPERATOR | LOOPCONTEXT;
			}

			public override void exec(Interpreter ip)
			{
				ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
				if (proc.Literal)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				Any any = (Any) ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.STRING);
				Enumerable container = (Enumerable) any;
				switch (any.typeID())
				{
				case Types_Fields.ARRAY:
				case Types_Fields.STRING:
					ip.estack.pushRef(new ArrayForallActionOp(proc, container));
					break;
				case Types_Fields.DICT:
					ip.estack.pushRef(new DictForallActionOp(proc, container));
					break;
				}
			}

		}

		internal sealed class ArrayForallActionOp : ForallOp
		{

			internal ArrayType proc;
			internal System.Collections.IEnumerator iterator;

			internal ArrayForallActionOp(ArrayType proc, Enumerable container) : base()
			{
				this.proc = proc;
				iterator = container.elements();
			}

			public override void exec(Interpreter ip)
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (iterator.hasMoreElements())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ip.ostack.pushRef((Any) iterator.nextElement());
					ip.estack.pushRef(this);
					ip.estack.push(proc);
				}
			}

		}

		internal sealed class DictForallActionOp : ForallOp
		{

			internal ArrayType proc;
			internal System.Collections.IEnumerator keyIterator;
			internal System.Collections.IEnumerator valIterator;

			internal DictForallActionOp(ArrayType proc, Enumerable container) : base()
			{
				this.proc = proc;
				keyIterator = container.keys();
				valIterator = container.elements();
			}

			public override void exec(Interpreter ip)
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (valIterator.hasMoreElements())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ip.ostack.pushRef((Any) keyIterator.nextElement());
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					ip.ostack.pushRef((Any) valIterator.nextElement());
					ip.estack.pushRef(this);
					ip.estack.push(proc);
				}
			}

		}

		internal class RepeatOp : OperatorType
		{

			internal RepeatOp() : base("repeat")
			{
			}

			public override int typeCode()
			{
				return Types_Fields.OPERATOR | LOOPCONTEXT;
			}

			public override void exec(Interpreter ip)
			{
				ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
				if (proc.Literal)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				int count = ip.ostack.popInteger();
				if (count < 0)
				{
					throw new Stop(Stoppable_Fields.RANGECHECK);
				}
				ip.estack.pushRef(new RepeatActionOp(proc, count));
			}

		}

		internal sealed class RepeatActionOp : RepeatOp
		{

			internal ArrayType proc;
			internal int count;
			internal int maxCount;

			internal RepeatActionOp(ArrayType proc, int maxCount) : base()
			{
				this.proc = proc;
				this.maxCount = maxCount;
			}

			public override void exec(Interpreter ip)
			{
				if (count++ < maxCount)
				{
					ip.estack.pushRef(this);
					ip.estack.push(proc);
				}
			}

		}

		internal class ForOp : OperatorType
		{

			internal ForOp() : base("for")
			{
			}

			public override int typeCode()
			{
				return Types_Fields.OPERATOR | LOOPCONTEXT;
			}

			public override void exec(Interpreter ip)
			{
				ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
				if (proc.Literal)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				NumberType maxcount = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType increment = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType initial = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				if (increment.floatValue() == 0)
				{
					throw new Stop(Stoppable_Fields.UNDEFINED);
				}
				ip.estack.pushRef(new ForActionOp(proc, initial, increment, maxcount));
			}

		}

		internal sealed class ForActionOp : ForOp
		{

			internal ArrayType proc;
			internal NumberType initial;
			internal NumberType increment;
			internal NumberType maxCount;

			internal ForActionOp(ArrayType proc, NumberType initial, NumberType increment, NumberType maxCount) : base()
			{
				this.proc = proc;
				this.initial = initial;
				this.increment = increment;
				this.maxCount = maxCount;
			}

			public override void exec(Interpreter ip)
			{
				if (increment.realValue() >= 0)
				{
					if (initial.realValue() <= maxCount.realValue())
					{
						ip.ostack.pushRef(initial);
						ip.estack.pushRef(this);
						ip.estack.push(proc);
						initial = ArithOp.add(initial, increment);
					}
					else
					{
						proc = null;
					}
				}
				else
				{
					if (initial.realValue() >= maxCount.realValue())
					{
						ip.ostack.pushRef(initial);
						ip.estack.pushRef(this);
						ip.estack.push(proc);
						initial = ArithOp.add(initial, increment);
					}
					else
					{
						proc = null;
					}
				}
			}

		}

		internal sealed class QuitOp : OperatorType
		{

			internal QuitOp() : base("quit")
			{
			}

			public override int typeCode()
			{
				return Types_Fields.OPERATOR | QUITCONTEXT;
			}

			public override void exec(Interpreter ip)
			{
				System.Console.WriteLine("Good bye!");
				int count = ip.estack.countto(QUITCONTEXT);
				if (count > 0)
				{
					ip.estack.remove(count);
				}
				else
				{
					ip.estack.remove(ip.estack.count());
				}
			}

		}

	}

}