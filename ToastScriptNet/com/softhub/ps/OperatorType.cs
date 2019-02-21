﻿namespace com.softhub.ps
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
	/// 
	/// There are three basic types of operators:
	/// 
	/// (1) operators which use reflection and invoke
	///     a static method on some class by passing the
	///     interpreter object as a parameter.
	/// (2) operators which keep an opcode and use a
	///     switch block to select the proper method.
	/// (3) operators which directly overwrite the exec
	///     method.
	/// 
	/// Implementing all operators like (1) makes the interpreter
	/// about 40% slower on average.
	/// Implementing all operators like (3) makes the jar file
	/// more then twice as big and (2) is a good compromise between
	/// code size and speed.
	/// 
	/// Frequently used operators should use (3). Heavy operators
	/// performing quite a bit of work should use (1). There are
	/// some operators like "if" which have to be implemented using (1),
	/// because the name is a Java key word.
	/// </summary>

	public class OperatorType : Any
	{

		private string name;

		public OperatorType(string name)
		{
			this.name = name;
		}

		public override int typeCode()
		{
			return Types_Fields.OPERATOR;
		}

		public override string typeName()
		{
			return "operatortype";
		}

		protected internal override bool Global
		{
			get
			{
				return true;
			}
		}

		public override bool Literal
		{
			get
			{
				return false;
			}
		}

		public override bool Executable
		{
			get
			{
				return true;
			}
		}

		public override void exec(Interpreter ip)
		{
			ip.ostack.pushRef(this);
		}

		public override bool Equals(object obj)
		{
			return obj is OperatorType && string.ReferenceEquals(name, ((OperatorType) obj).name);
		}

		public override string ToString()
		{
			return name;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

	}

}