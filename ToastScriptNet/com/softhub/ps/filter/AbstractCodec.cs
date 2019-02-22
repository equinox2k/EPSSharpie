﻿using System;

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

	public abstract class AbstractCodec : Codec
	{

		protected internal int mode;
		protected internal CharStream stream;
		protected internal bool endOfData;

		public virtual void open(CharStream stream, int mode)
		{
			this.stream = stream;
			this.mode = mode;
		}

		public virtual void close()
		{
		}

		public virtual int decode()
		{
			throw new NotImplementedException("not implemented");
		}

		public virtual void encode(int c)
		{
			throw new NotImplementedException("not implemented");
		}

		public virtual Type[] OptionalParameterTypes
		{
			get
			{
				return null;
			}
		}

		public virtual object[] OptionalParameters
		{
			set
			{
			}
		}

		protected internal static bool isWhiteSpace(int c)
		{
			switch (c)
			{
			case '\0':
			case '\n':
			case '\r':
			case '\f':
			case '\t':
			case ' ':
				return true;
			}
			return false;
		}

	}

}