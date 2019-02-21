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

	using CharStream = com.softhub.ps.util.CharStream;

	public abstract class CharSequenceType : CompositeType, CharStream
	{
		/// <summary>
		/// Construct a character sequence object. </summary>
		/// <param name="vm"> the virtual memory </param>
		public CharSequenceType(VM vm) : base(vm.Global)
		{
		}

		/// <summary>
		/// Construct a character sequence object. </summary>
		/// <param name="vm"> the virtual memory </param>
		/// <param name="seq"> the object to copy </param>
		public CharSequenceType(VM vm, CharSequenceType seq) : base(seq, vm.Global)
		{
		}

		/// <summary>
		/// Construct a character sequence object. </summary>
		/// <param name="seq"> the object to copy </param>
		public CharSequenceType(CharSequenceType seq) : base(seq)
		{
		}

		/// <summary>
		/// Read a token from this object and push
		/// it onto operand stack. </summary>
		/// <param name="ip"> the interpreter </param>
		/// <returns> true if there are more tokens </returns>
		public abstract bool token(Interpreter ip);

		/// <summary>
		/// Read a single character from this object. </summary>
		/// <returns> the character </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract int getchar() throws java.io.IOException;
		public abstract int getchar();

		/// <summary>
		/// Unread the character. </summary>
		/// <param name="c"> the character to be pushed back </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void ungetchar(int c) throws java.io.IOException;
		public abstract void ungetchar(int c);

		/// <summary>
		/// Write a single character to this object. </summary>
		/// <param name="c"> the character </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void putchar(int c) throws java.io.IOException;
		public abstract void putchar(int c);

		/// <summary>
		/// Close the stream.
		/// </summary>
		public abstract void close();

	}

}