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

	public class Stop : Exception, Stoppable
	{

		/// <summary>
		/// The exception id.
		/// </summary>
		private int cause;

		/// <summary>
		/// Construct an exception which causes the interpreter to stop execution. </summary>
		/// <param name="cause"> the exception id </param>
		public Stop(int cause) : base()
		{
			this.cause = cause;
		}

		/// <summary>
		/// Construct an exception which causes the interpreter to stop execution. </summary>
		/// <param name="cause"> the exception id </param>
		/// <param name="msg"> the detail message </param>
		public Stop(int cause, string msg) : base(msg)
		{
			this.cause = cause;
		}

		/// <summary>
		/// Get the cause for this exception. </summary>
		/// <returns> the cause </returns>
		public virtual int ExceptionId
		{
			get
			{
				return cause;
			}
		}

		/// <summary>
		/// Handle the exception. </summary>
		/// <param name="ip"> the interpreter </param>
		public virtual void recorderror(Interpreter ip)
		{
			DictType serror = (DictType) ip.systemdict.get("$error");
			bool global = ip.vm.Global;
			if (global)
			{
				// reset VM allocation mode to local
				ip.vm.Global = false;
			}
			// capture the state
			if (serror != null)
			{
				Any cmd = ip.estack.top();
				string details = Message;
				// record information about error
				serror.put(ip.vm, "newerror", BoolType.TRUE);
				serror.put(ip.vm, "errorname", new NameType(Name));
				serror.put(ip.vm, "command", cmd);
				serror.put(ip.vm, "errorinfo", new ArrayType(ip.vm, 0)); // TODO: implement this
				serror.put(ip.vm, "ostack", new ArrayType(ip.vm, ip.ostack.count(), ip.ostack));
				serror.put(ip.vm, "estack", new ArrayType(ip.vm, ip.estack.count(), ip.estack));
				serror.put(ip.vm, "dstack", new ArrayType(ip.vm, ip.dstack.count(), ip.dstack));
				serror.put(ip.vm, "recordstacks", BoolType.TRUE); // TODO: Display PostScript
				serror.put(ip.vm, "binary", BoolType.FALSE); // TODO: Display PostScript
				serror.put(ip.vm, "details", new NameType(!string.ReferenceEquals(details, null) ? details : "none"));
				serror.put(ip.vm, "global", new BoolType(global));
				serror.put(ip.vm, "lineno", new IntegerType(cmd.LineNo));
				serror.put(ip.vm, "currentline", new IntegerType(ip.lineno));
				serror.put(ip.vm, "currentfilename", new StringType(ip.vm, FileOp.getCurrentFile(ip).Name));
			}
		}

		public virtual string Name
		{
			get
			{
				switch (cause)
				{
				case Stoppable_Fields.TYPECHECK:
					return "typecheck";
				case Stoppable_Fields.STACKUNDERFLOW:
					return "stackunderflow";
				case Stoppable_Fields.STACKOVERFLOW:
					return "stackoverflow";
				case Stoppable_Fields.EXSTACKOVERFLOW:
					return "execstackoverflow";
				case Stoppable_Fields.DICTSTACKOVERFLOW:
					return "dictstackoverflow";
				case Stoppable_Fields.DICTSTACKUNDERFLOW:
					return "dictstackunderflow";
				case Stoppable_Fields.UNDEFINED:
					return "undefined";
				case Stoppable_Fields.UNDEFINEDRESULT:
					return "undefinedresult";
				case Stoppable_Fields.RANGECHECK:
					return "rangecheck";
				case Stoppable_Fields.UNMATCHEDMARK:
					return "unmatchedmark";
				case Stoppable_Fields.LIMITCHECK:
					return "limitcheck";
				case Stoppable_Fields.SYNTAXERROR:
					return "syntaxerror";
				case Stoppable_Fields.INVALIDACCESS:
					return "invalidaccess";
				case Stoppable_Fields.INVALIDEXIT:
					return "invalidexit";
				case Stoppable_Fields.INVALIDRESTORE:
					return "invalidrestore";
				case Stoppable_Fields.UNDEFINEDFILENAME:
					return "undefinedfilename";
				case Stoppable_Fields.UNDEFINEDRESOURCE:
					return "undefinedresource";
				case Stoppable_Fields.INVALIDFILEACCESS:
					return "invalidfileaccess";
				case Stoppable_Fields.INVALIDFONT:
					return "invalidfont";
				case Stoppable_Fields.IOERROR:
					return "ioerror";
				case Stoppable_Fields.INTERRUPT:
					return "interrupt";
				case Stoppable_Fields.NOCURRENTPOINT:
					return "nocurrentpoint";
				case Stoppable_Fields.SECURITYCHECK:
					return "securitycheck";
				case Stoppable_Fields.TIMEOUT:
					return "timeout";
				case Stoppable_Fields.INTERNALERROR:
					return "internalerror";
				default:
					System.Console.WriteLine("errcode: " + cause);
					return "unknownerror";
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

	}

}