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

	public interface Stoppable
	{

	}

	public static class Stoppable_Fields
	{
		public const int TYPECHECK = 1;
		public const int STACKOVERFLOW = 2;
		public const int STACKUNDERFLOW = 3;
		public const int EXSTACKOVERFLOW = 4;
		public const int DICTSTACKOVERFLOW = 5;
		public const int DICTSTACKUNDERFLOW = 6;
		public const int UNDEFINED = 7;
		public const int UNDEFINEDRESULT = 8;
		public const int RANGECHECK = 9;
		public const int UNMATCHEDMARK = 10;
		public const int LIMITCHECK = 11;
		public const int SYNTAXERROR = 12;
		public const int INVALIDACCESS = 13;
		public const int INVALIDEXIT = 14;
		public const int INVALIDRESTORE = 15;
		public const int UNDEFINEDFILENAME = 16;
		public const int UNDEFINEDRESOURCE = 17;
		public const int INVALIDFILEACCESS = 18;
		public const int INVALIDFONT = 19;
		public const int IOERROR = 20;
		public const int NOCURRENTPOINT = 21;
		public const int SECURITYCHECK = 22;
		public const int INTERRUPT = 23;
		public const int INTERNALERROR = 24;
		public const int TIMEOUT = 25;
	}

}