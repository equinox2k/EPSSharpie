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

	public class RealType : NumberType
	{

		private double val;

		public RealType(double val)
		{
			this.val = val;
		}

		public override bool Real
		{
			get
			{
				return true;
			}
		}

		public override int intValue()
		{
			return (int) val;
		}

		public override float floatValue()
		{
			return (float) val;
		}

		public override double realValue()
		{
			return val;
		}

		public override object cvj()
		{
			return new double?(floatValue());
		}

		public override int typeCode()
		{
			return Types_Fields.REAL;
		}

		public override string typeName()
		{
			return "realtype";
		}

		public override bool Equals(object obj)
		{
			return obj is RealType && ((RealType) obj).val == val;
		}

		public override string ToString()
		{
			return val.ToString();
		}

		public override int GetHashCode()
		{
			long bits = System.BitConverter.DoubleToInt64Bits(val);
			return (int)(bits ^ (bits >> 32));
		}

	}

}