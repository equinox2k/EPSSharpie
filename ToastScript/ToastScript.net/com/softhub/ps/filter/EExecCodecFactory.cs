namespace com.softhub.ps.filter
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

	public class EExecCodecFactory
	{

		private const int ESC = 128;

		private const int TXT = 1;
		private const int BIN = 2;
		private const int END = 3;
		private const int HEX = 4;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Codec createCodec(com.softhub.ps.util.CharStream stream, int mode) throws java.io.IOException
		public static Codec createCodec(CharStream stream, int mode)
		{
			Codec codec;
			int esc = stream.getchar();
			if (esc < 0)
			{
				throw new IOException();
			}
			if (esc == ESC)
			{
				int enc = stream.getchar();
				if (enc < 0)
				{
					throw new IOException();
				}
				switch (enc)
				{
				case TXT:
					codec = new EExecTextCodec();
					codec.open(stream, mode);
					skip(stream, 4);
					break;
				case BIN:
					codec = new EExecBinaryCodec();
					codec.open(stream, mode);
					skip(stream, 4);
					skipDecode(codec, 4);
					break;
				default:
					throw new IOException();
				}
			}
			else if (esc == '%')
			{
				stream.ungetchar(esc);
				codec = new EExecNullCodec();
				codec.open(stream, mode);
			}
			else
			{
				stream.ungetchar(esc);
				codec = new EExecHexCodec();
				codec.open(stream, mode);
				skipDecode(codec, 4);
			}
			return codec;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void skip(com.softhub.ps.util.CharStream stream, int n) throws java.io.IOException
		private static void skip(CharStream stream, int n)
		{
			for (int i = 0; i < n; i++)
			{
				stream.getchar();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void skipDecode(Codec codec, int n) throws java.io.IOException
		private static void skipDecode(Codec codec, int n)
		{
			for (int i = 0; i < n; i++)
			{
				codec.decode();
			}
		}

		internal class EExecTextCodec : EExecNullCodec, Codec
		{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode() throws java.io.IOException
			public override int decode()
			{
				int c = stream.getchar();
				if (c == ESC)
				{
					c = stream.getchar();
					if (c == END)
					{
						c = -1;
					}
				}
				return c;
			}

		}

		internal class EExecNullCodec : AbstractCodec
		{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode(com.softhub.ps.util.CharStream cs) throws java.io.IOException
			public virtual int decode(CharStream cs)
			{
				return cs.getchar();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode(int c) throws java.io.IOException
			public virtual int decode(int c)
			{
				return c;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode() throws java.io.IOException
			public override int decode()
			{
				return stream.getchar();
			}

		}

		internal class EExecBinaryCodec : BinaryCodec
		{

			internal EExecBinaryCodec() : base(EEXEC_SEED)
			{
			}

		}

		internal class EExecHexCodec : BinaryCodec
		{

			internal EExecHexCodec() : base(EEXEC_SEED)
			{
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode() throws java.io.IOException
			public override int decode()
			{
				return decode(nextHex(stream));
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int nextHex(com.softhub.ps.util.CharStream cs) throws java.io.IOException
			internal static int nextHex(CharStream cs)
			{
				int c0 = 0, c1 = 0;
				c0 = nextNonBlankChar(cs);
				if (c0 < 0)
				{
					return -1;
				}
				c1 = nextNonBlankChar(cs);
				int x0 = ASCIIHexCodec.hexValue(c0);
				if (c1 < 0)
				{
					return x0;
				}
				int x1 = ASCIIHexCodec.hexValue(c1);
				return (x0 << 4) | x1;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int nextNonBlankChar(com.softhub.ps.util.CharStream cs) throws java.io.IOException
			internal static int nextNonBlankChar(CharStream cs)
			{
				int c = 0;
				do
				{
					c = cs.getchar();
				} while (c >= 0 && c <= ' ');
				return c;
			}

		}

	}

}