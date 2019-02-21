using System;
using System.Collections;

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


	public class DictType : CompositeType, Enumerable
	{

		private int initialSize;
		private DictNode node;

		public DictType(VM vm, int size) : this(vm, size, false)
		{
		}

		public DictType(VM vm, int size, bool global) : base(global)
		{
			initialSize = size;
			node = new DictNode(vm, size);
		}

		public DictType(VM vm, int n, System.Collections.Stack stack) : this(vm, n / 2)
		{
			int offset = stack.count() - n;
			for (int i = offset; i < n + offset; i += 2)
			{
				put(vm, (Any) stack.elementAt(i), (Any) stack.elementAt(i + 1));
			}
		}

		public DictType(VM vm, DictType dict) : base(dict)
		{
			initialSize = dict.initialSize;
			node = new DictNode(vm, dict.node);
		}

		public virtual DictType copyTo(VM vm, DictType dict)
		{
			System.Collections.IEnumerator keyIter = keys();
			System.Collections.IEnumerator valIter = elements();
			while (keyIter.MoveNext())
			{
				Any key = (Any) keyIter.Current;
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Any val = (Any) valIter.nextElement();
				dict.put(vm, key, val);
			}
			return dict;
		}

		public override int typeCode()
		{
			return Types_Fields.DICT;
		}

		public override string typeName()
		{
			return "dicttype";
		}

		public virtual void put(VM vm, Any key, Any val)
		{
			if (!vm.InitialSaveLevel && Global && (!key.Global || !val.Global))
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "put(dict,global)" + key.Global + " " + val.Global);
			}
			node.put(vm, key, val);
		}

		public virtual void put(VM vm, string key, Any val)
		{
			put(vm, new NameType(key), val);
		}

		public virtual Any get(Any key)
		{
			return node.get(key);
		}

		public virtual Any get(Any key, int mask)
		{
			return node.get(key, mask);
		}

		public virtual Any get(string key)
		{
			return node.get(new NameType(key));
		}

		public virtual Any get(string key, int mask)
		{
			return node.get(new NameType(key), mask);
		}

		public virtual void remove(VM vm, Any key)
		{
			node.remove(vm, key);
		}

		public virtual bool known(Any key)
		{
			if (key is NullType)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			return node.get(key) != null;
		}

		public virtual bool known(string key)
		{
			return node.get(new NameType(key)) != null;
		}

		public virtual int length()
		{
			return node.length();
		}

		public virtual int maxlength()
		{
			int len = length();
			return len < initialSize ? initialSize : len;
		}

		public virtual System.Collections.IEnumerator elements()
		{
			return node.elements();
		}

		public virtual System.Collections.IEnumerator keys()
		{
			return node.keys();
		}

		public override void exec(Interpreter ip)
		{
			ip.ostack.push(this);
		}

		public override bool Equals(object obj)
		{
			return obj is DictType && ((DictType) obj).node == node;
		}

		/// <summary>
		/// Temporarily overwrite the access attributes. </summary>
		/// <returns> the access attributes </returns>
		public override int saveAccessFlags()
		{
			return node.saveAccessFlags();
		}

		/// <summary>
		/// Restore the access attributes. </summary>
		/// <param name="flags"> the access attributes </param>
		public override void restoreAccessFlags(int flags)
		{
			node.restoreAccessFlags(flags);
		}

		public override bool rcheck()
		{
			return node.rcheck();
		}

		public override bool wcheck()
		{
			return node.wcheck();
		}

		public override Any executeonly()
		{
			node.executeonly();
			return this;
		}

		public override Any noaccess()
		{
			node.noaccess();
			return this;
		}

		public override Any @readonly()
		{
			node.@readonly();
			return this;
		}

		public virtual void setFontAttr()
		{
			node.setFontAttr();
		}

		public virtual bool FontAttr
		{
			get
			{
				return node.FontAttr;
			}
		}

		public override string ToString()
		{
			return "dict<" + length() + ">";
		}

		public override int GetHashCode()
		{
			return initialSize + node.GetHashCode();
		}

		public override int SaveLevel
		{
			get
			{
				return node.SaveLevel;
			}
		}

		internal class DictNode : Node, Stoppable
		{

			internal const int MIN_DICT_SIZE = 8;

			internal const int RMODE_BIT = 1;
			internal const int WMODE_BIT = 2;
			internal const int XMODE_BIT = 4;
			internal const int FONT_BIT = 8;

			internal int flags = RMODE_BIT | WMODE_BIT | XMODE_BIT;

			internal Hashtable map;

			internal DictNode(VM vm, int size) : base(vm)
			{
				map = new Hashtable(Math.Max(size, MIN_DICT_SIZE));
			}

			internal DictNode(VM vm, DictNode node) : base(vm, node)
			{
				flags = node.flags;
				map = (Hashtable) node.map.clone();
			}

			internal override void copy(Node node)
			{
				base.copy(node);
				((DictNode) node).map = map;
			}

			internal virtual void put(VM vm, Any key, Any val)
			{
				if (checkLevel(vm))
				{
					new DictNode(vm, this);
				}
				if (!wcheck())
				{
					throw new Stop(Stoppable_Fields.INVALIDACCESS, "put " + key + " " + val + " " + this);
				}
				if (key is NullType)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				if (key is StringType)
				{
					map[new NameType((StringType) key)] = val;
				}
				else
				{
					map[key] = val;
				}
			}

			internal virtual Any get(Any key)
			{
				if (!rcheck())
				{
					throw new Stop(Stoppable_Fields.INVALIDACCESS, "get " + key);
				}
				if (key is NullType)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				if (key is StringType)
				{
					key = new NameType((StringType) key);
				}
				return (Any) map[key];
			}

			internal virtual Any get(Any key, int mask)
			{
				Any val = get(key);
				if (val == null || !val.typeOf(mask))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "get " + key);
				}
				return val;
			}

			internal virtual void remove(VM vm, Any key)
			{
				if (!wcheck())
				{
					throw new Stop(Stoppable_Fields.INVALIDACCESS, "remove " + key);
				}
				if (checkLevel(vm))
				{
					new DictNode(vm, this);
				}
				if (key is NullType)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				map.Remove(key);
			}

			internal virtual int length()
			{
				return map.Count;
			}

			public virtual System.Collections.IEnumerator elements()
			{
				return map.Values.GetEnumerator();
			}

			public virtual System.Collections.IEnumerator keys()
			{
				return map.Keys.GetEnumerator();
			}

			public override int GetHashCode()
			{
				return map.GetHashCode();
			}

			/// <summary>
			/// Temporarily overwrite the access attributes. </summary>
			/// <returns> the access attributes </returns>
			public virtual int saveAccessFlags()
			{
				int save = flags;
				flags |= RMODE_BIT | WMODE_BIT | XMODE_BIT;
				return save;
			}

			/// <summary>
			/// Restore the access attributes. </summary>
			/// <param name="flags"> the access attributes </param>
			public virtual void restoreAccessFlags(int flags)
			{
				this.flags = flags;
			}

			public virtual bool rcheck()
			{
				return (flags & RMODE_BIT) != 0;
			}

			public virtual bool wcheck()
			{
				return (flags & WMODE_BIT) != 0;
			}

			public virtual void executeonly()
			{
				flags &= XMODE_BIT | FONT_BIT;
			}

			public virtual void noaccess()
			{
				flags = 0;
			}

			public virtual void @readonly()
			{
				flags &= RMODE_BIT | XMODE_BIT | FONT_BIT;
			}

			public virtual void setFontAttr()
			{
				flags |= FONT_BIT;
			}

			public virtual bool FontAttr
			{
				get
				{
					return (flags & FONT_BIT) != 0;
				}
			}

			public override string ToString()
			{
				return "DictNode<" + map.Count + ", " + flags + ">";
			}

		}

	}

}