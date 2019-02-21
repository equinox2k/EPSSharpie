using System.Text;

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

	public class System.Collections.Stack : Stoppable, Types
	{

		protected internal int size;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal int count_Renamed;
		protected internal Any[] array;
		private int bottom;

		/// <summary>
		/// Construct the stack. </summary>
		/// <param name="size"> the initial size </param>
		public Stack(int size)
		{
			init(size);
		}

		/// <summary>
		/// Initialize the stack. </summary>
		/// <param name="size"> the initial size </param>
		protected internal virtual void init(int size)
		{
			this.size = size;
			this.count_Renamed = 0;
			this.array = new Any[size];
		}

		/// <summary>
		/// Resize the stack. </summary>
		/// <param name="newSize"> the new size of stack </param>
		protected internal virtual void resize(int newSize)
		{
			throw new Stop(overflow(), ToString());
		}

		/// <summary>
		/// Set the line number for the topmost element. </summary>
		/// <param name="any"> the parent object </param>
		public virtual Any LineNo
		{
			set
			{
				array[count_Renamed - 1].LineNo = value;
			}
		}

		/// <summary>
		/// Duplicate topmost element. </summary>
		/// <returns> the copy of the topmost element </returns>
		public virtual Any dup()
		{
			if (count_Renamed <= 0)
			{
				throw new Stop(underflow(), ToString());
			}
			if (count_Renamed >= size)
			{
				resize(size * 2);
			}
			try
			{
				Any any = (Any) array[count_Renamed - 1].clone();
				array[count_Renamed++] = any;
				return any;
			}
			catch (CloneNotSupportedException)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR);
			}
		}

		/// <summary>
		/// Push a copy of the parameter onto stack. </summary>
		/// <param name="any"> the element to push </param>
		public virtual void push(Any any)
		{
			if (count_Renamed >= size)
			{
				resize(size * 2);
			}
			try
			{
				array[count_Renamed++] = (Any) any.clone();
			}
			catch (CloneNotSupportedException)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "Stack.push");
			}
		}

		/// <summary>
		/// Push an element by reference. Usually this should
		/// only be used, if the element is moved from one stack
		/// to another, or if the object is newly created. </summary>
		/// <param name="any"> the element to push </param>
		public virtual void pushRef(Any any)
		{
			if (count_Renamed >= size)
			{
				resize(size * 2);
			}
			array[count_Renamed++] = any;
		}

		/// <summary>
		/// Set the bottom marker.
		/// </summary>
		public virtual void setBottom()
		{
			bottom = count_Renamed;
		}

		/// <summary>
		/// Clear the stack.
		/// </summary>
		public virtual void clear()
		{
			remove(count_Renamed - bottom);
		}

		/// <summary>
		/// Pop the stack and return the topmost value. </summary>
		/// <returns> a value of any type </returns>
		public virtual Any pop()
		{
			if (count_Renamed <= bottom)
			{
				throw new Stop(underflow(), ToString());
			}
			return array[--count_Renamed];
		}

		/// <summary>
		/// Pop the stack and return the topmost value. </summary>
		/// <param name="the"> expected data type mask </param>
		/// <returns> a value of any type </returns>
		public virtual Any pop(int type)
		{
			if (count_Renamed <= bottom)
			{
				throw new Stop(underflow(), ToString());
			}
			Any any = array[--count_Renamed];
			if ((any.typeCode() & type) == 0)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "pop: " + any.typeName());
			}
			return any;
		}

		/// <returns> a value of type IntegerType </returns>
		public virtual int popInteger()
		{
			return ((IntegerType) pop(Types_Fields.INTEGER)).intValue();
		}


		/// <returns> a value of type NumberType </returns>
		public virtual double popNumber()
		{
			return ((NumberType) pop(Types_Fields.NUMBER)).realValue();
		}

		/// <returns> a value of type BoolType </returns>
		public virtual bool popBoolean()
		{
			return ((BoolType) pop(Types_Fields.BOOLEAN)).booleanValue();
		}

		/// <returns> a value of type String </returns>
		public virtual string popString()
		{
			return pop(Types_Fields.STRING | Types_Fields.NAME).ToString();
		}

		/// <summary>
		/// Get a copy of the top element of stack, leaving
		/// the stack unchanged. </summary>
		/// <returns> the element at top of stack </returns>
		public virtual Any top()
		{
			if (count_Renamed <= 0)
			{
				throw new Stop(underflow(), ToString());
			}
			return array[count_Renamed - 1];
		}

		/// <summary>
		/// Get a copy of the top element of stack, leaving
		/// the stack unchanged. </summary>
		/// <param name="type"> the expected type </param>
		/// <returns> the element at top of stack </returns>
		public virtual Any top(int type)
		{
			if (count_Renamed <= 0)
			{
				throw new Stop(underflow(), ToString());
			}
			Any any = array[count_Renamed - 1];
			if ((any.typeCode() & type) == 0)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "top: " + any.typeName());
			}
			return any;
		}

		/// <summary>
		/// Access the elements relative to top of stack. </summary>
		/// <param name="the"> index </param>
		/// <returns> the element at index </returns>
		public virtual Any index(int index)
		{
			if (index < 0 || index >= count_Renamed)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			return array[count_Renamed - index - 1];
		}

		/// <param name="the"> index </param>
		/// <param name="type"> the expected type </param>
		/// <returns> the element at index </returns>
		public virtual Any index(int index, int type)
		{
			if (index < 0 || index >= count_Renamed)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			Any any = array[count_Renamed - index - 1];
			if ((any.typeCode() & type) == 0)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "index: " + any.typeName());
			}
			return any;
		}

		/// <summary>
		/// Access the elements like an array, indexed begining
		/// at bottom of stack. </summary>
		/// <param name="the"> index </param>
		/// <returns> the element at index </returns>
		public virtual Any elementAt(int index)
		{
			if (index < 0 || index >= count_Renamed)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK, ToString());
			}
			return array[index];
		}

		/// <summary>
		/// Exchange the two topmost elements.
		/// </summary>
		public virtual void exch()
		{
			if (count_Renamed < 2)
			{
				throw new Stop(underflow(), ToString());
			}
			int index1 = count_Renamed - 1;
			int index2 = count_Renamed - 2;
			Any tmp = array[index1];
			array[index1] = array[index2];
			array[index2] = tmp;
		}

		/// <summary>
		/// Roll the stack. </summary>
		/// <param name="n"> the number of elements to roll </param>
		/// <param name="j"> the number of rolls </param>
		public virtual void roll(int n, int j)
		{
			if (count_Renamed < n)
			{
				throw new Stop(underflow(), ToString());
			}
			if (n < 0)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK, ToString());
			}
			if (j != 0 && n != 0)
			{
				// TODO: this could be improved!
				int offset = count_Renamed - n;
				if (j > 0)
				{
					while (j-- > 0)
					{
						Any tmp = array[count_Renamed - 1];
						for (int i = offset + n - 1; i > offset; i--)
						{
							array[i] = array[i - 1];
						}
						array[offset] = tmp;
					}
				}
				else
				{
					while (j++ < 0)
					{
						Any tmp = array[offset];
						for (int i = offset; i < count_Renamed - 1; i++)
						{
							array[i] = array[i + 1];
						}
						array[count_Renamed - 1] = tmp;
					}
				}
			}
		}

		/// <returns> the number of elements on the stack </returns>
		public virtual int count()
		{
			return count_Renamed;
		}

		/// <param name="typecode"> the type we are looking for </param>
		/// <returns> the number of elements to topmost type </returns>
		public virtual int countto(int typecode)
		{
			int n = count_Renamed - 1;
			int m = n;
			while (m >= 0 && (array[m].typeCode() & typecode) == 0)
			{
				m--;
			}
			return n - m;
		}

		/// <returns> the number of elements to topmost mark </returns>
		public virtual int counttomark()
		{
			int m = countto(Types_Fields.MARK);
			if (m >= count_Renamed)
			{
				throw new Stop(Stoppable_Fields.UNMATCHEDMARK, ToString());
			}
			return m;
		}

		/// <summary>
		/// Pop the stack until we find a mark.
		/// </summary>
		public virtual void cleartomark()
		{
			int m = countto(Types_Fields.MARK);
			if (m >= count_Renamed)
			{
				throw new Stop(Stoppable_Fields.UNMATCHEDMARK);
			}
			remove(m + 1);
		}

		/// <summary>
		/// Remove n elements from the stack. </summary>
		/// <param name="n"> the number of elements to remove from stack </param>
		public virtual void remove(int n)
		{
			for (int i = 0; i < n; i++)
			{
				array[--count_Renamed] = null;
			}
		}

		/// <summary>
		/// Check stack for invalid objects. </summary>
		/// <param name="the"> current save level </param>
		public virtual void check(int level)
		{
			for (int i = count_Renamed - 1; i >= 0; i--)
			{
				if (array[i] is CompositeType)
				{
					CompositeType comp = (CompositeType) array[i];
					if (comp.SaveLevel > level)
					{
						throw new Stop(Stoppable_Fields.INVALIDRESTORE, comp.ToString());
					}
				}
			}
		}

		/// <returns> the error code for stack overflows </returns>
		protected internal virtual int overflow()
		{
			return Stoppable_Fields.STACKOVERFLOW;
		}

		/// <returns> the error code for stack underflows </returns>
		protected internal virtual int underflow()
		{
			return Stoppable_Fields.STACKUNDERFLOW;
		}

		/// <summary>
		/// Print string representation of object on the stack. </summary>
		/// <param name="out"> the output stream </param>
		public virtual void print(PrintStream @out)
		{
			for (int i = 0; i < count_Renamed; i++)
			{
				string val = escapeSpecialChars(array[i].ToString());
				string type = array[i].typeName();
				@out.println("" + (count_Renamed - i - 1) + ": " + val + " <" + type + ">");
			}
		}

		/// <returns> string representation using escape sequences </returns>
		private static string escapeSpecialChars(string s)
		{
			StringBuilder buf = new StringBuilder();
			int i, n = s.Length;
			for (i = 0; i < n; i++)
			{
				char c = s[i];
				switch (c)
				{
				case '\t':
					buf.Append("\\t");
					break;
				case '\r':
					buf.Append("\\r");
					break;
				case '\n':
					buf.Append("\\n");
					break;
				case '\f':
					buf.Append("\\f");
					break;
				case '\b':
					buf.Append("\\b");
					break;
				case '\0':
					buf.Append("\\0");
					break;
				default:
					buf.Append(c);
					break;
				}
			}
			return buf.ToString();
		}

		/// <returns> string representation </returns>
		public override string ToString()
		{
			return "stack<" + size + ">";
		}

	}

}