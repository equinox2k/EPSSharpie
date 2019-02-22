using System;

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

	public class SubFileCodec : AbstractCodec
	{

		private static readonly Type[] PARAMETERS = new Type[] {typeof(int), typeof(string)};

		private int count;
		private char[] mark;
		private char[] buffer;
		private int low, high;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(com.softhub.ps.util.CharStream stream, int mode) throws java.io.IOException
		public override void open(CharStream stream, int mode)
		{
			base.open(stream, mode);
			low = high = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode() throws java.io.IOException
		public override int decode()
		{
			if (low < high)
			{
				return buffer[low++];
			}
			do
			{
				low = high = 0;
				int c;
				while ((c = stream.getchar()) >= 0)
				{
					buffer[high] = (char) c;
					if ((char)c != mark[high++])
					{
						return buffer[low++];
					}
					if (high >= buffer.Length)
					{
						if (--count < 0)
						{
							endOfData = true;
						}
						break;
					}
				}
			} while (count >= 0);
			return Codec_Fields.EOD;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void encode(int c) throws java.io.IOException
		public override void encode(int c)
		{
			throw new NotImplementedException("SubFileCodec.encode not yet implemented");
		}

		public override Type[] OptionalParameterTypes
		{
			get
			{
				return PARAMETERS;
			}
		}

		public override object[] OptionalParameters
		{
			set
			{
				count = ((int)value[0]);
				mark = ((string) value[1]).ToCharArray();
				buffer = new char[mark.Length];
			}
		}

	}

}