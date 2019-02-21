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

	using CharStream = com.softhub.ps.util.CharStream;

	internal sealed class ImageOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"image", "imagemask", "colorimage", "execform", "makepattern"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(ImageOp));
		}

		internal static void image(Interpreter ip)
		{
			Any src = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.FILE | Types_Fields.STRING);
			if (src is DictType)
			{
				image(ip, (DictType) src, false);
			}
			else if (src is ArrayType)
			{
				image(ip, (ArrayType) src);
			}
			else if (src is CharStream)
			{
				image(ip, (CharStream) src);
			}
			else
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "image: " + src.typeName());
			}
		}

		internal static void imagemask(Interpreter ip)
		{
			Any src = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT | Types_Fields.FILE | Types_Fields.STRING);
			if (src is DictType)
			{
				image(ip, (DictType) src, true);
			}
			else if (src is ArrayType)
			{
				imagemask(ip, (ArrayType) src);
			}
			else if (src is CharStream)
			{
				imagemask(ip, (CharStream) src);
			}
			else
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "imagemask: " + src.typeName());
			}
		}

		private static void image(Interpreter ip, ArrayType proc)
		{
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform xform = ((ArrayType) ip.ostack.pop(Types_Fields.ARRAY)).toTransform();
			int bits = ip.ostack.popInteger();
			int height = ip.ostack.popInteger();
			int width = ip.ostack.popInteger();
			if (width < 0 || height < 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			gstate.image(ip, width, height, bits, xform, proc);
		}

		private static void image(Interpreter ip, CharStream src)
		{
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform xform = ((ArrayType) ip.ostack.pop(Types_Fields.ARRAY)).toTransform();
			int bits = ip.ostack.popInteger();
			int height = ip.ostack.popInteger();
			int width = ip.ostack.popInteger();
			if (width < 0 || height < 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			gstate.image(ip, width, height, bits, xform, src);
		}

		private static void imagemask(Interpreter ip, ArrayType proc)
		{
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform xform = ((ArrayType) ip.ostack.pop(Types_Fields.ARRAY)).toTransform();
			bool polarity = ip.ostack.popBoolean();
			int height = ip.ostack.popInteger();
			int width = ip.ostack.popInteger();
			if (width < 0 || height < 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			gstate.imagemask(ip, width, height, 1, polarity, xform, proc);
		}

		private static void imagemask(Interpreter ip, CharStream src)
		{
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform xform = ((ArrayType) ip.ostack.pop(Types_Fields.ARRAY)).toTransform();
			bool polarity = ip.ostack.popBoolean();
			int height = ip.ostack.popInteger();
			int width = ip.ostack.popInteger();
			if (width < 0 || height < 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			gstate.imagemask(ip, width, height, 1, polarity, xform, src);
		}

		private static void image(Interpreter ip, DictType dict, bool mask)
		{
			GraphicsState gstate = ip.GraphicsState;
			int type = ((IntegerType) dict.get("ImageType", Types_Fields.INTEGER)).intValue();
			int width = ((IntegerType) dict.get("Width", Types_Fields.INTEGER)).intValue();
			int height = ((IntegerType) dict.get("Height", Types_Fields.INTEGER)).intValue();
			if (width < 0 || height < 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			int bits = ((IntegerType) dict.get("BitsPerComponent", Types_Fields.INTEGER)).intValue();
			Any any = dict.get("Interpolate");
			bool interpolate = (any is BoolType) ? ((BoolType) any).booleanValue() : false;
			any = dict.get("MultipleDataSources");
			bool mds = (any is BoolType) ? ((BoolType) any).booleanValue() : false;
			ArrayType array = (ArrayType) dict.get("ImageMatrix", Types_Fields.ARRAY);
			if (!array.Matrix)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "ImageMatrix");
			}
			AffineTransform xform = array.toTransform();
			ArrayType decode = (ArrayType) dict.get("Decode", Types_Fields.ARRAY);
			if (mds)
			{
				ArrayType src = (ArrayType) dict.get("DataSource", Types_Fields.ARRAY);
				if (mask)
				{
					gstate.imagemask(ip, width, height, bits, true, xform, src);
				}
				else
				{
					gstate.image(ip, width, height, bits, xform, src);
				}
			}
			else
			{
				Any datasrc = dict.get("DataSource");
				if (datasrc is CharStream)
				{
					CharStream src = (CharStream) datasrc;
					if (mask)
					{
						gstate.imagemask(ip, width, height, bits, true, xform, src);
					}
					else
					{
						gstate.image(ip, width, height, bits, xform, src);
					}
				}
				else if (datasrc is ArrayType)
				{
					ArrayType src = (ArrayType) datasrc;
					if (mask)
					{
						gstate.imagemask(ip, width, height, bits, true, xform, src);
					}
					else
					{
						gstate.image(ip, width, height, bits, xform, src);
					}
				}
				else
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "DataSource: " + datasrc);
				}
			}
		}

		internal static void colorimage(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			int ncomp = ((IntegerType) ip.ostack.pop()).intValue();
			if (!(ncomp == 1 || ncomp == 3 || ncomp == 4))
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT, "ncomp: " + ncomp);
			}
			Any[] src;
			bool multi = ip.ostack.popBoolean();
			if (multi)
			{
				src = new Any[ncomp];
				for (int i = ncomp - 1; i >= 0; i--)
				{
					src[i] = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING | Types_Fields.FILE);
				}
			}
			else
			{
				src = new Any[1];
				src[0] = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.STRING | Types_Fields.FILE);
			}
			AffineTransform xform = ((ArrayType) ip.ostack.pop(Types_Fields.ARRAY)).toTransform();
			int bitsPerComp = ip.ostack.popInteger();
			int height = ip.ostack.popInteger();
			int width = ip.ostack.popInteger();
			if (width < 0 || height < 0)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			gstate.colorimage(ip, width, height, bitsPerComp, ncomp, multi, xform, src);
		}

		internal static void execform(Interpreter ip)
		{
			// TODO: implement form caching
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			ArrayType proc = (ArrayType) dict.get("PaintProc", Types_Fields.ARRAY);
			if (proc.Literal)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "PaintProc");
			}
			ArrayType matrix = (ArrayType) dict.get("Matrix");
			if (!matrix.Matrix)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "Matrix");
			}
			ArrayType bbox = (ArrayType) dict.get("BBox", Types_Fields.ARRAY);
			if (bbox.length() != 4 || !bbox.check(Types_Fields.NUMBER))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "BBox");
			}
			float llx = ((NumberType) bbox.get(0)).floatValue();
			float lly = ((NumberType) bbox.get(1)).floatValue();
			float width = ((NumberType) bbox.get(2)).floatValue() - llx;
			float height = ((NumberType) bbox.get(3)).floatValue() - lly;
			try
			{
				ip.gsave();
				GraphicsState gstate = ip.GraphicsState;
				gstate.concat(matrix.toTransform());
				gstate.rectclip(llx, lly, width, height);
				ip.estack.run(ip, proc);
			}
			finally
			{
				ip.grestore();
			}
		}

		internal static void makepattern(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			if (!matrix.Matrix)
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "matrix");
			}
			// TODO: implement pattern
			DictType pattern = new DictType(ip.vm, dict.length());
			dict.copyTo(ip.vm, pattern);
			ip.ostack.push(pattern);
		}

	}

}