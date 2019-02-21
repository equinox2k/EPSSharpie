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

	public class Type3Decoder : AbstractFontDecoder
	{

		public Type3Decoder(Interpreter ip, DictType font) : base(ip, font)
		{
		}

		public static void checkFont(Interpreter ip, DictType font)
		{
			ArrayType fontBBox = (ArrayType) font.get("FontBBox", Types_Fields.ARRAY);
			if (!fontBBox.check(Types_Fields.NUMBER, 4))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "FontBBox");
			}
			ArrayType fontMatrix = (ArrayType) font.get("FontMatrix", Types_Fields.ARRAY);
			if (!fontMatrix.Matrix)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "FontMatrix type");
			}
			font.get("Encoding", Types_Fields.ARRAY);
		}

		public override CharWidth buildchar(Interpreter ip, int index, bool render)
		{
			ip.gsave();
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform ctm = (AffineTransform) gstate.currentmatrix().clone();
			AffineTransform fx = FontMatrix;
			try
			{
				Point2D curpt = gstate.currentpoint();
				gstate.translate(curpt.X, curpt.Y);
				gstate.concat(fx);
				buildglyph(ip, index);
			}
			finally
			{
				ip.grestore();
			}
			return CharWidth.transform(fx);
		}

		public override void buildglyph(Interpreter ip, int index)
		{
			DictType fontdict = FontDictionary;
			Any proc = fontdict.get("BuildChar");
			if (proc != null)
			{
				ip.ostack.pushRef(fontdict);
				ip.ostack.pushRef(new IntegerType(index));
			}
			else
			{
				proc = fontdict.get("BuildGlyph");
				if (proc == null)
				{
					throw new Stop(Stoppable_Fields.UNDEFINED, "BuildGlyph");
				}
				ip.ostack.pushRef(fontdict);
				ip.ostack.pushRef(encode(index));
			}
			ip.estack.run(ip, proc);
		}

	}

}