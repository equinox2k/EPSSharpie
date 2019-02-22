using System;
using ToastScriptNet;

namespace com.softhub.ps.graphics
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

	using CharWidth = com.softhub.ps.util.CharWidth;

	[Serializable]
	public class CharacterShape : DisplayList, Reusable
	{

		private string charCode;
		private CharWidth charWidth;

		public CharacterShape() : base(1)
		{
		}

		public virtual string CharCode
		{
			set
			{
				this.charCode = value;
			}
			get
			{
				return charCode;
			}
		}


		public virtual CharWidth CharWidth
		{
			set
			{
				this.charWidth = value;
			}
			get
			{
				return charWidth;
			}
		}


		public virtual Rectangle2D Bounds2D
		{
			get
			{
				Rectangle2D rect = null;
				for (int i = 0; i < index; i++)
				{
					Rectangle2D r = buffer[i].Bounds2D;
					if (r != null)
					{
						if (rect == null)
						{
							rect = r;
						}
						else
						{
							rect = rect.createUnion(r);
						}
					}
				}
				return rect;
			}
		}

        Rectangle2D Reusable.Bounds2D => throw new NotImplementedException();

        public void draw(Graphics2D g)
        {
            throw new NotImplementedException();
        }

        public void normalize(AffineTransform xform)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
		{
			return "character-shape<" + charCode + ">";
		}

	}

}