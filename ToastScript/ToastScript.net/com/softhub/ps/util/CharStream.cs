namespace com.softhub.ps.util
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

	public interface CharStream
	{

		/// <summary>
		/// Read mode constant.
		/// </summary>

		/// <summary>
		/// Write mode constant.
		/// </summary>

		/// <returns> current character from stream </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getchar() throws java.io.IOException;
		int getchar();

		/// <param name="c"> the character to be written to stream </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void putchar(int c) throws java.io.IOException;
		void putchar(int c);

		/// <summary>
		/// Push back character to be read again at next
		/// call to getchar. </summary>
		/// <param name="c"> the character to be pushed </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void ungetchar(int c) throws java.io.IOException;
		void ungetchar(int c);

		/// <summary>
		/// Close the character stream.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void close() throws java.io.IOException;
		void close();

	}

	public static class CharStream_Fields
	{
		public const int READ_MODE = 1;
		public const int WRITE_MODE = 2;
	}

}