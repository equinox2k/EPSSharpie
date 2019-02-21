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


	public class ArrayType : CompositeType, Enumerable, Interval
	{

		/// <summary>
		/// The shared value.
		/// </summary>
		private ArrayNode node;

		/// <summary>
		/// The start index of the array.
		/// </summary>
		private int index;

		/// <summary>
		/// The number of elements in the array.
		/// </summary>
		private int count;

		public ArrayType(VM vm, int size) : base(vm.Global)
		{
			node = new ArrayNode(vm, size);
			count = size;
		}

		public ArrayType(VM vm, int n, System.Collections.Stack stack) : this(vm, n)
		{
			int offset = stack.count() - n;
			for (int i = 0; i < n; i++)
			{
				put(vm, i, (Any) stack.elementAt(i + offset));
			}
		}

		private ArrayType(ArrayType array, int index, int count) : base(array)
		{
			this.node = array.node;
			this.index = index;
			this.count = count;
		}

		public override int typeCode()
		{
			return Types_Fields.ARRAY;
		}

		public override string typeName()
		{
			return Packed ? "packedarraytype" : "arraytype";
		}

		public override object cvj()
		{
			if (length() == 0)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "zero length");
			}
			object obj = get(0).cvj();
			if (obj is bool?)
			{
				return packBoolean();
			}
			if (obj is Number)
			{
				return packNumber();
			}
			if (obj is string)
			{
				return packString();
			}
			throw new Stop(Stoppable_Fields.TYPECHECK, obj + " not packable");
		}

		private bool[] packBoolean()
		{
			int i, n = length();
			bool[] result = new bool[n];
			for (i = 0; i < n; i++)
			{
				object obj = get(i).cvj();
				if (!(obj is bool?))
				{
					return null;
				}
				result[i] = ((bool?) obj).Value;
			}
			return result;
		}

		private object packNumber()
		{
			int i, n = length();
			Number[] array = new Number[n];
			for (i = 0; i < n; i++)
			{
				object obj = get(i).cvj();
				if (!(obj is Number))
				{
					return null;
				}
				array[i] = (Number) obj;
			}
			return array;
		}

		private string[] packString()
		{
			int i, n = length();
			string[] result = new string[n];
			for (i = 0; i < n; i++)
			{
				object obj = get(i).cvj();
				if (!(obj is string))
				{
					return null;
				}
				result[i] = obj.ToString();
			}
			return result;
		}

		public virtual void put(VM vm, int m, Any any)
		{
			if (!vm.InitialSaveLevel && Global && !any.Global)
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "put(array,global)");
			}
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "put(array,wcheck)");
			}
			int i = m + index;
			if (i < 0 || m >= count)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			node.put(vm, i, any);
		}

		public virtual Any get(int m)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "put(array,rcheck)");
			}
			int i = m + index;
			if (i < 0 || m >= count)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			return node.get(i);
		}

		public virtual void putinterval(VM vm, int m, Any any)
		{
			if (!(wcheck() && any.rcheck()))
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "putinterval(array,wcheck)");
			}
			if (!(any is ArrayType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "bad param: " + any);
			}
			ArrayType array = (ArrayType) any;
			int i = m + index;
			if (i < 0 || m + array.count > count)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			node.putinterval(vm, array.index, array.node, i, array.count, Global);
		}

		public virtual Any getinterval(int m, int n)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "getinterval(array,rcheck)");
			}
			int i = m + index;
			if (i < 0 || m + n > count)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			return new ArrayType(this, i, n);
		}

		public virtual void put(VM vm, Any key, Any val)
		{
			if (!(key is IntegerType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			put(vm, ((IntegerType) key).intValue(), val);
		}

		public virtual Any get(Any key)
		{
			if (!(key is IntegerType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			return get(((IntegerType) key).intValue());
		}

		public virtual ArrayType put(VM vm, AffineTransform xform)
		{
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "put(array,xform)");
			}
			if (length() != 6)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			if (!check(Types_Fields.NUMBER | Types_Fields.NULL))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			double[] m = new double[6];
			xform.getMatrix(m);
			node.put(vm, 0 + index, new RealType(m[0]));
			node.put(vm, 1 + index, new RealType(m[1]));
			node.put(vm, 2 + index, new RealType(m[2]));
			node.put(vm, 3 + index, new RealType(m[3]));
			node.put(vm, 4 + index, new RealType(m[4]));
			node.put(vm, 5 + index, new RealType(m[5]));
			return this;
		}

		internal virtual void bind(Interpreter ip)
		{
			if (!Bound)
			{
				int i, n = length();
				for (i = 0; i < n; i++)
				{
					Any any = get(i);
					if (any is ArrayType)
					{
						ArrayType array = (ArrayType) any;
						array.bind(ip);
					}
					else if (any.Executable)
					{
						Any val = ip.dstack.load(any);
						if (val != null && val is OperatorType)
						{
							node.put(ip.vm, i, val);
						}
					}
				}
				setBound();
			}
		}

		public virtual AffineTransform toTransform()
		{
			if (length() != 6)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			if (!check(Types_Fields.NUMBER))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			return new AffineTransform(((NumberType) node.get(0)).floatValue(), ((NumberType) node.get(1)).floatValue(), ((NumberType) node.get(2)).floatValue(), ((NumberType) node.get(3)).floatValue(), ((NumberType) node.get(4)).floatValue(), ((NumberType) node.get(5)).floatValue());
		}

		public virtual float[] toFloatArray()
		{
			int i, n = length();
			float[] array = new float[n];
			for (i = 0; i < n; i++)
			{
				array[i] = ((NumberType) node.get(i)).floatValue();
			}
			return array;
		}

		public virtual bool Matrix
		{
			get
			{
				return check(Types_Fields.NUMBER, 6);
			}
		}

		public virtual bool check(int typemask, int len)
		{
			if (length() != len)
			{
				return false;
			}
			for (int i = 0; i < len; i++)
			{
				Any any = node.get(i);
				if (!any.typeOf(typemask))
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool check(int typemask)
		{
			return check(typemask, length());
		}

		public virtual System.Collections.IEnumerator keys()
		{
			throw new Stop(Stoppable_Fields.INTERNALERROR, "ArrayType.keys");
		}

		public virtual System.Collections.IEnumerator elements()
		{
			return new EnumElements(this);
		}

		public override void exec(Interpreter ip)
		{
			if (Literal)
			{
				ip.ostack.pushRef(this);
			}
			else
			{
				if (count > 0)
				{
					Any any = node.get(index++);
					if (--count > 0)
					{
						ip.estack.pushRef(this);
					}
					if (any.Literal || any.typeOf(Types_Fields.ARRAY))
					{
						ip.ostack.push(any);
					}
					else
					{
						ip.estack.pushRef(any);
					}
				}
			}
		}

		public virtual int length()
		{
			return count;
		}

		public override bool Equals(object obj)
		{
			if (obj is ArrayType)
			{
				ArrayType a = (ArrayType) obj;
				return node == a.node && index == a.index && count == a.count;
			}
			return false;
		}

		public override string ToString()
		{
			bool literal = Literal;
			string val = literal ? "[ " : "{ ";
			val += node.ToString(index, count);
			val += literal ? "]" : "}";
			return val;
		}

		public override int GetHashCode()
		{
			return count + 43;
		}

		public override int SaveLevel
		{
			get
			{
				return node.SaveLevel;
			}
		}

		internal class EnumElements : System.Collections.IEnumerator, Stoppable
		{
			private readonly ArrayType outerInstance;

			public EnumElements(ArrayType outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			internal int count;

			public virtual bool hasMoreElements()
			{
				return count < outerInstance.length();
			}

			public virtual object nextElement()
			{
				if (count >= outerInstance.length())
				{
					throw new Stop(Stoppable_Fields.RANGECHECK);
				}
				return outerInstance.get(count++);
			}

		}

		internal class ArrayNode : Node
		{

			internal Any[] val;

			internal ArrayNode(VM vm, int size) : base(vm)
			{
				val = new Any[size];
			}

			internal ArrayNode(VM vm, ArrayNode node) : base(vm, node)
			{
				val = new Any[node.val.Length];
				Array.Copy(node.val, 0, val, 0, node.val.Length);
			}

			internal override void copy(Node node)
			{
				base.copy(node);
				((ArrayNode) node).val = val;
			}

			internal virtual void put(VM vm, int index, Any any)
			{
				if (checkLevel(vm))
				{
					new ArrayNode(vm, this);
				}
				val[index] = any;
			}

			internal virtual void putinterval(VM vm, int srcIndex, ArrayNode src, int dstIndex, int count, bool dstIsGlobal)
			{
				if (checkLevel(vm))
				{
					new ArrayNode(vm, this);
				}
				if (dstIsGlobal)
				{
					for (int i = 0; i < count; i++)
					{
						Any any = src.val[srcIndex++];
						if (!any.Global)
						{
							throw new Stop(Stoppable_Fields.INVALIDACCESS);
						}
						val[dstIndex++] = any;
					}
				}
				else
				{
					Array.Copy(src.val, srcIndex, val, dstIndex, count);
				}
			}

			internal virtual Any get(int index)
			{
				Any value = val[index];
				return value == null ? new NullType() : value;
			}

			internal virtual string ToString(int index, int count)
			{
				string s = "";
				int i, n = index + count;
				for (i = index; i < n; i++)
				{
					s += (val[i] + " ");
				}
				return s;
			}

			public override string ToString()
			{
				return ToString(0, val.Length);
			}

		}

	}

}