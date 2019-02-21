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

	using Device = com.softhub.ps.device.Device;

	internal sealed class FontOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"findfont", "definefont", "undefinefont", "scalefont", "makefont", "setfont", "selectfont", "currentfont", "show", "ashow", "widthshow", "awidthshow", "kshow", "stringwidth", "charpath", "xshow", "yshow", "xyshow", "cshow"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(FontOp));
		}

		internal static void findfont(Interpreter ip)
		{
			Any key = ip.ostack.pop();
			if (!ResourceOp.handleFindResource(ip, "Font", key))
			{
				// handler has altered the stack -> restore
				ip.ostack.pushRef(key);
				throw new Stop(Stoppable_Fields.UNDEFINED, "check font resources");
			}
		}

		internal static void definefont(Interpreter ip)
		{
			DictType font = (DictType) ip.ostack.pop(Types_Fields.DICT);
			Any name = ip.ostack.pop();
			Any fontname = mapFontName(ip, name);
			if (fontname != null)
			{
				name = fontname;
			}
			int id, type = ((IntegerType) font.get("FontType", Types_Fields.INTEGER)).intValue();
			checkFont(ip, font, type);
			Any uid = font.get("UniqueID");
			if (uid is IntegerType)
			{
				id = ((IntegerType) uid).intValue();
			}
			else
			{
				id = (new Random()).Next();
			}
			if ((name.typeID() & (Types_Fields.NAME | Types_Fields.STRING)) == 0)
			{
				name = new NameType("anonymous font [" + id + "]");
			}
			FontIDType fontID = new FontIDType((type << 24) + id, name.ToString());
			font.put(ip.vm, new NameType("FID"), fontID);
			font.setFontAttr();
			if (fontname != null)
			{
				// fix the font name for clones
				int flags = font.saveAccessFlags();
				font.put(ip.vm, "FontName", name);
				font.restoreAccessFlags(flags);
			}
			DictType dir = getFontDirectory(ip);
			dir.put(ip.vm, name, font);
			ip.ostack.pushRef(font);
		}

		internal static void undefinefont(Interpreter ip)
		{
			Any name = ip.ostack.pop();
			DictType dir = getFontDirectory(ip);
			if (dir.known(name))
			{
				dir.remove(ip.vm, name);
			}
		}

		internal static void scalefont(Interpreter ip)
		{
			NumberType scale = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			DictType font = (DictType) ip.ostack.pop(Types_Fields.DICT);
			if (!font.FontAttr)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT);
			}
			NameType key = new NameType("FontMatrix");
			ArrayType fontmatrix = (ArrayType) font.get(key);
			ArrayType newfontmat = new ArrayType(ip.vm, fontmatrix.length());
			newfontmat.putinterval(ip.vm, 0, fontmatrix);
			int flags = newfontmat.saveAccessFlags();
			MatrixOp.scale(ip, newfontmat, scale, scale);
			newfontmat.restoreAccessFlags(flags);
			DictType newfontdict = new DictType(ip.vm, font);
			newfontdict.put(ip.vm, key, newfontmat);
			ip.ostack.pushRef(newfontdict);
		}

		internal static void makefont(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			if (!matrix.Matrix)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			DictType font = (DictType) ip.ostack.pop(Types_Fields.DICT);
			if (!font.FontAttr)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT);
			}
			NameType key = new NameType("FontMatrix");
			ArrayType fontmatrix = (ArrayType) font.get(key);
			ArrayType newfontmat = new ArrayType(ip.vm, fontmatrix.length());
			newfontmat.putinterval(ip.vm, 0, fontmatrix);
			int flags = newfontmat.saveAccessFlags();
			MatrixOp.concat(ip, newfontmat, matrix, newfontmat);
			newfontmat.restoreAccessFlags(flags);
			DictType newfontdict = new DictType(ip.vm, font);
			newfontdict.put(ip.vm, key, newfontmat);
			ip.ostack.pushRef(newfontdict);
		}

		internal static void selectfont(Interpreter ip)
		{
			Any scale = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			Any key = ip.ostack.pop();
			DictType dir = getFontDirectory(ip);
			DictType font = (DictType) dir.get(key, Types_Fields.DICT);
			if (font == null)
			{
				ip.ostack.pushRef(key);
				ip.estack.run(ip, (new NameType("findfont")).cvx());
			}
			else
			{
				ip.ostack.pushRef(font);
			}
			ip.ostack.pushRef(scale);
			if (scale.typeOf(Types_Fields.NUMBER))
			{
				scalefont(ip);
			}
			else
			{
				makefont(ip);
			}
			setfont(ip);
		}

		internal static void setfont(Interpreter ip)
		{
			DictType font = (DictType) ip.ostack.pop(Types_Fields.DICT);
			if (!font.FontAttr)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT);
			}
			FontDecoder decoder = createFontDecoder(ip, font);
			GraphicsState gstate = ip.GraphicsState;
			gstate.FontDecoder = decoder;
			Device device = gstate.currentdevice();
			device.Font = decoder;
		}

		internal static void currentfont(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			FontDecoder fontdecoder = gstate.FontDecoder;
			if (fontdecoder == null)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "no current font");
			}
			ip.ostack.push(fontdecoder.FontDictionary);
		}

		internal static void show(Interpreter ip)
		{
			ip.GraphicsState.show(ip, (StringType) ip.ostack.pop(Types_Fields.STRING));
		}

		internal static void ashow(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			double ay = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double ax = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ip.GraphicsState.ashow(ip, ax, ay, @string);
		}

		internal static void widthshow(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			int c = ip.ostack.popInteger();
			double cy = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double cx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ip.GraphicsState.widthshow(ip, cx, cy, c, @string);
		}

		internal static void awidthshow(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			double ay = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double ax = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			int c = ip.ostack.popInteger();
			double cy = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double cx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ip.GraphicsState.awidthshow(ip, cx, cy, c, ax, ay, @string);
		}

		internal static void kshow(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ip.GraphicsState.kshow(ip, proc, @string);
		}

		internal static void cshow(Interpreter ip)
		{
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ArrayType proc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ip.GraphicsState.cshow(ip, proc, @string);
		}

		internal static void xshow(Interpreter ip)
		{
			Any displacement = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ip.GraphicsState.xshow(ip, @string, displacement);
		}

		internal static void yshow(Interpreter ip)
		{
			Any displacement = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ip.GraphicsState.yshow(ip, @string, displacement);
		}

		internal static void xyshow(Interpreter ip)
		{
			Any displacement = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING);
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ip.GraphicsState.xyshow(ip, @string, displacement);
		}

		internal static void stringwidth(Interpreter ip)
		{
			StringType s = (StringType) ip.ostack.pop(Types_Fields.STRING);
			float[] cw = ip.GraphicsState.stringwidth(ip, s);
			ip.ostack.pushRef(new RealType(cw[0]));
			ip.ostack.pushRef(new RealType(cw[1]));
		}

		internal static void charpath(Interpreter ip)
		{
			bool stroked = ip.ostack.popBoolean();
			StringType @string = (StringType) ip.ostack.pop(Types_Fields.STRING);
			ip.GraphicsState.charpath(ip, @string, stroked);
		}

		internal static DictType getFontDirectory(Interpreter ip)
		{
			DictType dir;
			if (ip.vm.Global)
			{
				dir = (DictType) ip.systemdict.get("GlobalFontDirectory", Types_Fields.DICT);
			}
			else
			{
				dir = (DictType) ip.systemdict.get("FontDirectory", Types_Fields.DICT);
			}
			return dir;
		}

		internal static Any mapFontName(Interpreter ip, Any name)
		{
			DictType statusdict = ip.StatusDict;
			DictType fontnames = (DictType) statusdict.get("fontnamedict");
			return fontnames != null ? fontnames.get(name) : null;
		}

		internal static FontDecoder createFontDecoder(Interpreter ip, DictType font)
		{
			try
			{
				int fontType = ((IntegerType) font.get("FontType")).intValue();
				Type clazz = loadFontDecoder(fontType);
				Type[] types = new Type[2];
				types[0] = typeof(Interpreter);
				types[1] = typeof(DictType);
				System.Reflection.ConstructorInfo constructor = clazz.GetConstructor(types);
				object[] @params = new object[2];
				@params[0] = ip;
				@params[1] = font;
				return (FontDecoder) constructor.newInstance(@params);
			}
			catch (Exception ex)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, ex.Message);
			}
		}

		internal static void checkFont(Interpreter ip, DictType font, int fontType)
		{
			try
			{
				Type clazz = loadFontDecoder(fontType);
				Type[] types = new Type[] {typeof(Interpreter), typeof(DictType)};
				System.Reflection.MethodInfo method = clazz.GetMethod("checkFont", types);
				object[] @params = new object[2];
				@params[0] = ip;
				@params[1] = font;
				method.invoke(clazz, @params);
			}
			catch (Exception ex)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, ex.Message + " type: " + fontType);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Class loadFontDecoder(int type) throws ClassNotFoundException
		private static Type loadFontDecoder(int type)
		{
			string name = "com.softhub.ps.Type" + type + "Decoder";
			return Type.GetType(name);
		}

	}

}