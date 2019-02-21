using System;

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

	using CacheDevice = com.softhub.ps.device.CacheDevice;
	using CharWidth = com.softhub.ps.util.CharWidth;

	public abstract class AbstractFontDecoder : FontDecoder, Types, ICloneable, Stoppable
	{
		/// <summary>
		/// The font dictionary.
		/// </summary>
		private DictType fontDict;

		/// <summary>
		/// The "FontMatrix" value.
		/// </summary>
		private AffineTransform fontMatrix;

		/// <summary>
		/// The "FontBBox" value.
		/// </summary>
		private Rectangle2D fontBBox;

		/// <summary>
		/// The encoding vector.
		/// </summary>
		private ArrayType encodingVector;

		/// <summary>
		/// The FontID.
		/// </summary>
		private FontIDType fontID;

		/// <summary>
		/// The corresponding bitmap font.
		/// </summary>
		private Font systemfont;

		/// <summary>
		/// The font type.
		/// </summary>
		private int fontType;

		/// <summary>
		/// The current char width.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal CharWidth charwidth_Renamed = new CharWidth();

		public AbstractFontDecoder(Interpreter ip, DictType font)
		{
			fontID = (FontIDType) font.get("FID", Types_Fields.FONTID);
			fontMatrix = ((ArrayType) font.get("FontMatrix")).toTransform();
			ArrayType fontbbox = (ArrayType) font.get("FontBBox");
			// FontBBox not required in type 0 fonts
			if (fontbbox != null)
			{
				setFontBBox(fontbbox);
			}
			encodingVector = (ArrayType) font.get("Encoding");
			fontType = ((IntegerType) font.get("FontType")).intValue();
			fontDict = font;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
			AbstractFontDecoder decoder = (AbstractFontDecoder) base.clone();
			decoder.fontMatrix = (AffineTransform) fontMatrix.clone();
			return decoder;
		}

		public virtual CharWidth show(Interpreter ip, int index)
		{
			GraphicsState gstate = ip.GraphicsState;
			CacheDevice cachedevice = gstate.cachedevice();
			AffineTransform ctm = (AffineTransform) gstate.currentmatrix().clone();
			Point2D curpt = gstate.currentpoint();
			string character = encode(index).ToString();
			CharWidth cw = cachedevice.showCachedCharacter(this, ctm, curpt, character);
			return cw != null ? cw : buildchar(ip, index, true);
		}

		public abstract CharWidth buildchar(Interpreter ip, int index, bool render);

		public abstract void buildglyph(Interpreter ip, int index);

		public virtual AffineTransform FontMatrix
		{
			get
			{
				return fontMatrix;
			}
		}

		public virtual CharWidth CharWidth
		{
			set
			{
				charwidth_Renamed.Width = value;
			}
			get
			{
				return charwidth_Renamed;
			}
		}


		protected internal virtual FontIDType FontID
		{
			get
			{
				return fontID;
			}
		}

		public virtual int FontType
		{
			get
			{
				return fontType;
			}
		}

		public virtual Font getSystemFont()
		{
			return systemfont;
		}

		protected internal virtual void setFontBBox(ArrayType fontbbox)
		{
			Any x0 = fontbbox.get(0);
			if (!(x0 is NumberType))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad font bbox<0>");
			}
			Any y0 = fontbbox.get(1);
			if (!(y0 is NumberType))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad font bbox<1>");
			}
			Any x1 = fontbbox.get(2);
			if (!(x1 is NumberType))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad font bbox<2>");
			}
			Any y1 = fontbbox.get(3);
			if (!(y1 is NumberType))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad font bbox<3>");
			}
			float x = ((NumberType) x0).floatValue();
			float y = ((NumberType) y0).floatValue();
			float w = ((NumberType) x1).floatValue() - x;
			float h = ((NumberType) y1).floatValue() - y;
			fontBBox = new Rectangle2D.Float(x, y, w, h);
		}

		public virtual Rectangle2D getFontBBox()
		{
			return fontBBox;
		}

		public virtual DictType FontDictionary
		{
			get
			{
				return fontDict;
			}
		}

		public virtual CharWidth charwidth(Interpreter ip, int index)
		{
			CharWidth cw = getMetrics(ip, index);
			return cw != null ? cw : buildchar(ip, index, false);
		}

		protected internal virtual CharWidth getMetrics(Interpreter ip, int index)
		{
			Any metrics = fontDict.get("Metrics");
			if (metrics == null)
			{
				return null;
			}
			if (!(metrics is DictType))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "metrics entry not a dict");
			}
			Any any = ((DictType) metrics).get(encode(index));
			if (any is NumberType)
			{
				return new CharWidth(((NumberType) any).floatValue());
			}
			if (!(any is ArrayType))
			{
				return null;
			}
			ArrayType array = (ArrayType) any;
			NumberType wx, wy, sx, sy;
			switch (array.length())
			{
			case 2:
				if (!array.check(Any.NUMBER))
				{
					throw new Stop(Stoppable_Fields.INVALIDFONT, "bad metrics entry<2>");
				}
				sx = (NumberType) array.get(0);
				wx = (NumberType) array.get(1);
				return new CharWidth(sx.floatValue(), wx.floatValue());
			case 4:
				if (!array.check(Any.NUMBER))
				{
					throw new Stop(Stoppable_Fields.INVALIDFONT, "bad metrics entry<4>");
				}
				sx = (NumberType) array.get(0);
				sy = (NumberType) array.get(1);
				wx = (NumberType) array.get(2);
				wy = (NumberType) array.get(3);
				return new CharWidth(sx.floatValue(), sy.floatValue(), wx.floatValue(), wy.floatValue());
			default:
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad metric value");
			}
		}

		public virtual CharWidth charpath(Interpreter ip, int index)
		{
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform ctm = (AffineTransform) gstate.currentmatrix().clone();
			AffineTransform fx = FontMatrix;
			Point2D oldpt = gstate.currentpoint();
			gstate.concat(fx);
			Point2D curpt = gstate.currentpoint();
			double x = curpt.X;
			double y = curpt.Y;
			gstate.translate(x, y);
			gstate.moveto(0, 0);
			buildglyph(ip, index);
			gstate.setmatrix(ctm);
			gstate.moveto(oldpt.X, oldpt.Y);
			return charwidth_Renamed.transform(fx);
		}

		public virtual Any encode(int index)
		{
			return encodingVector.get(index & 0xff);
		}

		public virtual string FontName
		{
			get
			{
				return fontID.FontName;
			}
		}

		public virtual object FontUniqueID
		{
			get
			{
				return fontID;
			}
		}

		public virtual string getCharName(int index)
		{
			return encode(index).ToString();
		}

		private void setSystemFont(Interpreter ip)
		{
			DictType dict = ip.GraphicsState.SystemFonts;
			if (dict == null)
			{
				return;
			}
			string fontname = FontName;
			Any sysname = dict.get(fontname);
			if (sysname == null)
			{
				return;
			}
			if (!(sysname is NameType || sysname is StringType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "bad sysname: " + fontname + " -> " + sysname);
			}
			int size = 12, style = Font.PLAIN;
			string name = sysname.ToString();
			int i = name.IndexOf('-');
			if (i > 0)
			{
				string str = name;
				name = str.Substring(0, i);
				str = str.Substring(i + 1);
				if ((i = str.IndexOf('-')) >= 0)
				{
					if (str.StartsWith("bold-", StringComparison.Ordinal))
					{
						style = Font.BOLD;
					}
					else if (str.StartsWith("italic-", StringComparison.Ordinal))
					{
						style = Font.ITALIC;
					}
					else if (str.StartsWith("bolditalic-", StringComparison.Ordinal))
					{
						style = Font.BOLD | Font.ITALIC;
					}
					str = str.Substring(i + 1);
				}
				try
				{
					size = Convert.ToInt32(str);
				}
				catch (System.FormatException)
				{
				}
			}
			systemfont = new Font(name, style, size);
		}

	}

}