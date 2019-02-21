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

	using FontInfo = com.softhub.ps.device.FontInfo;
	using CharWidth = com.softhub.ps.util.CharWidth;

	public interface FontDecoder : FontInfo
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object clone() throws CloneNotSupportedException;
		object clone();

		CharWidth show(Interpreter ip, int index);

		CharWidth buildchar(Interpreter ip, int index, bool render);

		void buildglyph(Interpreter ip, int index);

		CharWidth charwidth(Interpreter ip, int index);

		CharWidth charpath(Interpreter ip, int index);

		AffineTransform FontMatrix {get;}

		CharWidth CharWidth {set;get;}


		Rectangle2D FontBBox {get;}

		DictType FontDictionary {get;}

		Any encode(int index);

	}

}