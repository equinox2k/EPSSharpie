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

	public interface Types
	{

	}

	public static class Types_Fields
	{
		public const int ANY = -1;
		public static readonly int INTEGER = 1 << 0;
		public static readonly int REAL = 1 << 1;
		public static readonly int NUMBER = INTEGER | REAL;
		public static readonly int BOOLEAN = 1 << 2;
		public static readonly int NAME = 1 << 3;
		public static readonly int STRING = 1 << 4;
		public static readonly int ARRAY = 1 << 5;
		public static readonly int DICT = 1 << 6;
		public static readonly int OPERATOR = 1 << 7;
		public static readonly int NULL = 1 << 8;
		public static readonly int MARK = 1 << 9;
		public static readonly int FILE = 1 << 10;
		public static readonly int FONTID = 1 << 11;
		public static readonly int SAVE = 1 << 12;
		public static readonly int GSTATE = 1 << 13;
	}

}