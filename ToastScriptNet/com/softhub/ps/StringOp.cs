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

	internal sealed class StringOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"string", "search", "anchorsearch", "length", "put", "get", "putinterval", "getinterval", "copy"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(StringOp));
		}

		internal static void @string(Interpreter ip)
		{
			int len = ip.ostack.popInteger();
			if (len < 0 || len > 65535)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			ip.ostack.pushRef(new StringType(ip.vm, len));
		}

		internal static void search(Interpreter ip)
		{
			StringType seek = (StringType) ip.ostack.pop(Types_Fields.STRING);
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			int seeklen = seek.length();
			int strlen = @string.length();
			if (seeklen > strlen)
			{
				ip.ostack.pushRef(@string);
				ip.ostack.push(BoolType.FALSE);
			}
			else
			{
				string seekString = seek.ToString();
				string s = new string(@string.toCharArray());
				int index = s.IndexOf(seekString, StringComparison.Ordinal);
				if (index >= 0)
				{
					int postindex = index + seeklen;
					Any post = @string.getinterval(postindex, strlen - postindex);
					ip.ostack.pushRef(post);
					ip.ostack.pushRef(seek);
					Any pre = @string.getinterval(0, index);
					ip.ostack.pushRef(pre);
					ip.ostack.push(BoolType.TRUE);
				}
				else
				{
					ip.ostack.pushRef(@string);
					ip.ostack.push(BoolType.FALSE);
				}
			}
		}

		internal static void anchorsearch(Interpreter ip)
		{
			StringType seek = (StringType) ip.ostack.pop(Types_Fields.STRING);
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			int seeklen = seek.length();
			int strlen = @string.length();
			if (seeklen > strlen)
			{
				ip.ostack.pushRef(@string);
				ip.ostack.push(BoolType.FALSE);
			}
			else
			{
				string seek1 = seek.ToString();
				string string1 = @string.ToString();
				if (string1.regionMatches(0, seek1, 0, seeklen))
				{
					ip.ostack.pushRef(new StringType(@string, seeklen, strlen - seeklen));
					ip.ostack.pushRef(seek);
					ip.ostack.push(BoolType.TRUE);
				}
				else
				{
					ip.ostack.pushRef(@string);
					ip.ostack.push(BoolType.FALSE);
				}
			}
		}

		internal static void length(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.STRING | Types_Fields.NAME);
			if (any is NameType)
			{
				ip.ostack.pushRef(new IntegerType(((NameType) any).length()));
			}
			else
			{
				ip.ostack.pushRef(new IntegerType(((Enumerable) any).length()));
			}
		}

		internal static void put(Interpreter ip)
		{
			Any val = ip.ostack.pop();
			Any key = ip.ostack.pop();
			Enumerable c = (Enumerable) ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.STRING);
			c.put(ip.vm, key, val);
		}

		internal static void get(Interpreter ip)
		{
			Any key = ip.ostack.pop();
			Enumerable c = (Enumerable) ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.STRING);
			Any val = c.get(key);
			if (val == null)
			{
				throw new Stop(Stoppable_Fields.UNDEFINED);
			}
			ip.ostack.pushRef(val);
		}

		internal static void putinterval(Interpreter ip)
		{
			Any val = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			IntegerType index = (IntegerType) ip.ostack.pop(Types_Fields.INTEGER);
			Interval c = (Interval) ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			c.putinterval(ip.vm, index.intValue(), val);
		}

		internal static void getinterval(Interpreter ip)
		{
			IntegerType count = (IntegerType) ip.ostack.pop(Types_Fields.INTEGER);
			IntegerType index = (IntegerType) ip.ostack.pop(Types_Fields.INTEGER);
			Interval c = (Interval) ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			ip.ostack.pushRef(c.getinterval(index.intValue(), count.intValue()));
		}

		internal static void copy(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.INTEGER | Types_Fields.ARRAY | Types_Fields.STRING | Types_Fields.DICT | Types_Fields.GSTATE);
			switch (any.typeCode())
			{
			case Types_Fields.INTEGER:
				copy(ip, ((IntegerType) any).intValue());
				break;
			case Types_Fields.ARRAY:
			case Types_Fields.STRING:
				copy(ip, (Interval) any);
				break;
			case Types_Fields.DICT:
				copy(ip, (DictType) any);
				break;
			case Types_Fields.GSTATE:
				throw new Stop(Stoppable_Fields.INTERNALERROR, "copy gstate: not yet implemented"); // TODO
			}
		}

		private static void copy(Interpreter ip, int n)
		{
			for (int i = 0; i < n; i++)
			{
				ip.ostack.push(ip.ostack.index(n - 1));
			}
		}

		private static void copy(Interpreter ip, Interval val2)
		{
			Interval val1 = (Interval) ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			if (((Any) val1).typeCode() != ((Any) val2).typeCode())
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			val2.putinterval(ip.vm, 0, (Any) val1);
			ip.ostack.push((Any) val2);
		}

		private static void copy(Interpreter ip, DictType dict2)
		{
			DictType dict1 = (DictType) ip.ostack.pop(Types_Fields.DICT);
			ip.ostack.pushRef(dict1.copyTo(ip.vm, dict2));
		}

	}

}