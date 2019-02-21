﻿namespace com.softhub.ps
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

	public class IntegerType : NumberType
	{

		private int val;

		public IntegerType(int val)
		{
			this.val = val;
		}

		public override bool Real
		{
			get
			{
				return false;
			}
		}

		public override int intValue()
		{
			return val;
		}

		public override float floatValue()
		{
			return val;
		}

		public override double realValue()
		{
			return val;
		}

		public override object cvj()
		{
			return new int?(intValue());
		}

		public override int typeCode()
		{
			return Types_Fields.INTEGER;
		}

		public override string typeName()
		{
			return "integertype";
		}

		public override bool Equals(object obj)
		{
			return obj is IntegerType && ((IntegerType) obj).val == val;
		}

		public override string ToString()
		{
			return val.ToString();
		}

		public override int GetHashCode()
		{
			return val;
		}

	}

}