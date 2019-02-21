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

	public class ASCII85Codec : AbstractCodec
	{

		private static readonly int[] DECODING = new int[] {52200625, 614125, 7225, 85, 1};

		private sbyte[] buffer = new sbyte[4];
		private int count;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void close()
		{
			if (mode == com.softhub.ps.util.CharStream_Fields.WRITE_MODE)
			{
				if (count > 0)
				{
					while (count < 4)
					{
						buffer[count++] = 0;
					}
					flushEncodingBuffer();
				}
				stream.putchar('~');
				stream.putchar('>');
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int decode() throws java.io.IOException
		public override int decode()
		{
			if (endOfData && count <= 0)
			{
				return Codec_Fields.EOD;
			}
			if (count <= 0)
			{
				fillDecodingBuffer();
				if (count <= 0)
				{
					return Codec_Fields.EOD;
				}
			}
			return buffer[--count];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void encode(int c) throws java.io.IOException
		public override void encode(int c)
		{
			if (count >= 3)
			{
				buffer[count] = (sbyte) c;
				flushEncodingBuffer();
				count = 0;
			}
			else
			{
				buffer[count++] = (sbyte) c;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fillDecodingBuffer() throws java.io.IOException
		private void fillDecodingBuffer()
		{
			int c, i = 0;
			long m = 0;
			while (!endOfData && i < buffer.Length + 1 && (c = stream.getchar()) >= 0)
			{
				if (!isWhiteSpace(c))
				{
					if (c == 'z')
					{
						if ((i % 5) != 0)
						{
							throw new IOException("invalid z position");
						}
						i += 5;
						m = 0;
					}
					else if ('!' <= c && c <= 'u')
					{
						m += (c - '!') * DECODING[i++];
					}
					else if (c == '~')
					{
						if ((c = stream.getchar()) != '>')
						{
							throw new IOException("bad end of decoding character: " + c);
						}
						endOfData = true;
					}
					else
					{
						throw new IOException("invalid character: " + c);
					}
					if ((i % 5) == 0 || endOfData)
					{
						if (m >= (1L << 32))
						{
							throw new IOException("out of range: " + m);
						}
						buffer[count++] = unchecked((sbyte)(m & 255L));
						buffer[count++] = unchecked((sbyte)((m >> 8) & 255L));
						buffer[count++] = unchecked((sbyte)((m >> 16) & 255L));
						buffer[count++] = unchecked((sbyte)((m >> 24) & 255L));
						m = 0;
					}
				}
			}
			if (!endOfData)
			{
				// look ahead for end of filter sequence
				if ((c = stream.getchar()) == '~')
				{
					if ((c = stream.getchar()) != '>')
					{
						throw new IOException("bad end of decoding: " + c);
					}
					endOfData = true;
				}
				else
				{
					stream.ungetchar(c);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void flushEncodingBuffer() throws java.io.IOException
		private void flushEncodingBuffer()
		{
			long val = bufferToLong();
			if (val == 0)
			{
				stream.putchar('z');
			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					int m = (int)(val / DECODING[i]);
					stream.putchar((m % 85) + '!');
				}
			}
		}

		private long bufferToLong()
		{
			long result = 0;
			for (int i = 0; i < 4; i++)
			{
				result <<= 8;
				result += buffer[i];
			}
			return result;
		}

	}

}