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

	using CharWidth = com.softhub.ps.util.CharWidth;

	public class Type0Decoder : AbstractFontDecoder
	{

		private int fmaptype;

		private int escapeChar;

		private FontDecoder[] fontdecoder;

		private FontDecoder currentfont;

		public Type0Decoder(Interpreter ip, DictType font) : base(ip, font)
		{
			fmaptype = ((IntegerType) font.get("FMapType")).intValue();
			ArrayType fdepvector = (ArrayType) font.get("FDepVector");
			if (fmaptype == 3 || fmaptype == 7)
			{
				Any esc = font.get("EscChar");
				if (esc is IntegerType)
				{
					escapeChar = ((IntegerType) esc).intValue();
				}
				else
				{
					escapeChar = 255;
				}
			}
			AffineTransform fontMatrix = FontMatrix;
			int i, n = fdepvector.length();
			fontdecoder = new FontDecoder[n];
			for (i = 0; i < n; i++)
			{
				DictType fontDict = (DictType) fdepvector.get(i);
				ArrayType matrix = (ArrayType) fontDict.get("FontMatrix");
				AffineTransform xform = matrix.toTransform();
				AffineTransform xresult = (AffineTransform) fontMatrix.clone();
				xresult.concatenate(xform);
				matrix.put(ip.vm, xresult);
				fontDict.put(ip.vm, "FontMatrix", matrix);
				fontdecoder[i] = FontOp.createFontDecoder(ip, fontDict);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override object clone()
		{
			return (Type0Decoder) base.clone();
		}

		public static void checkFont(Interpreter ip, DictType font)
		{
			font.get("FMapType", Types_Fields.INTEGER);
			font.get("Encoding", Types_Fields.ARRAY);
			ArrayType fdepvector = (ArrayType) font.get("FDepVector", Types_Fields.ARRAY);
			int i, n = fdepvector.length();
			for (i = 0; i < n; i++)
			{
				DictType fontDict = (DictType) fdepvector.get(i);
				int fontType = ((IntegerType) fontDict.get("FontType", Types_Fields.INTEGER)).intValue();
				FontOp.checkFont(ip, fontDict, fontType);
			}
			ArrayType fontMatrix = (ArrayType) font.get("FontMatrix", Types_Fields.ARRAY);
			if (!fontMatrix.Matrix)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "FontMatrix");
			}
		}

		public override CharWidth buildchar(Interpreter ip, int index, bool render)
		{
			switch (fmaptype)
			{
			case 2:
				return buildcharFMapType2(ip, index, render);
			case 3:
				return buildcharFMapType3(ip, index, render);
			case 4:
				return buildcharFMapType4(ip, index, render);
			default:
				throw new Stop(Stoppable_Fields.INVALIDFONT, "FMapType " + fmaptype + " not implemented");
			}
		}

		private CharWidth buildcharFMapType2(Interpreter ip, int index, bool render)
		{
			CharWidth cw;
			if (currentfont == null)
			{
				int fontcode = index & 0xff;
				Any fontindex = encode(fontcode);
				if (!(fontindex is IntegerType))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "fontindex: " + fontindex);
				}
				int fdecIndex = ((IntegerType) fontindex).intValue();
				currentfont = fontdecoder[fdecIndex];
				cw = new CharWidth();
			}
			else
			{
				int charcode = index & 0xff;
				cw = currentfont.show(ip, charcode);
				currentfont = null;
			}
			return cw;
		}

		private CharWidth buildcharFMapType3(Interpreter ip, int index, bool render)
		{
			CharWidth cw;
			if (index == escapeChar)
			{
				currentfont = null;
				cw = new CharWidth();
			}
			else if (currentfont == null)
			{
				int fontcode = index & 0xff;
				Any fontindex = encode(fontcode);
				if (!(fontindex is IntegerType))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "fontindex: " + fontindex);
				}
				int fdecIndex = ((IntegerType) fontindex).intValue();
				currentfont = fontdecoder[fdecIndex];
				cw = new CharWidth();
			}
			else
			{
				int charcode = index & 0xff;
				cw = currentfont.show(ip, charcode);
			}
			return cw;
		}

		private CharWidth buildcharFMapType4(Interpreter ip, int index, bool render)
		{
			int fontcode = (index & 0xff) >> 7;
			Any fontindex = encode(fontcode);
			if (!(fontindex is IntegerType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "fontindex: " + fontindex);
			}
			int fdecIndex = ((IntegerType) fontindex).intValue();
			currentfont = fontdecoder[fdecIndex];
			int charcode = index & 0x7f;
			return currentfont.show(ip, charcode);
		}

		public override void buildglyph(Interpreter ip, int index)
		{
		}

	}

}