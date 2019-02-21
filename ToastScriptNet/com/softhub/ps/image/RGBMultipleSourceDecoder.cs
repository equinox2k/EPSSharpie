﻿namespace com.softhub.ps.image
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

	internal class RGBMultipleSourceDecoder : RGBPixelSource
	{

		private ImageDecoder redComponent;
		private ImageDecoder greenComponent;
		private ImageDecoder blueComponent;

		internal RGBMultipleSourceDecoder(ImageDataProducer producer, object[] procs, int bits)
		{
			redComponent = new ImageDecoder(producer, procs[0], bits);
			greenComponent = new ImageDecoder(producer, procs[1], bits);
			blueComponent = new ImageDecoder(producer, procs[2], bits);
		}

		internal RGBMultipleSourceDecoder(CharStream[] data, int bits)
		{
			redComponent = new ImageDecoder(data[0], bits);
			greenComponent = new ImageDecoder(data[1], bits);
			blueComponent = new ImageDecoder(data[2], bits);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextRedComponent() throws java.io.IOException
		public virtual int nextRedComponent()
		{
			return redComponent.nextPixel();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextGreenComponent() throws java.io.IOException
		public virtual int nextGreenComponent()
		{
			return greenComponent.nextPixel();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int nextBlueComponent() throws java.io.IOException
		public virtual int nextBlueComponent()
		{
			return blueComponent.nextPixel();
		}

	}

}