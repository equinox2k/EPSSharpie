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

	using CacheDevice = com.softhub.ps.device.CacheDevice;
	using Device = com.softhub.ps.device.Device;

	internal sealed class MiscOp : Stoppable, Types
	{

		private const string COPYRIGHT = "Copyright 1998 - 2005 by Christian Lehner";

		private static readonly string[] OPNAMES = new string[] {"handleerror", "executive", "usertime", "realtime", "setsystemparams", "currentsystemparams", "setuserparams", "currentuserparams", "showpage", "copypage", "erasepage", "print", "internaldict", "quit", "$$print", "$$println", "$$lineno", "$$currentlineno", "$$break"};

		private static readonly string[] STATUSDICT_OPNAMES = new string[] {"beginjob", "endjob"};

		private static DictType systemparams;
		private static DictType userparams;
		private static DictType internals;

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(MiscOp));
			ip.installOp(STATUSDICT_OPNAMES, typeof(MiscOp), ip.StatusDict);
			systemparams = new DictType(ip.vm, 10);
			userparams = new DictType(ip.vm, 10);
			internals = new DictType(ip.vm, 10);
		}

		internal static void handleerror(Interpreter ip)
		{
			// tell the device that an error occured
			DictType serror = (DictType) ip.systemdict.get("$error");
			string msg = serror != null ? serror.get("errorname").ToString() : "unknown";
			Device device = ip.GraphicsState.currentdevice();
			device.error(msg);
			// call the error handler defined in server script
			DictType errordict = (DictType) ip.systemdict.get("errordict");
			Any val = errordict.get("handleerror");
			if (val == null)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "handleerror undefined");
			}
			ip.estack.push(val);
		}

		internal static void executive(Interpreter ip)
		{
			// TODO: not yet implemented
		}

		internal static void usertime(Interpreter ip)
		{
			ip.ostack.push(new IntegerType(ip.usertime()));
		}

		internal static void realtime(Interpreter ip)
		{
			ip.ostack.push(new IntegerType(ip.usertime()));
		}

		internal static void setsystemparams(Interpreter ip)
		{
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			GraphicsState gstate = ip.GraphicsState;
			System.Collections.IEnumerator keys = dict.keys();
			System.Collections.IEnumerator vals = dict.elements();
			while (keys.MoveNext())
			{
				Any key = (Any) keys.Current;
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Any val = (Any) vals.nextElement();
				if (key is NameType)
				{
					NameType name = (NameType) key;
					if (name.Equals("vm.string.bug"))
					{
						if (val is BoolType)
						{
							ip.vm.StringBug = ((BoolType) val).booleanValue();
						}
					}
					else if (name.Equals("debug"))
					{
						debug(val, true);
					}
					else if (name.Equals("nodebug"))
					{
						debug(val, false);
					}
					else if (name.Equals("bitmapwidths"))
					{
						gstate.setRequestBitmapWidths(ip, val);
					}
					else if (name.Equals("systemfontdict"))
					{
						gstate.setSystemFonts(ip, val);
					}
				}
			}
		}

		internal static void currentsystemparams(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			StringType server = new StringType(ip.vm, ip.Server);
			systemparams.put(ip.vm, "server", server);
			systemparams.put(ip.vm, "bitmapwidths", new BoolType(gstate.RequestBitmapWidths));
			DictType sysfonts = gstate.SystemFonts;
			if (sysfonts != null)
			{
				systemparams.put(ip.vm, "systemfontdict", sysfonts);
			}
			systemparams.put(ip.vm, "vm.string.bug", new BoolType(ip.vm.StringBug));
			try
			{
				Properties prop = System.Properties;
				System.Collections.IEnumerator e = prop.propertyNames();
				while (e.MoveNext())
				{
					string key = (string) e.Current;
					string val = prop.getProperty(key);
					systemparams.put(ip.vm, key, new StringType(ip.vm, val));
				}
			}
			catch (SecurityException)
			{
			}
			systemparams.put(ip.vm, "copyright", new StringType(ip.vm, COPYRIGHT));
			Runtime runtime = Runtime.Runtime;
			systemparams.put(ip.vm, "free.memory", new IntegerType((int) runtime.freeMemory()));
			systemparams.put(ip.vm, "total.memory", new IntegerType((int) runtime.totalMemory()));
			ip.ostack.pushRef(systemparams);
		}

		internal static void setuserparams(Interpreter ip)
		{
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			System.Collections.IEnumerator keys = dict.keys();
			System.Collections.IEnumerator vals = dict.elements();
			while (keys.MoveNext())
			{
				Any key = (Any) keys.Current;
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Any val = (Any) vals.nextElement();
				if (key is NameType)
				{
					NameType name = (NameType) key;
					if (name.Equals("placeholder"))
					{
						// There are no userparams yet; the value of
						// "placeholder" would be set here.
					}
				}
			}
		}

		internal static void currentuserparams(Interpreter ip)
		{
			ip.ostack.pushRef(userparams);
		}

		internal static void internaldict(Interpreter ip)
		{
			Any magic = ip.ostack.pop(Types_Fields.INTEGER);
			// TODO: do we need to fail for wrong number?
			ip.ostack.pushRef(internals);
		}

		internal static void showpage(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			Device device = gstate.currentdevice();
			Any languagelevel = ip.dstack.load("languagelevel");
			int level = 0;
			if (languagelevel is IntegerType)
			{
				level = ((IntegerType) languagelevel).intValue();
			}
			if (level < 2)
			{
				int copies = 1;
				Any any = ip.dstack.load("#copies");
				if (any is IntegerType)
				{
					copies = ((IntegerType) any).intValue();
				}
				device.showpage();
				for (int i = 1; i < copies; i++)
				{
					device.copypage();
				}
			}
			else
			{
				device.showpage();
			}
			gstate.initgraphics();
		}

		internal static void copypage(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			Device device = gstate.currentdevice();
			device.copypage();
		}

		internal static void erasepage(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			Device device = gstate.currentdevice();
			device.erasepage();
		}

		internal static void setduplexmode(Interpreter ip)
		{
			bool mode = ip.ostack.popBoolean();
		}

		internal static void print(Interpreter ip)
		{
			ip.stdout.print((StringType) ip.ostack.pop(Types_Fields.STRING));
		}

		internal static void $$print(Interpreter ip)
		{
			System.Console.Write(ip.ostack.pop());
		}

		internal static void $$println(Interpreter ip)
		{
			System.Console.WriteLine(ip.ostack.pop());
		}

		internal static void $$lineno(Interpreter ip)
		{
			Any any = ip.ostack.pop();
			ip.ostack.pushRef(new IntegerType(any.LineNo));
		}

		internal static void $$currentlineno(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.lineno));
		}

		internal static void $$break(Interpreter ip)
		{
			try
			{
				sbyte[] cmd = new sbyte[64];
				do
				{
					print("break at " + ip.estack.top() + "\n");
					print("debug> ");
					int len = System.Console.Read(cmd);
					switch (cmd[0])
					{
					case 'o':
						printStack("ostack", ip, ip.ostack, cmd, len);
						break;
					case 'd':
						printStack("dstack", ip, ip.dstack, cmd, len);
						break;
					case 'e':
						printStack("estack", ip, ip.estack, cmd, len);
						break;
					case 's':
						Any any = ip.estack.pop();
						println("step: " + any);
						ip.estack.run(ip, any);
						break;
					case 'n':
						Any next = ip.estack.pop();
						if (next.typeID() == Types_Fields.ARRAY && next.Executable)
						{
							ArrayType proc = (ArrayType) next;
							println("proc: " + proc);
							int n = proc.length();
							if (n > 0)
							{
								if (n > 1)
								{
									ip.estack.pushRef(proc.getinterval(1, n - 1));
								}
								next = proc.get(0);
								println("next: " + next);
								if (next.typeID() == Types_Fields.ARRAY && next.Executable)
								{
									ip.ostack.pushRef(next);
								}
								else
								{
									next.exec(ip);
								}
							}
						}
						else
						{
							next.exec(ip);
						}
						break;
					case 'c':
						println("continue");
						cmd = null;
						break;
					case 'h':
						printDebugHelp();
						break;
					case 'q':
						println("quit");
						Environment.Exit(0);
						break;
					}
				} while (cmd != null);
			}
			catch (Exception ex)
			{
				println("error: " + ex);
			}
		}

		private static void printStack(string msg, Interpreter ip, System.Collections.Stack stack, sbyte[] cmd, int len)
		{
			int index = -1;
			string exe = null;
			if (len >= 2)
			{
				int n = len - 2, m;
				string s = StringHelper.NewString(cmd, 1, n);
				m = s.IndexOf(",", StringComparison.Ordinal);
				if (m < 0)
				{
					m = n;
				}
				else
				{
					exe = s.Substring(m + 1, n - (m + 1));
					s = s.Substring(0, m);
				}
				try
				{
					index = Convert.ToInt32(s);
				}
				catch (System.FormatException)
				{
				}
			}
			if (index >= 0)
			{
				Any any = stack.index(index);
				println(msg + "[" + index + "] " + any);
				if (!string.ReferenceEquals(exe, null))
				{
					ip.ostack.push(any);
					ip.estack.run(ip, (new StringType(ip.vm, exe)).cvx());
				}
			}
			else
			{
				print(msg + ":");
				if (stack.count() > 0)
				{
					println("");
					stack.print(System.out);
				}
				else
				{
					println(" empty");
				}
			}
		}

		private static void printDebugHelp()
		{
			println("[ode]<index> : print element at index");
			println("[ode]<index>,ps-code : push and exec ps-code");
			println("d0,{== ==} forall : dump current dictionary");
			println("o : dump operand stack");
			println("d : dump dictionary stack");
			println("e : dump execution stack");
			println("s : single step (over)");
			println("n : next step (into)");
			println("c : continue");
			println("h : help");
			println("q : quit");
		}

		private static void println(string msg)
		{
			print(msg + "\n");
		}

		private static void print(string msg)
		{
			System.Console.Write(msg);
		}

		internal static void beginjob(Interpreter ip)
		{
			Device device = ip.GraphicsState.currentdevice();
			device.beginJob();
		}

		internal static void endjob(Interpreter ip)
		{
			Device device = ip.GraphicsState.currentdevice();
			device.endJob();
			CacheDevice cachedevice = ip.GraphicsState.cachedevice();
			if (cachedevice != null)
			{
				cachedevice.clearCache();
			}
			if (ip.DebugMode)
			{
				NameType.printStatistics();
			}
			System.GC.Collect();
		}

		internal static void quit(Interpreter ip)
		{
			FileType.flushAllOpenFiles();
			Environment.Exit(0);
		}

		private static void debug(Any classname, bool state)
		{
			try
			{
				Type clazz = Type.GetType(classname.ToString());
				clazz.GetField("debug").setBoolean(clazz, state);
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine(ex);
			}
		}

	}

}