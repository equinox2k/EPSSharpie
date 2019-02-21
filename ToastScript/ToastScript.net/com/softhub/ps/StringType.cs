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


	public class StringType : CharSequenceType, Enumerable, Interval
	{

		/// <summary>
		/// The shared data.
		/// </summary>
		private StringNode node;

		/// <summary>
		/// The start index of the (sub-) string.
		/// </summary>
		private int index;

		/// <summary>
		/// The length of the (sub-) string.
		/// </summary>
		private int count;

		/// <summary>
		/// The scanner. Non-null if executed or tokenized.
		/// </summary>
		private Scanner scanner;

		public StringType(VM vm, int len) : base(vm)
		{
			this.node = new StringNode(vm, len);
			this.count = len;
		}

		public StringType(VM vm, char[] val) : this(vm, val, val.Length)
		{
		}

		public StringType(VM vm, char[] val, int len) : this(vm, len)
		{
			this.node = new StringNode(vm, val, 0, len);
			this.count = len;
		}

		public StringType(VM vm, NameType val) : this(vm, val.toCharArray())
		{
		}

		public StringType(VM vm, string val) : base(vm)
		{
			this.count = val.Length;
			char[] chars = new char[count];
			val.CopyTo(0, chars, 0, count - 0);
			this.node = new StringNode(vm, chars, 0, count);
		}

		public StringType(VM vm, StringType @string, char[] val) : base(vm, @string)
		{
			this.node = @string.node;
			this.count = val.Length;
			if (count > @string.length())
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			node.putinterval(vm, 0, val, @string.index, count);
		}

		public StringType(StringType @string, int index, int count) : base(@string)
		{
			this.node = @string.node;
			this.index = index;
			this.count = count;
		}

		public override int typeCode()
		{
			return Types_Fields.STRING;
		}

		public override string typeName()
		{
			return "stringtype";
		}

		public override object cvj()
		{
			return ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getchar() throws java.io.IOException
		public override int getchar()
		{
			if (index < 0)
			{
				throw new IOException();
			}
			if (count >= 0)
			{
				count--;
			}
			if (count < 0)
			{
				return -1;
			}
			return node.get(index++);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ungetchar(int ch) throws java.io.IOException
		public override void ungetchar(int ch)
		{
			if (index <= 0)
			{
				throw new IOException();
			}
			if (count >= 0)
			{
				index--;
				count++;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void putchar(int c) throws java.io.IOException
		public override void putchar(int c)
		{
			throw new IOException();
		}

		public override void close()
		{
			scanner = null;
		}

		public virtual void put(VM vm, int m, int val)
		{
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			if (val < 0 || val > 65535)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			int i = m + index;
			if (i < 0 || m >= length())
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			node.put(vm, i, val);
		}

		public virtual int get(int m)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			int i = m + index;
			if (i < 0 || m >= length())
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			return node.get(i);
		}

		public virtual void putinterval(VM vm, int m, Any any)
		{
			if (!(wcheck() && any.rcheck()))
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			if (!(any is StringType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "bad param: " + any);
			}
			StringType @string = (StringType) any;
			int i = m + index;
			int slen = @string.length();
			if (i < 0 || m + slen > length())
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			node.putinterval(vm, @string.index, @string.node, i, slen);
		}

		public virtual Any getinterval(int offset, int n)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			int i = offset + index;
			if (i < 0 || i + n > index + count)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK, "len: " + length() + " i: " + i + " n: " + n + " index: " + index + " count: " + count);
			}
			return new StringType(this, i, n);
		}

		public virtual void put(VM vm, Any key, Any val)
		{
			if (!(key is IntegerType && val is IntegerType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			put(vm, ((IntegerType) key).intValue(), ((IntegerType) val).intValue());
		}

		public virtual Any get(Any key)
		{
			if (!(key is IntegerType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			return new IntegerType(get(((IntegerType) key).intValue()));
		}

		public virtual System.Collections.IEnumerator keys()
		{
			throw new Stop(Stoppable_Fields.INTERNALERROR);
		}

		public virtual System.Collections.IEnumerator elements()
		{
			return new EnumElements(this);
		}

		public override void exec(Interpreter ip)
		{
			if (Literal)
			{
				ip.ostack.push(this);
			}
			else
			{
				ip.estack.pushRef(this);
				if (scanner == null)
				{
					scanner = new Scanner();
				}
				if (ip.scan(this, scanner, true) == Scanner.EOF)
				{
					ip.estack.pop();
					scanner = null;
				}
			}
		}

		/// <summary>
		/// Read a token from this string and push it onto the operand stack. </summary>
		/// <param name="ip"> the ps-interpreter </param>
		/// <returns> true if there are more tokens </returns>
		public override bool token(Interpreter ip)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			if (scanner == null)
			{
				scanner = new Scanner();
			}
			int type = ip.scan(this, scanner, false);
			if (type != Scanner.EOF)
			{
				ip.ostack.pushRef(this);
				ip.ostack.exch();
			}
			return type != Scanner.EOF;
		}

		/// <returns> the string length </returns>
		public virtual int length()
		{
			return count >= 0 ? count : 0;
		}

		/// <summary>
		/// Test if two strings are equal by comparing each character. </summary>
		/// <param name="obj"> the other string object </param>
		/// <returns> true if equal; false otherwise </returns>
		public override bool Equals(object obj)
		{
			if (obj is StringType)
			{
				return node.Equals(((StringType) obj).toCharArray(), index, length());
			}
			if (obj is NameType)
			{
				return node.Equals(((NameType) obj).toCharArray(), index, length());
			}
			if (obj is string)
			{
				return node.Equals(((string) obj).ToCharArray(), index, length());
			}
			return false;
		}

		/// <returns> an array of characters of the (sub-) string </returns>
		public virtual char[] toCharArray()
		{
			return node.toCharArray(index, length());
		}

		/// <returns> a string representation </returns>
		public override string ToString()
		{
			return node.ToString(index, length());
		}

		/// <returns> a hash code for this string
		/// 
		/// Note: Strings in PS are mutable, but if we
		///       use strings as keys in dictionaries,
		///       we convert them to names and this hash
		///       will not be used. </returns>
		public override int GetHashCode()
		{
			throw new Stop(Stoppable_Fields.INTERNALERROR, "attempt to use string in dict");
		}

		/// <returns> a the shared data's save level </returns>
		public override int SaveLevel
		{
			get
			{
				return node.SaveLevel;
			}
		}

		internal class EnumElements : System.Collections.IEnumerator, Stoppable
		{
			private readonly StringType outerInstance;


			internal int current;
			internal int length;

			internal EnumElements(StringType outerInstance)
			{
				this.outerInstance = outerInstance;
				this.length = outerInstance.length();
			}

			public virtual bool hasMoreElements()
			{
				return current < length;
			}

			public virtual object nextElement()
			{
				if (current >= length)
				{
					throw new Stop(Stoppable_Fields.RANGECHECK);
				}
				return new IntegerType(outerInstance.get(current++));
			}

		}

		internal class StringNode : Node
		{

			internal char[] val;

			internal StringNode(VM vm, int size) : base(vm)
			{
				val = new char[size];
			}

			internal StringNode(VM vm, char[] val, int index, int count) : this(vm, count)
			{
				Array.Copy(val, index, this.val, 0, count);
			}

			internal StringNode(VM vm, StringNode node) : base(vm, node)
			{
				val = new char[node.val.Length];
				Array.Copy(node.val, 0, val, 0, node.val.Length);
			}

			internal override void copy(Node node)
			{
				base.copy(node);
				((StringNode) node).val = val;
			}

			internal virtual void put(VM vm, int index, int c)
			{
				if (!vm.StringBug && checkLevel(vm))
				{
					new StringNode(vm, this);
				}
				val[index] = (char) c;
			}

			internal virtual void putinterval(VM vm, int srcIndex, StringNode node, int dstIndex, int count)
			{
				putinterval(vm, srcIndex, node.val, dstIndex, count);
			}

			internal virtual void putinterval(VM vm, int srcIndex, char[] src, int dstIndex, int count)
			{
				if (checkLevel(vm))
				{
					new StringNode(vm, this);
				}
				Array.Copy(src, srcIndex, this.val, dstIndex, count);
			}

			internal virtual int get(int index)
			{
				return val[index];
			}

			internal virtual int length()
			{
				return val.Length;
			}

			internal virtual bool Equals(char[] array, int index, int count)
			{
				if (array.Length != count)
				{
					return false;
				}
				for (int i = 0; i < count; i++)
				{
					if (val[index + i] != array[i])
					{
						return false;
					}
				}
				return true;
			}

			internal virtual char[] toCharArray(int index, int count)
			{
				char[] array = new char[count];
				Array.Copy(val, index, array, 0, count);
				return array;
			}

			internal virtual string ToString(int index, int count)
			{
				int i, n = index + count;
				for (i = index; i < n; i++)
				{
					if (val[i] == '\0')
					{
						count = i - index;
						break;
					}
				}
				return new string(val, index, count);
			}

			public override string ToString()
			{
				return ToString(0, val.Length);
			}

		}

	}

}