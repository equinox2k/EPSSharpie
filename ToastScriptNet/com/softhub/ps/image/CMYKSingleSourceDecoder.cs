namespace com.softhub.ps.image
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

	internal class CMYKSingleSourceDecoder : ImageDecoder, CMYKPixelSource
	{

		private int state;

		internal CMYKSingleSourceDecoder(ImageDataProducer producer, object proc, int bits) : base(producer, proc, bits)
		{
		}

		internal CMYKSingleSourceDecoder(CharStream src, int bits) : base(src, bits)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextCyanComponent() throws java.io.IOException
		public virtual int nextCyanComponent()
		{
			if ((state++ % 4) != 0)
			{
				throw new IOException("illegal state");
			}
			return nextPixel();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextMagentaComponent() throws java.io.IOException
		public virtual int nextMagentaComponent()
		{
			if ((state++ % 4) != 1)
			{
				throw new IOException("illegal state");
			}
			return nextPixel();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextYellowComponent() throws java.io.IOException
		public virtual int nextYellowComponent()
		{
			if ((state++ % 4) != 2)
			{
				throw new IOException("illegal state");
			}
			return nextPixel();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextBlackComponent() throws java.io.IOException
		public virtual int nextBlackComponent()
		{
			if ((state++ % 4) != 3)
			{
				throw new IOException("illegal state");
			}
			return nextPixel();
		}

	}

}