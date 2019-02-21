namespace com.softhub.ps.device
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

	using Reusable = com.softhub.ps.graphics.Reusable;
	using CharWidth = com.softhub.ps.util.CharWidth;

	internal class CacheEntry
	{

		private string character;
		private object fontID;
		private int fontType;
		private Reusable obj;
		private CharWidth charWidth;

		internal CacheEntry(FontInfo info, string character)
		{
			fontID = info.FontUniqueID;
			fontType = info.FontType;
			this.character = character;
		}

		internal CacheEntry(FontInfo info, string ch, CharWidth cw, Reusable obj) : this(info, ch)
		{
			this.obj = obj;
			this.charWidth = cw;
		}

		internal virtual Reusable Object
		{
			get
			{
				return obj;
			}
		}

		internal virtual CharWidth CharWidth
		{
			get
			{
				return charWidth;
			}
		}

		public override int GetHashCode()
		{
			return character.GetHashCode() ^ fontID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CacheEntry))
			{
				return false;
			}
			CacheEntry entry = (CacheEntry) obj;
			return character.Equals(entry.character) && fontID.Equals(entry.fontID) && fontType == entry.fontType;
		}

	}

}