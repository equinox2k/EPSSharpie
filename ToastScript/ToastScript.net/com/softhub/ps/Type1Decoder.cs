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

	using BinaryCodec = com.softhub.ps.filter.BinaryCodec;
	using CharStream = com.softhub.ps.util.CharStream;
	using CharWidth = com.softhub.ps.util.CharWidth;

	public class Type1Decoder : AbstractFontDecoder
	{

		public bool debug = false;

		// The command set:
		internal const sbyte HSTEM = 1;
		internal const sbyte VSTEM = 3;
		internal const sbyte VMOVETO = 4;
		internal const sbyte RLINETO = 5;
		internal const sbyte HLINETO = 6;
		internal const sbyte VLINETO = 7;
		internal const sbyte RCURVETO = 8;
		internal const sbyte CLOSEPATH = 9;
		internal const sbyte CALLSUBR = 10;
		internal const sbyte RETURN = 11;
		internal const sbyte ESCAPE = 12;
		internal const sbyte HSBW = 13;
		internal const sbyte ENDCHAR = 14;
		internal const sbyte RMOVETO = 21;
		internal const sbyte HMOVETO = 22;
		internal const sbyte VHCURVETO = 30;
		internal const sbyte HVCURVETO = 31;
		// positive 2 byte integers
		internal static readonly sbyte POS20 = unchecked((sbyte) 247);
		internal static readonly sbyte POS21 = unchecked((sbyte) 248);
		internal static readonly sbyte POS22 = unchecked((sbyte) 249);
		internal static readonly sbyte POS23 = unchecked((sbyte) 250);
		// negative 2 byte integers
		internal static readonly sbyte NEG20 = unchecked((sbyte) 251);
		internal static readonly sbyte NEG21 = unchecked((sbyte) 252);
		internal static readonly sbyte NEG22 = unchecked((sbyte) 253);
		internal static readonly sbyte NEG23 = unchecked((sbyte) 254);
		// 4 byte integers
		internal static readonly sbyte NUM4 = unchecked((sbyte) 255);

		// The extended command set:
		internal const sbyte EX_DOTSECTION = 0;
		internal const sbyte EX_VSTEM3 = 1;
		internal const sbyte EX_HSTEM3 = 2;
		internal const sbyte EX_SEAC = 6;
		internal const sbyte EX_SBW = 7;
		internal const sbyte EX_DIV = 12;
		internal const sbyte EX_CALLOTHERSUBR = 16;
		internal const sbyte EX_POP = 17;
		internal const sbyte EX_SETCURRENTPOINT = 33;

		/// <summary>
		/// The glyph dictionary.
		/// </summary>
		private DictType charstringdict;

		/// <summary>
		/// The "Private" dictionary.
		/// </summary>
		private DictType privatedict;

		/// <summary>
		/// The paint type.
		/// </summary>
		private int paintType;

		/// <summary>
		/// The stroke width.
		/// </summary>
		private float strokeWidth;

		/// <summary>
		/// The subroutines.
		/// </summary>
		private ArrayType subroutines;

		/// <summary>
		/// The skip count for decoding.
		/// </summary>
		private int skipcount = 4;

		/// <summary>
		/// The parameter stack.
		/// </summary>
		private int[] charstack = new int[128];

		/// <summary>
		/// The parameter stack pointer.
		/// </summary>
		private int charstackindex;

		public Type1Decoder(Interpreter ip, DictType font) : base(ip, font)
		{
			charstringdict = (DictType) font.get("CharStrings");
			privatedict = (DictType) font.get("Private");
			paintType = ((IntegerType) font.get("PaintType")).intValue();
			int flags = privatedict.saveAccessFlags();
			Any skip = privatedict.get("lenIV");
			Any subrs = privatedict.get("Subrs");
			privatedict.restoreAccessFlags(flags);
			if (skip is IntegerType)
			{
				skipcount = ((IntegerType) skip).intValue();
			}
			if (subrs is ArrayType)
			{
				subroutines = (ArrayType) subrs;
			}
			Any optStrokeWidth = font.get("StrokeWidth");
			if (optStrokeWidth is NumberType)
			{
				strokeWidth = ((NumberType) optStrokeWidth).floatValue();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override object clone()
		{
			return (Type1Decoder) base.clone();
		}

		public static void checkFont(Interpreter ip, DictType font)
		{
			ArrayType fontBBox = (ArrayType) font.get("FontBBox", Types_Fields.ARRAY);
			if (!fontBBox.check(Types_Fields.NUMBER, 4))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "FontBBox");
			}
			font.get("Encoding", Types_Fields.ARRAY);
			ArrayType fontMatrix = (ArrayType) font.get("FontMatrix", Types_Fields.ARRAY);
			if (!fontMatrix.Matrix)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "FontMatrix");
			}
			font.get("Private", Types_Fields.DICT);
			font.get("PaintType", Types_Fields.INTEGER);
		}

		public override CharWidth buildchar(Interpreter ip, int index, bool render)
		{
			AffineTransform fx = FontMatrix;
			ip.gsave();
			try
			{
				GraphicsState gstate = ip.GraphicsState;
				if (render)
				{
					Point2D curpt = gstate.currentpoint();
					gstate.translate(curpt.X, curpt.Y);
				}
				gstate.concat(fx);
				gstate.newpath();
				gstate.moveto(0,0);
				buildglyph(ip, index);
				if (render)
				{
					gstate.setcachedevice(new CharWidth(charwidth_Renamed), FontBBox);
					if (paintType == 2)
					{
						gstate.setlinewidth(strokeWidth);
						gstate.stroke();
					}
					else
					{
						gstate.fill(GeneralPath.WIND_NON_ZERO);
					}
				}
			}
			finally
			{
				ip.grestore();
			}
			if (debug)
			{
				System.Console.Error.WriteLine("\nstack count: " + count());
			}
			return charwidth_Renamed.transform(fx);
		}

		public override void buildglyph(Interpreter ip, int index)
		{
			Any ch = encode(index);
			if (ch == null || ch is NullType)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad encoding vector");
			}
			Any charstr = charstringdict.get(ch);
			if (!(charstr is StringType))
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "buildglyph: [" + index + " " + ch + " " + charstr + "]");
			}
			clear();
			charwidth_Renamed.init();
			try
			{
				GraphicsState gstate = ip.GraphicsState;
				buildglyph(gstate, new CharFilter((StringType) charstr));
				CharWidth cw = getMetrics(ip, index);
				if (cw != null)
				{
					charwidth_Renamed = cw;
				}
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "bad char strings");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void buildglyph(GraphicsState gstate, CharFilter param) throws java.io.IOException
		private void buildglyph(GraphicsState gstate, CharFilter param)
		{
			bool done = false;
			for (int i = 0; i < skipcount; i++)
			{
				param.decode();
			}
			int c;
			while (!done && (c = param.decode()) >= 0)
			{
				done = exec(gstate, c, param);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean exec(GraphicsState gstate, int cmd, CharFilter param) throws java.io.IOException
		private bool exec(GraphicsState gstate, int cmd, CharFilter param)
		{
			if (debug)
			{
				System.Console.Error.WriteLine("cmd: " + cmd);
			}
			switch (cmd)
			{
			case HSTEM:
				exec_hstem(gstate);
				break;
			case VSTEM:
				exec_vstem(gstate);
				break;
			case VMOVETO:
				exec_vmoveto(gstate);
				break;
			case RLINETO:
				exec_rlineto(gstate);
				break;
			case HLINETO:
				exec_hlineto(gstate);
				break;
			case VLINETO:
				exec_vlineto(gstate);
				break;
			case RCURVETO:
				exec_rcurveto(gstate);
				break;
			case CLOSEPATH:
				gstate.closepath();
				break;
			case CALLSUBR:
				exec_callsubr(gstate);
				break;
			case RETURN:
				exec_return(gstate);
				break;
			case ESCAPE:
				exec_escape(gstate, param);
				break;
			case HSBW:
				exec_hsbw(gstate);
				break;
			case ENDCHAR:
				return true;
			case RMOVETO:
				exec_rmoveto(gstate);
				break;
			case HMOVETO:
				exec_hmoveto(gstate);
				break;
			case VHCURVETO:
				exec_vhcurveto(gstate);
				break;
			case HVCURVETO:
				exec_hvcurveto(gstate);
				break;
			default:
				push(cmd, param);
				break;
			}
			if (debug)
			{
				System.Console.Error.WriteLine();
			}
			return false;
		}

		private void exec_hstem(GraphicsState gstate)
		{
			int y = pop();
			int x = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" hstem: " + x + " " + y);
			}
		}

		private void exec_vstem(GraphicsState gstate)
		{
			int y = pop();
			int x = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" vstem: " + x + " " + y);
			}
		}

		private void exec_vmoveto(GraphicsState gstate)
		{
			int y = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" vmoveto: " + y);
			}
			gstate.rmoveto(0, y);
		}

		private void exec_rlineto(GraphicsState gstate)
		{
			int y = pop();
			int x = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" rlineto: " + x + " " + y);
			}
			gstate.rlineto(x, y);
		}

		private void exec_hlineto(GraphicsState gstate)
		{
			int x = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" hlineto: " + x);
			}
			gstate.rlineto(x, 0);
		}

		private void exec_vlineto(GraphicsState gstate)
		{
			int y = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" vlineto: " + y);
			}
			gstate.rlineto(0, y);
		}

		private void exec_rcurveto(GraphicsState gstate)
		{
			int y3 = pop();
			int x3 = pop();
			int y2 = pop();
			int x2 = pop();
			int y1 = pop();
			int x1 = pop();
			int ax2 = x1 + x2;
			int ay2 = y1 + y2;
			int ax3 = ax2 + x3;
			int ay3 = ay2 + y3;
			gstate.rcurveto(new Point2D.Float(x1, y1), new Point2D.Float(ax2, ay2), new Point2D.Float(ax3, ay3)
		   );
			if (debug)
			{
				System.Console.Error.WriteLine(" rcurveto: " + x1 + " " + y1 + ", " + ax2 + " " + ay2 + ", " + ax3 + " " + ay3);
			}
		}

		private void exec_callsubr(GraphicsState gstate)
		{
			int index = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" callsubr: " + index);
			}
			int flags = subroutines.saveAccessFlags();
			try
			{
				Any any = subroutines.get(index);
				subroutines.restoreAccessFlags(flags);
				if (!(any is StringType))
				{
					throw new Stop(Stoppable_Fields.INVALIDFONT, "callsubr");
				}
				buildglyph(gstate, new CharFilter((StringType) any));
			}
			catch (IOException)
			{
				subroutines.restoreAccessFlags(flags);
				throw new Stop(Stoppable_Fields.INVALIDFONT, "exec_callsubr: io-error");
			}
		}

		private void exec_return(GraphicsState gstate)
		{
			if (debug)
			{
				System.Console.Error.WriteLine(" return");
			}
		}

		private void exec_hsbw(GraphicsState gstate)
		{
			switch (count())
			{
			case 1:
				pop_sx();
				break;
			case 2:
				pop_wx_sx();
				break;
			case 4:
				pop_wy_wx_sy_sx();
				break;
			default:
				throw new Stop(Stoppable_Fields.INVALIDFONT, "hsbw");
			}
			if (debug)
			{
				System.Console.Error.WriteLine(" hsbw: " + charwidth_Renamed);
			}
			float sx = charwidth_Renamed.SideBearingX;
			float sy = charwidth_Renamed.SideBearingY;
			gstate.rmoveto(sx, sy);
		}

		private void pop_sx()
		{
			int sx = pop();
			charwidth_Renamed.Width = sx;
		}

		private void pop_wx_sx()
		{
			int wx = pop();
			int sx = pop();
			charwidth_Renamed.setWidth(sx, wx);
		}

		private void pop_wy_wx_sy_sx()
		{
			int wy = pop();
			int wx = pop();
			int sy = pop();
			int sx = pop();
			charwidth_Renamed.setWidth(sx, sy, wx, wy);
		}

		private void exec_rmoveto(GraphicsState gstate)
		{
			int y = pop();
			int x = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" rmoveto: " + x + " " + y);
			}
			gstate.rmoveto(x, y);
		}

		private void exec_hmoveto(GraphicsState gstate)
		{
			int x = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" hmoveto: " + x);
			}
			gstate.rmoveto(x, 0);
		}

		private void exec_vhcurveto(GraphicsState gstate)
		{
			int x3 = pop();
			int y2 = pop();
			int x2 = pop();
			int y1 = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" vhcurveto: " + y1 + " " + x2 + ", " + y2 + " " + x3);
			}
			int ay2 = y1 + y2;
			gstate.rcurveto(new Point2D.Float(0, y1), new Point2D.Float(x2, ay2), new Point2D.Float(x2 + x3, ay2)
		   );
		}

		private void exec_hvcurveto(GraphicsState gstate)
		{
			int y3 = pop();
			int y2 = pop();
			int x2 = pop();
			int x1 = pop();
			if (debug)
			{
				System.Console.Error.WriteLine(" hvcurveto: " + x1 + " " + x2 + ", " + y2 + " " + y3);
			}
			int ax2 = x1 + x2;
			gstate.rcurveto(new Point2D.Float(x1, 0), new Point2D.Float(ax2, y2), new Point2D.Float(ax2, y2 + y3)
		   );
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void exec_escape(GraphicsState gstate, CharFilter param) throws java.io.IOException
		private void exec_escape(GraphicsState gstate, CharFilter param)
		{
			int cmd = param.decode();
			if (debug)
			{
				System.Console.Error.WriteLine(" escape: " + cmd);
			}
			switch (cmd)
			{
			case EX_DOTSECTION:
				break;
			case EX_VSTEM3:
				pop();
				pop();
				pop();
				pop();
				pop();
				pop();
				break;
			case EX_HSTEM3:
				pop();
				pop();
				pop();
				pop();
				pop();
				pop();
				break;
			case EX_SEAC:
				pop();
				pop();
				pop();
				pop();
				break;
			case EX_SBW:
				break;
			case EX_DIV:
				pop();
				break;
			case EX_CALLOTHERSUBR:
				ex_callothersubr();
				break;
			case EX_POP:
				pop();
				break;
			case EX_SETCURRENTPOINT:
				ex_setcurrentpoint(gstate);
				break;
			}
		}

		private void ex_callothersubr()
		{
			int cmd = top();
			switch (cmd)
			{
			default:
				if (debug)
				{
					System.Console.Error.WriteLine(" ex_cmd: " + cmd + " ");
				}
			break;
			}
		}

		private void ex_setcurrentpoint(GraphicsState gstate)
		{
			int y = pop();
			int x = pop();
			gstate.moveto(x, y);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void push(int cmd, CharFilter param) throws java.io.IOException
		private void push(int cmd, CharFilter param)
		{
			int num = 0;
			if (cmd >= 32 && cmd < 247)
			{
				num = cmd - 139;
			}
			else if (cmd >= 247 && cmd <= 250)
			{
				num = (cmd - 247 << 8) + 108 + param.decode();
			}
			else if (cmd >= 251 && cmd <= 254)
			{
				num = -((cmd - 251 << 8) + 108 + param.decode());
			}
			else if (cmd == 255)
			{
				for (int i = 0; i < 4; i++)
				{
					num = (num << 8) + param.decode();
				}
			}
			else
			{
				if (debug)
				{
					System.Console.Error.WriteLine(" bad: " + num);
				}
			}
			if (charstackindex >= charstack.Length)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "stack overflow");
			}
			charstack[charstackindex++] = num;
		}

		private int pop()
		{
			if (charstackindex <= 0)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "stack underflow");
			}
			return charstack[--charstackindex];
		}

		private int top()
		{
			if (charstackindex <= 0)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "stack underflow");
			}
			return charstack[charstackindex - 1];
		}

		private void clear()
		{
			charstackindex = 0;
		}

		private int count()
		{
			return charstackindex;
		}

		private string stackTrace()
		{
			string s = "";
			for (int i = 0; i < charstackindex; i++)
			{
				s += charstack[i] + " ";
			}
			return s;
		}

		private void print(string msg)
		{
			if (debug)
			{
				System.Console.Write(msg);
			}
		}

		internal class CharFilter : BinaryCodec
		{

			public CharFilter(StringType charstring) : base(CHARSTRING_SEED)
			{
				try
				{
					open((CharStream) charstring.clone(), com.softhub.ps.util.CharStream_Fields.READ_MODE); // TODO: why clone?
				}
				catch (Exception)
				{
					throw new Stop(Stoppable_Fields.INTERNALERROR, "CharFilter");
				}
			}

		}

	}

}