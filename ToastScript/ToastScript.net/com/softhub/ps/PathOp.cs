using System;
using System.Collections.Generic;

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


	internal sealed class PathOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"newpath", "moveto", "rmoveto", "lineto", "rlineto", "curveto", "rcurveto", "arc", "arcn", "arct", "arcto", "closepath", "flattenpath", "reversepath", "strokepath", "initclip", "clippath", "clip", "eoclip", "rectclip", "setbbox", "pathbbox", "pathforall", "fill", "eofill", "stroke", "rectstroke", "rectfill", "uappend", "upath", "ufill", "ueofill", "ustroke", "ustrokepath", "ucache", "ucachestatus"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(PathOp));
		}

		private static Point2D popPoint2D(Interpreter ip)
		{
			double y = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double x = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			return new Point2D.Double(x, y);
		}

		internal static void newpath(Interpreter ip)
		{
			ip.GraphicsState.newpath();
		}

		internal static void moveto(Interpreter ip)
		{
			ip.GraphicsState.moveto(popPoint2D(ip));
		}

		internal static void rmoveto(Interpreter ip)
		{
			ip.GraphicsState.rmoveto(popPoint2D(ip));
		}

		internal static void lineto(Interpreter ip)
		{
			ip.GraphicsState.lineto(popPoint2D(ip));
		}

		internal static void rlineto(Interpreter ip)
		{
			ip.GraphicsState.rlineto(popPoint2D(ip));
		}

		internal static void curveto(Interpreter ip)
		{
			Point2D c = popPoint2D(ip);
			Point2D b = popPoint2D(ip);
			Point2D a = popPoint2D(ip);
			ip.GraphicsState.curveto(a, b, c);
		}

		internal static void rcurveto(Interpreter ip)
		{
			Point2D c = popPoint2D(ip);
			Point2D b = popPoint2D(ip);
			Point2D a = popPoint2D(ip);
			ip.GraphicsState.rcurveto(a, b, c);
		}

		private static void arc(Interpreter ip, bool ccw)
		{
			float ang2 = (float)(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue() * Math.PI / 180);
			float ang1 = (float)(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue() * Math.PI / 180);
			float r = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			Point2D center = popPoint2D(ip);
			ip.GraphicsState.arc(center, r, ang1, ang2, ccw);
		}

		internal static void arc(Interpreter ip)
		{
			arc(ip, true);
		}

		internal static void arcn(Interpreter ip)
		{
			arc(ip, false);
		}

		internal static void arct(Interpreter ip)
		{
			float r = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			Point2D p2 = popPoint2D(ip);
			Point2D p1 = popPoint2D(ip);
			ip.GraphicsState.arct(p1, p2, r);
		}

		internal static void arcto(Interpreter ip)
		{
			float r = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			Point2D p2 = popPoint2D(ip);
			Point2D p1 = popPoint2D(ip);
			double[] result = ip.GraphicsState.arcto(p1, p2, r);
			ip.ostack.pushRef(new RealType(result[0]));
			ip.ostack.pushRef(new RealType(result[1]));
			ip.ostack.pushRef(new RealType(result[2]));
			ip.ostack.pushRef(new RealType(result[3]));
		}

		internal static void closepath(Interpreter ip)
		{
			ip.GraphicsState.closepath();
		}

		internal static void flattenpath(Interpreter ip)
		{
			try
			{
				GraphicsState gstate = ip.GraphicsState;
				AffineTransform ctm = gstate.currentmatrix();
				GeneralPath path = (GeneralPath) gstate.currentpath().clone();
				float flatness = gstate.currentflat();
				float[] coords = new float[6];
				PathIterator iter = path.getPathIterator(ctm.createInverse());
				PathIterator fiter = new FlatteningPathIterator(iter, flatness);
				gstate.newpath();
				while (!fiter.Done)
				{
					switch (fiter.currentSegment(coords))
					{
					case PathIterator.SEG_MOVETO:
						gstate.moveto(coords[0], coords[1]);
						break;
					case PathIterator.SEG_LINETO:
						gstate.lineto(coords[0], coords[1]);
						break;
					case PathIterator.SEG_CLOSE:
						gstate.closepath();
						break;
					default:
						throw new Stop(Stoppable_Fields.INTERNALERROR, "flattenpath");
					}
					fiter.next();
				}
			}
			catch (Exception)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
		}

		internal static void reversepath(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform ctm = gstate.currentmatrix();
			GeneralPath path = gstate.currentpath();
			List<object> v = new List<object>(100);
			float[] coords = new float[6];
			float[] lastmove = new float[2];
			try
			{
				PathIterator iter = path.getPathIterator(ctm.createInverse());
				while (!iter.Done)
				{
					int type = iter.currentSegment(coords);
					switch (type)
					{
					case PathIterator.SEG_MOVETO:
						lastmove[0] = coords[0];
						lastmove[1] = coords[1];
						v.Add(new Segment(type, coords, 2));
						break;
					case PathIterator.SEG_LINETO:
						v.Add(new Segment(type, coords, 2));
						break;
					case PathIterator.SEG_CUBICTO:
						v.Add(new Segment(type, coords, 6));
						break;
					case PathIterator.SEG_CLOSE:
						v.Add(new Segment(type, lastmove, 2));
						break;
					}
					iter.next();
				}
				gstate.newpath();
				Segment lastseg = null;
				int i = v.Count;
				while (--i >= 0)
				{
					Segment seg = (Segment) v[i];
					if (lastseg != null)
					{
						switch (lastseg.type)
						{
						case PathIterator.SEG_MOVETO:
							break;
						case PathIterator.SEG_LINETO:
							gstate.lineto(seg.LastPoint);
							break;
						case PathIterator.SEG_CUBICTO:
							gstate.curveto(lastseg.getPointAt(1), lastseg.getPointAt(0), seg.LastPoint);
							break;
						case PathIterator.SEG_CLOSE:
							gstate.lineto(seg.LastPoint);
							break;
						}
						lastseg = seg;
					}
					else
					{
						do
						{
							lastseg = (Segment) v[--i];
						} while (lastseg.type == PathIterator.SEG_MOVETO);
						gstate.moveto(lastseg.LastPoint);
					}
				}
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
		}

		internal class Segment
		{

			internal int type;
			internal float[] coords;

			internal Segment(int type, float[] coords, int len)
			{
				this.type = type;
				this.coords = new float[len];
				Array.Copy(coords, 0, this.coords, 0, len);
			}

			internal virtual Point2D getPointAt(int index)
			{
				int j = 2 * index;
				return new Point2D.Float(coords[j], coords[j + 1]);
			}

			internal virtual Point2D LastPoint
			{
				get
				{
					switch (type)
					{
					case PathIterator.SEG_MOVETO:
						return new Point2D.Float(coords[0], coords[1]);
					case PathIterator.SEG_LINETO:
						return new Point2D.Float(coords[0], coords[1]);
					case PathIterator.SEG_CUBICTO:
						return new Point2D.Float(coords[4], coords[5]);
					case PathIterator.SEG_CLOSE:
						return new Point2D.Float(coords[0], coords[1]);
					}
					throw new Stop(Stoppable_Fields.INTERNALERROR, "getLastPoint: " + type);
				}
			}

		}

		internal static void strokepath(Interpreter ip)
		{
			ip.GraphicsState.strokepath();
		}

		internal static void initclip(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			gc.initclip(gc.currentdevice());
		}

		internal static void clippath(Interpreter ip)
		{
			ip.GraphicsState.clippath();
		}

		internal static void clip(Interpreter ip)
		{
			ip.GraphicsState.clip(GeneralPath.WIND_NON_ZERO);
		}

		internal static void eoclip(Interpreter ip)
		{
			ip.GraphicsState.clip(GeneralPath.WIND_EVEN_ODD);
		}

		internal static void rectclip(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY | Types_Fields.STRING);
			GraphicsState gstate = ip.GraphicsState;
			float llx, lly, width, height;
			if (any is NumberType)
			{
				height = ((NumberType) any).floatValue();
				width = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				lly = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				llx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				gstate.rectclip(llx, lly, width, height);
			}
			else if (any is ArrayType)
			{
				ArrayType array = (ArrayType) any;
				if (!array.check(Types_Fields.NUMBER))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				gstate.rectclip(array.toFloatArray());
			}
			else
			{
				// TODO: implement encoded string
				throw new Stop(Stoppable_Fields.INTERNALERROR, "<string> rectclip not yet implemented");
			}
		}

		internal static void setbbox(Interpreter ip)
		{
			Point2D ur = popPoint2D(ip);
			Point2D ll = popPoint2D(ip);
	//		ip.getGraphicsState().currentpath().setbbox(ll, ur);
		}

		internal static void pathbbox(Interpreter ip)
		{
			double[] r = ip.GraphicsState.pathbbox();
			ip.ostack.pushRef(new RealType(r[0]));
			ip.ostack.pushRef(new RealType(r[1]));
			ip.ostack.pushRef(new RealType(r[2]));
			ip.ostack.pushRef(new RealType(r[3]));
		}

		internal static void pathforall(Interpreter ip)
		{
			Any close = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.NAME);
			Any curve = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.NAME);
			Any line = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.NAME);
			Any move = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.NAME);
			// Some page descriptions use literal names instead of procs.
			move.cvx();
			line.cvx();
			curve.cvx();
			close.cvx();
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform ctm = gstate.currentmatrix();
			GeneralPath path = gstate.currentpath();
			try
			{
				float[] coords = new float[6];
				PathIterator e = path.getPathIterator(ctm.createInverse());
				while (!e.Done)
				{
					switch (e.currentSegment(coords))
					{
					case PathIterator.SEG_MOVETO:
						ip.ostack.pushRef(new RealType(coords[0]));
						ip.ostack.pushRef(new RealType(coords[1]));
						ip.estack.run(ip, move);
						break;
					case PathIterator.SEG_LINETO:
						ip.ostack.pushRef(new RealType(coords[0]));
						ip.ostack.pushRef(new RealType(coords[1]));
						ip.estack.run(ip, line);
						break;
					case PathIterator.SEG_CUBICTO:
						ip.ostack.pushRef(new RealType(coords[0]));
						ip.ostack.pushRef(new RealType(coords[1]));
						ip.ostack.pushRef(new RealType(coords[2]));
						ip.ostack.pushRef(new RealType(coords[3]));
						ip.ostack.pushRef(new RealType(coords[4]));
						ip.ostack.pushRef(new RealType(coords[5]));
						ip.estack.run(ip, curve);
						break;
					case PathIterator.SEG_CLOSE:
						ip.estack.run(ip, close);
						break;
					}
					e.next();
				}
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
		}

		internal static void fill(Interpreter ip)
		{
			ip.GraphicsState.fill(GeneralPath.WIND_NON_ZERO);
		}

		internal static void eofill(Interpreter ip)
		{
			ip.GraphicsState.fill(GeneralPath.WIND_EVEN_ODD);
		}

		internal static void stroke(Interpreter ip)
		{
			ip.GraphicsState.stroke();
		}

		internal static void rectstroke(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY | Types_Fields.STRING);
			GraphicsState gstate = ip.GraphicsState;
			float llx, lly, width, height;
			if (any is NumberType)
			{
				height = ((NumberType) any).floatValue();
				width = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				lly = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				llx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				gstate.rectstroke(llx, lly, width, height);
			}
			else if (any is ArrayType)
			{
				// TODO: implement matrix case
				ArrayType array = (ArrayType) any;
				if (!array.check(Types_Fields.NUMBER))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
	//			gstate.rectfill(array.toFloatArray());
				// TODO: implement multiple rectangles
				throw new Stop(Stoppable_Fields.INTERNALERROR, "<array> rectfill not yet implemented");
			}
			else
			{
				// TODO: implement encoded string
				throw new Stop(Stoppable_Fields.INTERNALERROR, "<string> rectfill not yet implemented");
			}
		}

		internal static void rectfill(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY | Types_Fields.STRING);
			GraphicsState gstate = ip.GraphicsState;
			float llx, lly, width, height;
			if (any is NumberType)
			{
				height = ((NumberType) any).floatValue();
				width = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				lly = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				llx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				gstate.rectfill(llx, lly, width, height);
			}
			else if (any is ArrayType)
			{
				ArrayType array = (ArrayType) any;
				if (!array.check(Types_Fields.NUMBER))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
	//			gstate.rectfill(array.toFloatArray());
				// TODO: implement multiple rectangles
				throw new Stop(Stoppable_Fields.INTERNALERROR, "<array> rectfill not yet implemented");
			}
			else
			{
				// TODO: implement encoded string
				throw new Stop(Stoppable_Fields.INTERNALERROR, "<string> rectfill not yet implemented");
			}
		}

		internal static void ucache(Interpreter ip)
		{
			// TODO: implement ucache
		}

		internal static void ucachestatus(Interpreter ip)
		{
			int bsize = 1024;
			int bmax = 1024;
			int rsize = 100;
			int rmax = 100;
			int blimit = 1024;
			ip.ostack.pushRef(new MarkType());
			ip.ostack.pushRef(new IntegerType(bsize));
			ip.ostack.pushRef(new IntegerType(bmax));
			ip.ostack.pushRef(new IntegerType(rsize));
			ip.ostack.pushRef(new IntegerType(rmax));
			ip.ostack.pushRef(new IntegerType(blimit));
		}

		internal static void upath(Interpreter ip)
		{
			bool ucache = ip.ostack.popBoolean();
			GraphicsState gstate = ip.GraphicsState;
			AffineTransform ctm = gstate.currentmatrix();
			GeneralPath path = gstate.currentpath();
			try
			{
				float[] coords = new float[6];
				AffineTransform inverse = ctm.createInverse();
				ip.ostack.pushRef(new MarkType());
				if (ucache)
				{
					ip.ostack.pushRef((new NameType("ucache")).cvx());
				}
				PathIterator e = path.getPathIterator(inverse);
				while (!e.Done)
				{
					switch (e.currentSegment(coords))
					{
					case PathIterator.SEG_MOVETO:
						ip.ostack.pushRef(new RealType(coords[0]));
						ip.ostack.pushRef(new RealType(coords[1]));
						ip.ostack.pushRef((new NameType("moveto")).cvx());
						break;
					case PathIterator.SEG_LINETO:
						ip.ostack.pushRef(new RealType(coords[0]));
						ip.ostack.pushRef(new RealType(coords[1]));
						ip.ostack.pushRef((new NameType("lineto")).cvx());
						break;
					case PathIterator.SEG_CUBICTO:
						ip.ostack.pushRef(new RealType(coords[0]));
						ip.ostack.pushRef(new RealType(coords[1]));
						ip.ostack.pushRef(new RealType(coords[2]));
						ip.ostack.pushRef(new RealType(coords[3]));
						ip.ostack.pushRef(new RealType(coords[4]));
						ip.ostack.pushRef(new RealType(coords[5]));
						ip.ostack.pushRef((new NameType("curveto")).cvx());
						break;
					case PathIterator.SEG_CLOSE:
						ip.ostack.pushRef((new NameType("closepath")).cvx());
						break;
					}
					e.next();
				}
				int n = ip.ostack.counttomark();
				ArrayType array = new ArrayType(ip.vm, n, ip.ostack);
				ip.ostack.cleartomark();
				ip.ostack.pushRef(array);
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
		}

		internal static void uappend(Interpreter ip)
		{
			ArrayType array = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			array.cvx();
			ip.estack.run(ip, array);
		}

		internal static void ufill(Interpreter ip)
		{
			upaint(ip, 0);
		}

		internal static void ueofill(Interpreter ip)
		{
			upaint(ip, 1);
		}

		internal static void ustroke(Interpreter ip)
		{
			upaint(ip, 2);
		}

		internal static void ustrokepath(Interpreter ip)
		{
			upaint(ip, 3);
		}

		private static void upaint(Interpreter ip, int operation)
		{
			ArrayType array = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			AffineTransform xform = null;
			if (array.Matrix)
			{
				xform = array.toTransform();
				array = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			}
			if (operation != 3)
			{
				ip.gsave();
			}
			GraphicsState gstate = ip.GraphicsState;
			gstate.newpath();
			array.cvx();
			ip.estack.run(ip, array);
			AffineTransform ctm = null;
			if (xform != null)
			{
				ctm = gstate.currentmatrix();
				gstate.concat(xform);
			}
			switch (operation)
			{
			case 0:
				gstate.fill(GeneralPath.WIND_NON_ZERO);
				break;
			case 1:
				gstate.fill(GeneralPath.WIND_EVEN_ODD);
				break;
			case 2:
				gstate.stroke();
				break;
			case 3:
				gstate.strokepath();
				if (ctm != null)
				{
					gstate.setmatrix(ctm);
				}
				break;
			}
			if (operation != 3)
			{
				ip.grestore();
			}
		}

	}

}