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

	public class BinaryCodec : AbstractCodec
	{

		public static bool debug;

		public const int EEXEC_SEED = 55665;
		public const int CHARSTRING_SEED = 4330;

		private const int CRYPT_C1 = 52845;
		private const int CRYPT_C2 = 22719;

		private int state;

		public BinaryCodec(int seed)
		{
			state = seed;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode() throws java.io.IOException
		public override int decode()
		{
			int c = stream.getchar();
			return c < 0 ? -1 : decode(c);
		}

		public virtual int decode(int c)
		{
			int val = c ^ (state >> 8);
			state = (((c + state) * CRYPT_C1) + CRYPT_C2) & 0xffff;
			if (debug)
			{
				print(val);
			}
			return val & 0xff;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void encode(com.softhub.ps.util.CharStream cs, int c) throws java.io.IOException
		public virtual void encode(CharStream cs, int c)
		{
			throw new IOException("BinaryCodec.encode not yet implemented");
		}

		private void print(int val)
		{
			char ch = (char)(val & 0xff);
			if (ch == '\n' || ch == '\r')
			{
				lineno++;
			}
			if (charno < 80)
			{
				System.Console.Write(ch);
			}
			if (charno++ >= 40 && (ch == ' ' || ch == '\r' || ch == '\n'))
			{
				System.Console.Write('\n');
				charno = 0;
			}
		}

		// for debugging only
		private int lineno = 1;
		private int charno = 0;

	}

}