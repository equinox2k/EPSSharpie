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

	public class NameType : Any
	{

		private const int INITIAL_SIZE = 1024;

		private static Hashtable nameSpace = new Hashtable(INITIAL_SIZE);
		private static int nameIndex;

		private Node val;

		public NameType(string s)
		{
			val = load(s);
		}

		public NameType(char[] s, int len)
		{
			val = load(new string(s, 0, len));
		}

		public NameType(StringType s)
		{
			val = load(s.ToString());
			if (!s.Literal)
			{
				cvx();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override object clone()
		{
			reload(val);
			return base.clone();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void finalize() throws Throwable
		~NameType()
		{
			unload(val);
//JAVA TO C# CONVERTER NOTE: The base class finalizer method is automatically called in C#:
//			base.finalize();
		}

		public override int typeCode()
		{
			return Types_Fields.NAME;
		}

		public override string typeName()
		{
			return "nametype";
		}

		protected internal override bool Global
		{
			get
			{
				return true;
			}
		}

		public override void exec(Interpreter ip)
		{
			if (Literal)
			{
				ip.ostack.pushRef(this);
			}
			else
			{
				Any any = ip.dstack.load(this);
				if (any == null)
				{
					throw new Stop(Stoppable_Fields.UNDEFINED, ToString());
				}
				ip.estack.push(any);
				ip.estack.LineNo = this;
			}
		}

		public virtual int length()
		{
			return val.Name.Length;
		}

		public virtual char[] toCharArray()
		{
			return val.Name.ToCharArray();
		}

		public override string ToString()
		{
			return val.Name;
		}

		public override int GetHashCode()
		{
			return val.ID;
		}

		public override bool Equals(object obj)
		{
			if (obj is NameType)
			{
				return val == ((NameType) obj).val;
			}
			if (obj is StringType)
			{
				return obj.Equals(this);
			}
			if (obj is string)
			{
				return obj.Equals(val.Name);
			}
			return false;
		}

		internal static void printStatistics()
		{
			System.Console.WriteLine("Name Space: " + nameSpace.Count);
		}

		private static Node load(string s)
		{
			lock (typeof(NameType))
			{
				Node node = (Node) nameSpace[s];
				if (node != null)
				{
					node.incRef();
					return node;
				}
				node = new Node(s, nameIndex++);
				nameSpace[s] = node;
				return node;
			}
		}

		private static void reload(Node node)
		{
			lock (typeof(NameType))
			{
				node.incRef();
			}
		}

		private static void unload(Node node)
		{
			lock (typeof(NameType))
			{
				if (node.decRef() < 1)
				{
					nameSpace.Remove(node.Name);
				}
			}
		}

		internal class Node
		{

			internal string name;
			internal int id;
			internal int refCount = 1;

			internal Node(string name, int id)
			{
				this.name = name;
				this.id = id;
			}

			internal virtual string Name
			{
				get
				{
					return name;
				}
			}

			internal virtual int ID
			{
				get
				{
					return id;
				}
			}

			internal virtual void incRef()
			{
				refCount++;
			}

			internal virtual int decRef()
			{
				return --refCount;
			}

		}

	}

}