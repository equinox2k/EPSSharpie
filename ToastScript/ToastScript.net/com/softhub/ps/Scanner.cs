using System;
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

	public class Scanner : Stoppable
	{

		public const int EOF = 0;
		public const int NUMBER = 1;
		public const int IDENT = 2;
		public const int LITERAL = 3;
		public const int IMMEDIATE = 4;
		public const int STRING = 5;
		public const int PROC_BEGIN = 6;
		public const int PROC_END = 7;

		/// <summary>
		/// String parenthesis balance counter.
		/// </summary>
		private int stringbalance;

		/// <summary>
		/// Proc curly brace balance counter.
		/// </summary>
		private int procbalance;

		/// <summary>
		/// The char buffer.
		/// </summary>
		private StringBuilder charbuffer = new StringBuilder();

		/// <summary>
		/// The numeric value.
		/// </summary>
		private Number number;

		public Scanner()
		{
		}

		public virtual string String
		{
			get
			{
				return charbuffer.ToString();
			}
		}

		public virtual Number Number
		{
			get
			{
				return number;
			}
		}

		public virtual int token(CharSequenceType cs)
		{
			int result = EOF;
			charbuffer.Length = 0;
			try
			{
				result = readToken(cs);
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, "token " + cs + " " + ex);
			}
			return result;
		}

		public virtual bool defered()
		{
			return procbalance > 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readToken(CharSequenceType cs) throws java.io.IOException
		private int readToken(CharSequenceType cs)
		{
			while (true)
			{
				int c = cs.getchar();
				switch (c)
				{
				case -1:
					return EOF;
				case ')':
					throw new Stop(Stoppable_Fields.SYNTAXERROR, "readToken");
				case '%':
					readComment(cs);
					break;
				case '\0':
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case '\b':
				case '\t':
				case '\n':
				case 11:
				case '\f':
				case '\r':
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case ' ':
					break;
				case '(':
					readString(cs);
					return STRING;
				case '<':
					return readDictBeginOrHexString(cs);
				case '>':
					if ((c = cs.getchar()) != '>')
					{
						throw new Stop(Stoppable_Fields.SYNTAXERROR, "readToken >");
					}
					charbuffer.Append('>');
					charbuffer.Append('>');
					return IDENT;
				case '/':
					return readLiteralName(cs);
				case '{':
					procbalance++;
					return PROC_BEGIN;
				case '}':
					if (procbalance <= 0)
					{
						throw new Stop(Stoppable_Fields.SYNTAXERROR, "readToken }");
					}
					procbalance--;
					return PROC_END;
				case '[':
				case ']':
					charbuffer.Append((char) c);
					return IDENT;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '+':
				case '-':
				case '.':
					cs.ungetchar(c);
					readName(cs);
					return convertNameToNumber(cs);
				default:
					cs.ungetchar(c);
					readName(cs);
					return IDENT;
				}
			}
		}

		/// <summary>
		/// Read over comments.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readComment(CharSequenceType cs) throws java.io.IOException
		private void readComment(CharSequenceType cs)
		{
			int c = cs.getchar();
			if (c == '%')
			{
				readDSC(cs);
			}
			else
			{
				// skip
				while (c >= 0 && c != '\n' && c != '\r')
				{
					c = cs.getchar();
				}
			}
		}

		/// <summary>
		/// Read DSC.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readDSC(CharSequenceType cs) throws java.io.IOException
		private void readDSC(CharSequenceType cs)
		{
			int c;
			StringBuilder buffer = new StringBuilder();
			while ((c = cs.getchar()) >= 0 && c != '\n' && c != '\r')
			{
				buffer.Append((char) c);
			}
			string dsc = buffer.ToString();
			if (dsc.Equals("EOF"))
			{
				cs.close();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readLiteralName(CharSequenceType cs) throws java.io.IOException
		private int readLiteralName(CharSequenceType cs)
		{
			int c;
			if ((c = cs.getchar()) == '/')
			{
				readName(cs);
				return IMMEDIATE;
			}
			else
			{
				cs.ungetchar(c);
				readName(cs);
				return LITERAL;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readName(CharSequenceType cs) throws java.io.IOException
		private void readName(CharSequenceType cs)
		{
			while (true)
			{
				int c = cs.getchar();
				switch (c)
				{
				case ')':
					throw new Stop(Stoppable_Fields.SYNTAXERROR, "readName");
				case -1:
				case '%':
				case '<':
				case '>':
				case '(':
				case '{':
				case '}':
				case '[':
				case ']':
				case '/':
					cs.ungetchar(c);
					// fall through
					goto case ' ';
				case ' ':
				case '\t':
				case '\r':
				case '\n':
				case '\b':
				case '\f':
				case '\0':
					return;
				default:
					charbuffer.Append((char) c);
					break;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int readDictBeginOrHexString(CharSequenceType cs) throws java.io.IOException
		private int readDictBeginOrHexString(CharSequenceType cs)
		{
			int result, c = cs.getchar();
			if (c == '<')
			{
				charbuffer.Append('<');
				charbuffer.Append('<');
				result = IDENT;
			}
			else
			{
				cs.ungetchar(c);
				readHexString(cs);
				result = STRING;
			}
			return result;
		}

		private int convertNameToNumber(CharSequenceType cs)
		{
			try
			{
				try
				{
					number = Convert.ToInt32(charbuffer.ToString());
				}
				catch (System.FormatException)
				{
					number = Convert.ToDouble(charbuffer.ToString());
				}
				return NUMBER;
			}
			catch (System.FormatException)
			{
				return convertNameToRadix();
			}
		}

		private int convertNameToRadix()
		{
			int i, c, radix = 0;
			int len = charbuffer.Length;
			char[] buffer = new char[len];
			charbuffer.getChars(0, len, buffer, 0);
			// scan the base
			for (i = 0; radix < 26 && i < len; i++)
			{
				c = buffer[i];
				if (i > 0 && c == '#')
				{
					break;
				}
				if (c < '0' || c > '9')
				{
					return IDENT;
				}
				radix = radix * 10 + (c - '0');
			}
			if (radix > 26)
			{
				return IDENT;
			}
			if (i >= len)
			{
				return IDENT;
			}
			// parse the NumberType using base
			string s = new string(buffer, i + 1, len - i - 1);
			try
			{
				number = Convert.ToInt32(s, radix);
				return NUMBER;
			}
			catch (System.FormatException)
			{
				return IDENT;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readString(CharSequenceType cs) throws java.io.IOException
		private void readString(CharSequenceType cs)
		{
			int c = -1;
			stringbalance = 1;
			charbuffer.Length = 0;
			while (stringbalance > 0 && (c = cs.getchar()) >= 0)
			{
				switch (c)
				{
				case '\\':
					readEscapeChar(cs);
					break;
				case '(':
					stringbalance++;
					charbuffer.Append((char) c);
					break;
				case ')':
					if (--stringbalance > 0)
					{
						charbuffer.Append((char) c);
					}
					break;
				default:
					charbuffer.Append((char) c);
					break;
				}
			}
			if (c < 0)
			{
				throw new Stop(Stoppable_Fields.SYNTAXERROR, "readString");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readEscapeChar(CharSequenceType cs) throws java.io.IOException
		private void readEscapeChar(CharSequenceType cs)
		{
			int c = cs.getchar();
			if (c < 0)
			{
				throw new Stop(Stoppable_Fields.SYNTAXERROR, "readEscapeChar");
			}
			switch (c)
			{
			case 'n':
				charbuffer.Append('\n');
				break;
			case 'r':
				charbuffer.Append('\r');
				break;
			case 't':
				charbuffer.Append('\t');
				break;
			case 'f':
				charbuffer.Append('\f');
				break;
			case 'b':
				charbuffer.Append('\b');
				break;
			case '\\':
				charbuffer.Append('\\');
				break;
			case '(':
			case ')':
				charbuffer.Append((char) c);
				break;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
				readOctalChar(cs, hexValue(c));
				break;
			default:
				charbuffer.Append((char) c);
			break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readOctalChar(CharSequenceType cs, int c0) throws java.io.IOException
		private void readOctalChar(CharSequenceType cs, int c0)
		{
			int c, c1, c2;
			c = cs.getchar();
			if (c < 0)
			{
				throw new Stop(Stoppable_Fields.SYNTAXERROR, "readOctalChar");
			}
			if ((c1 = hexValue(c)) < 0)
			{
				charbuffer.Append((char) c0);
				cs.ungetchar(c);
				return;
			}
			c = cs.getchar();
			if (c < 0)
			{
				throw new Stop(Stoppable_Fields.SYNTAXERROR, "readOctalChar2");
			}
			if ((c2 = hexValue(c)) < 0)
			{
				charbuffer.Append((char)(c0 * 8 + c1));
				cs.ungetchar(c);
				return;
			}
			charbuffer.Append((char)(c0 * 64 + c1 * 8 + c2));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readHexString(CharSequenceType cs) throws java.io.IOException
		private void readHexString(CharSequenceType cs)
		{
			int c = -1, c0, c1;
			bool eos = false;
			charbuffer.Length = 0;
			while (!eos && (c = cs.getchar()) >= 0 && c != '>')
			{
				if ((c0 = hexValue(c)) >= 0)
				{
					c = cs.getchar();
					if (c < 0)
					{
						throw new Stop(Stoppable_Fields.SYNTAXERROR, "readHexString");
					}
					if ((c1 = hexValue(c)) >= 0)
					{
						charbuffer.Append((char)(c0 * 16 + c1));
					}
					else if (c == '>')
					{
						charbuffer.Append((char)(c0 * 16));
						eos = true;
					}
				}
			}
			if (c < 0)
			{
				throw new Stop(Stoppable_Fields.SYNTAXERROR, "readHexString < 0");
			}
		}

		public static int hexValue(int c)
		{
			int result;
			if (c >= '0' && c <= '9')
			{
				result = c - '0';
			}
			else if (c >= 'a' && c <= 'f')
			{
				result = c - 'a' + 10;
			}
			else if (c >= 'A' && c <= 'F')
			{
				result = c - 'A' + 10;
			}
			else
			{
				result = -1;
			}
			return result;
		}

	}

}