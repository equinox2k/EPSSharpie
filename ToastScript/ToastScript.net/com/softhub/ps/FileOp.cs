﻿using System;

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

	using Codec = com.softhub.ps.filter.Codec;
	using CharStream = com.softhub.ps.util.CharStream;

	internal sealed class FileOp : Stoppable, Types
	{

		private const string PACKAGE = "com.softhub.ps";

		private static readonly string[] OPNAMES = new string[] {"file", "currentfile", "flush", "flushfile", "bytesavailable", "resetfile", "closefile", "read", "write", "readstring", "writestring", "readhexstring", "writehexstring", "readline", "run", "token", "status", "filter", "deletefile", "renamefile", "fileposition", "setfileposition"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(FileOp));
		}

		internal static void file(Interpreter ip)
		{
			StringType access = (StringType) ip.ostack.pop(Types_Fields.STRING);
			StringType name = (StringType) ip.ostack.pop(Types_Fields.STRING);
			if (access.length() != 1)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}
			int mode, m = access.get(0);
			switch (m)
			{
			case 'r':
				mode = FileType.READ_MODE;
				break;
			case 'w':
				mode = FileType.WRITE_MODE;
				break;
			default:
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}
			FileType file;
			if (name.Equals("%stdin"))
			{
				if (m != 'r')
				{
					throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
				}
				file = ip.stdin;
			}
			else if (name.Equals("%stdout"))
			{
				if (m != 'w')
				{
					throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
				}
				file = ip.stdout;
			}
			else if (name.Equals("%stderr"))
			{
				if (m != 'w')
				{
					throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
				}
				file = ip.stderr;
			}
			else if (name.Equals("%lineedit"))
			{
				if (m != 'r')
				{
					throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
				}
				file = ip.lineedit;
			}
			else
			{
				FileType currentFile = getCurrentFile(ip);
				file = new FileType(ip.vm, currentFile, ip.CodeBase, name, mode);
			}
			ip.ostack.pushRef(file);
		}

		internal static void currentfile(Interpreter ip)
		{
			ip.ostack.push(getCurrentFile(ip));
			ip.ostack.top().cvlit();
		}

		internal static FileType getCurrentFile(Interpreter ip)
		{
			int n = ip.estack.countto(Types_Fields.FILE);
			FileType file;
			if (n >= ip.estack.count())
			{
				file = ip.stdin;
			}
			else
			{
				file = (FileType) ip.estack.index(n);
			}
			return file;
		}

		internal static void flush(Interpreter ip)
		{
			ip.stdout.flush();
		}

		internal static void flushfile(Interpreter ip)
		{
			((FileType) ip.ostack.pop(Types_Fields.FILE)).flush();
		}

		internal static void bytesavailable(Interpreter ip)
		{
			int count = ((FileType) ip.ostack.pop(Types_Fields.FILE)).bytesavailable();
			ip.ostack.pushRef(new IntegerType(count));
		}

		internal static void resetfile(Interpreter ip)
		{
			((FileType) ip.ostack.pop(Types_Fields.FILE)).resetfile(ip.CodeBase);
		}

		internal static void closefile(Interpreter ip)
		{
			((FileType) ip.ostack.pop(Types_Fields.FILE)).close();
		}

		internal static void read(Interpreter ip)
		{
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			int c = file.read();
			if (c >= 0)
			{
				ip.ostack.pushRef(new IntegerType(c));
				ip.ostack.push(BoolType.TRUE);
			}
			else
			{
				ip.ostack.push(BoolType.FALSE);
			}
		}

		internal static void write(Interpreter ip)
		{
			int c = ip.ostack.popInteger();
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			file.write(c);
		}

		internal static void readstring(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			StringType[] result = new StringType[1];
			bool not_eof = file.read(ip.vm, s, result);
			ip.ostack.pushRef(result[0]);
			ip.ostack.pushRef(new BoolType(not_eof));
		}

		internal static void writestring(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			file.write(s);
		}

		internal static void readhexstring(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			StringType[] result = new StringType[1];
			bool not_eof = file.readhex(ip.vm, s, result);
			ip.ostack.pushRef(result[0]);
			ip.ostack.pushRef(new BoolType(not_eof));
		}

		internal static void writehexstring(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			file.writehex(s);
		}

		internal static void readline(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			StringType[] result = new StringType[1];
			bool not_eof = file.readline(ip.vm, s, result);
			ip.ostack.pushRef(result[0]);
			ip.ostack.pushRef(new BoolType(not_eof));
		}

		internal static void run(Interpreter ip)
		{
			ip.ostack.top(Types_Fields.STRING);
			ip.ostack.pushRef(new StringType(ip.vm, "r"));
			ip.estack.pushRef(ip.systemdict.get("exec"));
			ip.estack.pushRef(ip.systemdict.get("cvx"));
			ip.estack.pushRef(ip.systemdict.get("file"));
		}

		internal static void token(Interpreter ip)
		{
			CharSequenceType cs = (CharSequenceType) ip.ostack.pop(Types_Fields.FILE | Types_Fields.STRING);
			ip.ostack.pushRef(new BoolType(cs.token(ip)));
		}

		internal static void status(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.FILE | Types_Fields.STRING);
			if (any is FileType)
			{
				FileType file = (FileType) any;
				ip.ostack.pushRef(new BoolType(!file.Closed));
			}
			else
			{
				StringType name = (StringType) any;
				File file = new File(name.ToString());
				if (file.File)
				{
					int len = (int) file.length();
					ip.ostack.pushRef(new IntegerType((len + 1023) / 1024));
					ip.ostack.pushRef(new IntegerType(len));
					ip.ostack.pushRef(new IntegerType((int) file.lastModified()));
					ip.ostack.pushRef(new IntegerType((int) file.lastModified())); // TODO: date created!
					ip.ostack.pushRef(new BoolType(true));
				}
				else
				{
					ip.ostack.pushRef(new BoolType(false));
				}
			}
		}

		internal static void fileposition(Interpreter ip)
		{
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			int pos = file.FilePosition;
			ip.ostack.pushRef(new IntegerType(pos));
		}

		internal static void setfileposition(Interpreter ip)
		{
			int pos = ip.ostack.popInteger();
			FileType file = (FileType) ip.ostack.pop(Types_Fields.FILE);
			file.FilePosition = pos;
		}

		internal static void filter(Interpreter ip)
		{
			string name = ip.ostack.pop(Types_Fields.STRING | Types_Fields.NAME).ToString();
			int len = name.Length;
			string codecName;
			int mode;
			if (name.EndsWith("Encode", StringComparison.Ordinal))
			{
				codecName = name.Substring(0, len - "Encode".Length);
				mode = com.softhub.ps.util.CharStream_Fields.WRITE_MODE;
			}
			else if (name.EndsWith("Decode", StringComparison.Ordinal))
			{
				codecName = name.Substring(0, len - "Decode".Length);
				mode = com.softhub.ps.util.CharStream_Fields.READ_MODE;
			}
			else
			{
				throw new Stop(Stoppable_Fields.UNDEFINED, name);
			}
			Codec codec = null;
			try
			{
				Type clazz = Type.GetType(PACKAGE + ".filter." + codecName + "Codec");
				codec = (Codec) System.Activator.CreateInstance(clazz);
			}
			catch (Exception)
			{
				throw new Stop(Stoppable_Fields.UNDEFINED, codecName + " not found");
			}
			// pop optional parameters
			Type[] types = codec.OptionalParameterTypes;
			if (types != null)
			{
				int i, n = types.Length;
				object[] parameter = new object[n];
				for (i = n - 1; i >= 0; i--)
				{
					parameter[i] = ip.ostack.pop().cvj();
				}
				try
				{
					codec.OptionalParameters = parameter;
				}
				catch (System.InvalidCastException ex)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, ex.Message);
				}
			}
			Any any = ip.ostack.pop(Types_Fields.FILE | Types_Fields.STRING | Types_Fields.ARRAY);
			if (any is StringType)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "not yet implemented: " + any.typeName());
			}
			if (any is ArrayType)
			{
				throw new Stop(Types_Fields.ARRAY, "not yet implemented: " + any.typeName());
			}
			FileType file = (FileType) any;
			ip.ostack.pushRef(new FilterType(ip.vm, file, codec, mode));
		}

		internal static void deletefile(Interpreter ip)
		{
			string fileName = ip.ostack.popString();
			File file = new File(fileName);
			try
			{
				if (!file.delete())
				{
					throw new Stop(Stoppable_Fields.UNDEFINEDFILENAME, fileName);
				}
			}
			catch (SecurityException)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS, fileName);
			}
		}

		internal static void renamefile(Interpreter ip)
		{
			string newName = ip.ostack.popString();
			string oldName = ip.ostack.popString();
			File oldFile = new File(oldName);
			File newFile = new File(newName);
			try
			{
				if (!oldFile.renameTo(newFile))
				{
					throw new Stop(Stoppable_Fields.UNDEFINEDFILENAME, oldName + " " + newName);
				}
			}
			catch (SecurityException)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS, oldName + " " + newName);
			}
		}

	}

}