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

	using Bitmap = com.softhub.ps.device.Bitmap;
	using CacheDevice = com.softhub.ps.device.CacheDevice;
	using Device = com.softhub.ps.device.Device;
	using NullDevice = com.softhub.ps.device.NullDevice;
	using PageDevice = com.softhub.ps.device.PageDevice;
	using PageEvent = com.softhub.ps.device.PageEvent;
	using PageEventListener = com.softhub.ps.device.PageEventListener;
	using PixelSourceFactory = com.softhub.ps.image.PixelSourceFactory;
	using CMYKPixelSource = com.softhub.ps.image.CMYKPixelSource;
	using GrayPixelSource = com.softhub.ps.image.GrayPixelSource;
	using RGBPixelSource = com.softhub.ps.image.RGBPixelSource;
	using CharStream = com.softhub.ps.util.CharStream;
	using CharWidth = com.softhub.ps.util.CharWidth;

	public class GraphicsState : GStateType, PageEventListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			defaultdevice_Renamed = nulldevice_Renamed;
			pagedevice_Renamed = nulldevice_Renamed;
			currentdevice_Renamed = nulldevice_Renamed;
		}


		private const double SMALL_NUM = 1e-7;
		private const float DEFAULT_FLATNESS = 1;
		private const float DEFAULT_MITERLIMIT = 10;
		private static readonly double TWO_PI = 2.0 * Math.PI;
		private static readonly Screen NULL_SCREEN = new Screen(0, 0, null);
		private static readonly AffineTransform IDENT_MATRIX = new AffineTransform();

		/// <summary>
		/// The null device.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private NullDevice nulldevice_Renamed = new NullDevice();

		/// <summary>
		/// The cache device.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private CacheDevice cachedevice_Renamed = new CacheDevice();

		/// <summary>
		/// The default page device.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Device defaultdevice_Renamed;

		/// <summary>
		/// The page device.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Device pagedevice_Renamed;

		/// <summary>
		/// The current output device.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Device currentdevice_Renamed;

		/// <summary>
		/// The current transformation matrix.
		/// </summary>
		private AffineTransform ctm = new AffineTransform();

		/// <summary>
		/// The current path.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private GeneralPath currentpath_Renamed = new GeneralPath();

		/// <summary>
		/// The current point.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Point2D currentpoint_Renamed;

		/// <summary>
		/// Stroke changed flag.
		/// </summary>
		private bool strokeChanged;

		/// <summary>
		/// The current line width.
		/// </summary>
		private float currentwidth;

		/// <summary>
		/// The current line cap.
		/// </summary>
		private int currentcap;

		/// <summary>
		/// The current line join.
		/// </summary>
		private int currentjoin;

		/// <summary>
		/// The current dash array.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ArrayType currentdasharray_Renamed;

		/// <summary>
		/// The current dash offset.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private float currentdashphase_Renamed;

		/// <summary>
		/// The current flatness for curves.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private float currentflat_Renamed = DEFAULT_FLATNESS;

		/// <summary>
		/// The current miterlimit.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private float currentmiterlimit_Renamed = DEFAULT_MITERLIMIT;

		/// <summary>
		/// The current stroke adjustment.
		/// </summary>
		private bool strokeAdjustment;

		/// <summary>
		/// The current overprint.
		/// </summary>
		private bool overprint;

		/// <summary>
		/// The current gray.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private float currentgray_Renamed;

		/// <summary>
		/// The current red rgb color.
		/// </summary>
		private float currentred;

		/// <summary>
		/// The current green rgb color.
		/// </summary>
		private float currentgreen;

		/// <summary>
		/// The current blue rgb color.
		/// </summary>
		private float currentblue;

		/// <summary>
		/// The current halftone phase in X.
		/// </summary>
		private int halftonePhaseX;

		/// <summary>
		/// The current halftone phase in Y.
		/// </summary>
		private int halftonePhaseY;

		/// <summary>
		/// The gray halftone dictionary.
		/// </summary>
		private DictType halftone;

		/// <summary>
		/// The current halftone screen.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Screen currentredscreen_Renamed;

		/// <summary>
		/// The current halftone screen.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Screen currentgreenscreen_Renamed;

		/// <summary>
		/// The current halftone screen.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private Screen currentbluescreen_Renamed;

		/// <summary>
		/// The pattern dictionary.
		/// </summary>
		private DictType pattern;

		/// <summary>
		/// The color rendering dictionary.
		/// </summary>
		private DictType colorRendering;

		/// <summary>
		/// The current gray color transfer proc.
		/// </summary>
		private ArrayType currentgraytransfer;

		/// <summary>
		/// The current red color transfer proc.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ArrayType currentredtransfer_Renamed;

		/// <summary>
		/// The current green color transfer proc.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ArrayType currentgreentransfer_Renamed;

		/// <summary>
		/// The current blue color transfer proc.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ArrayType currentbluetransfer_Renamed;

		/// <summary>
		/// The current black generation proc.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ArrayType currentblackgeneration_Renamed;

		/// <summary>
		/// The current under color removal proc.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private ArrayType currentundercolorremoval_Renamed;

		/// <summary>
		/// The current colorspace.
		/// </summary>
		private ColorSpace colorspace;

		/// <summary>
		/// The font decoder.
		/// </summary>
		private FontDecoder fontdecoder;

		/// <summary>
		/// Bitmap font request flag.
		/// </summary>
		private bool bitmapwidths;

		/// <summary>
		/// The system font dictionary.
		/// </summary>
		private DictType systemfontdict;

		/// <summary>
		/// The device state.
		/// </summary>
		private object deviceState;

		/// <summary>
		/// Create graphics state.
		/// </summary>
		public GraphicsState()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Called after gsave has pushed graphics state
		/// onto gstack. </summary>
		/// <param name="level"> the current save level </param>
		public override void save(int level)
		{
			deviceState = currentdevice_Renamed.save();
			SaveLevel = level;
		}

		/// <summary>
		/// Called after grestore has popped graphics state
		/// from gstack. </summary>
		/// <param name="gstate"> the graphics state to restore to </param>
		public override void restore(GStateType gstate)
		{
			GraphicsState gc = (GraphicsState) gstate;
			if (gc.currentdevice_Renamed is CacheDevice)
			{
				// flush the cache device
				AffineTransform xform = (AffineTransform) gc.ctm.clone();
				cachedevice_Renamed.flush(gc.fontdecoder, xform);
			}
			currentdevice_Renamed.restore(gc.deviceState);
			strokeChanged = gc.strokeChanged;
		}

		/// <returns> a clone of the graphics state </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override object clone()
		{
			GraphicsState gc = (GraphicsState) base.clone();
			gc.ctm = (AffineTransform) ctm.clone();
			if (gc.currentpath_Renamed != null)
			{
				gc.currentpath_Renamed = (GeneralPath) currentpath_Renamed.clone();
			}
			if (gc.currentpoint_Renamed != null)
			{
				gc.currentpoint_Renamed = (Point2D) currentpoint_Renamed.clone();
			}
			if (currentdasharray_Renamed != null)
			{
				gc.currentdasharray_Renamed = (ArrayType) currentdasharray_Renamed.clone();
			}
			// clone the halftone screens
			if (gc.currentredscreen_Renamed != null)
			{
				gc.currentredscreen_Renamed = (Screen) currentredscreen_Renamed.clone();
			}
			if (gc.currentgreenscreen_Renamed != null)
			{
				gc.currentgreenscreen_Renamed = (Screen) currentgreenscreen_Renamed.clone();
			}
			if (gc.currentbluescreen_Renamed != null)
			{
				gc.currentbluescreen_Renamed = (Screen) currentbluescreen_Renamed.clone();
			}
			// clone the halftone
			if (gc.halftone != null)
			{
				gc.halftone = (DictType) halftone.clone();
			}
			// clone the pattern
			if (gc.pattern != null)
			{
				gc.pattern = (DictType) pattern.clone();
			}
			// clone the pattern
			if (gc.colorRendering != null)
			{
				gc.colorRendering = (DictType) colorRendering.clone();
			}
			// clone the color transfer functions
			if (currentredtransfer_Renamed != null)
			{
				gc.currentredtransfer_Renamed = (ArrayType) currentredtransfer_Renamed.clone();
			}
			if (currentgreentransfer_Renamed != null)
			{
				gc.currentgreentransfer_Renamed = (ArrayType) currentgreentransfer_Renamed.clone();
			}
			if (currentbluetransfer_Renamed != null)
			{
				gc.currentbluetransfer_Renamed = (ArrayType) currentbluetransfer_Renamed.clone();
			}
			if (currentgraytransfer != null)
			{
				gc.currentgraytransfer = (ArrayType) currentgraytransfer.clone();
			}
			if (currentblackgeneration_Renamed != null)
			{
				gc.currentblackgeneration_Renamed = (ArrayType) currentblackgeneration_Renamed.clone();
			}
			if (currentundercolorremoval_Renamed != null)
			{
				gc.currentundercolorremoval_Renamed = (ArrayType) currentundercolorremoval_Renamed.clone();
			}
			if (fontdecoder != null)
			{
				gc.fontdecoder = (FontDecoder) fontdecoder.clone();
			}
			return gc;
		}

		/// <summary>
		/// Install device driver. </summary>
		/// <param name="device"> the device </param>
		public virtual void install(Device device)
		{
			defaultdevice_Renamed = pagedevice_Renamed = device;
		}

		/// <summary>
		/// Initialize the graphics state.
		/// </summary>
		public virtual void initgraphics()
		{
			currentdevice_Renamed.init();
			initclip(currentdevice_Renamed);
			initmatrix(currentdevice_Renamed);
			newpath();
			currentwidth = 0;
			currentcap = 0;
			currentjoin = 0;
			currentdashphase_Renamed = 0;
			currentdasharray_Renamed = null;
			strokeChanged = true;
			setgray(0);
			setmiterlimit(10);
		}

		private void setdevice(Device device)
		{
			currentdevice_Renamed = device;
			device.init();
		}

		public virtual void pageDeviceChanged(PageEvent evt)
		{
			int type = evt.Type;
			if (type == PageEvent.RESIZE)
			{
				PageDevice device = evt.PageDevice;
				initmatrix(device);
			}
		}

		/// <summary>
		/// Set the page device and make it the current
		/// output device. </summary>
		/// <param name="devic"> the new page device </param>
		public virtual void setpagedevice(Device device)
		{
			pagedevice_Renamed = currentdevice_Renamed = device;
			initgraphics();
		}

		/// <returns> the page device </returns>
		public virtual Device pagedevice()
		{
			return pagedevice_Renamed;
		}

		/// <returns> the default page device </returns>
		public virtual Device defaultdevice()
		{
			return defaultdevice_Renamed;
		}

		/// <returns> the current output device </returns>
		public virtual Device currentdevice()
		{
			return currentdevice_Renamed;
		}

		/// <returns> the cache device </returns>
		public virtual CacheDevice cachedevice()
		{
			return cachedevice_Renamed;
		}

		/// <returns> the current path </returns>
		public virtual GeneralPath currentpath()
		{
			return currentpath_Renamed;
		}

		/// <summary>
		/// Initialize the current transformation
		/// matrix (CTM). </summary>
		/// <param name="device"> the output device </param>
		public virtual void initmatrix(Device device)
		{
			setmatrix(device.DefaultMatrix);
		}

		/// <returns> the current matrix (CTM) </returns>
		public virtual AffineTransform currentmatrix()
		{
			return ctm;
		}

		public virtual void setmatrix(AffineTransform xform)
		{
			ctm.Transform = xform;
		}

		public virtual void translate(double tx, double ty)
		{
			ctm.translate(tx, ty);
		}

		public virtual void scale(double sx, double sy)
		{
			ctm.scale(sx, sy);
		}

		public virtual void rotate(double angle)
		{
			ctm.rotate(angle);
		}

		public virtual void concat(AffineTransform xform)
		{
			ctm.concatenate(xform);
		}

		public virtual void newpath()
		{
			currentpath_Renamed = new GeneralPath();
			currentpoint_Renamed = null;
		}

		public virtual Point2D currentpoint()
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT);
			}
			Point2D icurpt = null;
			try
			{
				icurpt = ctm.inverseTransform(currentpoint_Renamed, null);
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			return icurpt;
		}

		public virtual Point2D currentdevicepoint()
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "currentdevicepoint");
			}
			return currentpoint_Renamed;
		}

		public virtual void moveto(double x, double y)
		{
			moveto(new Point2D.Double(x, y));
		}

		public virtual void moveto(Point2D pt)
		{
			currentpoint_Renamed = ctm.transform(pt, null);
			currentpath_Renamed.moveTo((float) currentpoint_Renamed.X, (float) currentpoint_Renamed.Y);
		}

		public virtual void rmoveto(double x, double y)
		{
			rmoveto(new Point2D.Double(x, y));
		}

		public virtual void rmoveto(Point2D v)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "rmoveto");
			}
			Point2D dv = ctm.deltaTransform(v, null);
			double x = currentpoint_Renamed.X + dv.X;
			double y = currentpoint_Renamed.Y + dv.Y;
			currentpoint_Renamed = new Point2D.Double(x, y);
			currentpath_Renamed.moveTo((float) x, (float) y);
		}

		public virtual void lineto(double x, double y)
		{
			lineto(new Point2D.Double(x, y));
		}

		public virtual void lineto(Point2D pt)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "lineto");
			}
			currentpoint_Renamed = ctm.transform(pt, null);
			currentpath_Renamed.lineTo((float) currentpoint_Renamed.X, (float) currentpoint_Renamed.Y);
		}

		public virtual void rlineto(double x, double y)
		{
			rlineto(new Point2D.Double(x, y));
		}

		public virtual void rlineto(Point2D v)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "rlineto");
			}
			Point2D dv = ctm.deltaTransform(v, null);
			double x = currentpoint_Renamed.X + dv.X;
			double y = currentpoint_Renamed.Y + dv.Y;
			currentpoint_Renamed = new Point2D.Double(x, y);
			currentpath_Renamed.lineTo((float) x, (float) y);
		}

		public virtual void curveto(Point2D p1, Point2D p2, Point2D p3)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "curveto");
			}
			Point2D a = ctm.transform(p1, null);
			Point2D b = ctm.transform(p2, null);
			currentpoint_Renamed = ctm.transform(p3, null);
			currentpath_Renamed.curveTo((float) a.X, (float) a.Y, (float) b.X, (float) b.Y, (float) currentpoint_Renamed.X, (float) currentpoint_Renamed.Y);
		}

		public virtual void rcurveto(Point2D v1, Point2D v2, Point2D v3)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "rcurveto");
			}
			Point2D dv1 = ctm.deltaTransform(v1, null);
			Point2D dv2 = ctm.deltaTransform(v2, null);
			Point2D dv3 = ctm.deltaTransform(v3, null);
			double ax = currentpoint_Renamed.X + dv1.X;
			double ay = currentpoint_Renamed.Y + dv1.Y;
			double bx = currentpoint_Renamed.X + dv2.X;
			double by = currentpoint_Renamed.Y + dv2.Y;
			double cx = currentpoint_Renamed.X + dv3.X;
			double cy = currentpoint_Renamed.Y + dv3.Y;
			currentpoint_Renamed = new Point2D.Double(cx, cy);
			currentpath_Renamed.curveTo((float) ax, (float) ay, (float) bx, (float) by, (float) cx, (float) cy);
		}

		private static int arcToCurve(double ang1, double ang2, bool ccw, Point2D[] result)
		{
			double ang = ccw ? angDiff(ang1, ang2) : angDiff(ang2, ang1);
			double angle = ang360(ang);
			int n = (int)((2 * Math.Abs(angle) - SMALL_NUM) / Math.PI) + 1;
			if (Math.Abs(ang) < SMALL_NUM)
			{
				return 0;
			}
			double ainc = angle / n * (ccw ? 1 : -1);
			double w = btan(ainc / 2);
			double s = Math.Sin(ang1);
			double c = Math.Cos(ang1);
			result[0] = new Point2D.Double(c, s);
			int m = 3 * n;
			for (int i = 1; i < m; i += 3)
			{
				result[i] = new Point2D.Double(c - w * s, s + w * c);
				ang1 += ainc;
				s = Math.Sin(ang1);
				c = Math.Cos(ang1);
				result[i + 1] = new Point2D.Double(c + w * s, s - w * c);
				result[i + 2] = new Point2D.Double(c, s);
			}
			return n;
		}

		private static double ang360(double a)
		{
			if (a >= TWO_PI)
			{
				return TWO_PI;
			}
			else if (a <= -TWO_PI)
			{
				return -TWO_PI;
			}
			return a;
		}

		private static double angDiff(double ang1, double ang2)
		{
			return (ang2 >= ang1) ? ang2 - ang1 : ang2 + TWO_PI - ang1;
		}

		private static double btan(double alpha)
		{
			double a = 1 - Math.Cos(alpha);
			double b = Math.Tan(alpha);
			double c = Math.Sqrt(1 + b * b) - 1 + a;
			return 4.0 / 3.0 * a * b / c;
		}

		public virtual void arc(Point2D center, double r, double ang1, double ang2, bool ccw)
		{
			Point2D[] points = new Point2D[13];
			int n = arcToCurve(ang1, ang2, ccw, points);
			if (n <= 0)
			{
				return;
			}
			int m = 3 * n + 1;
			AffineTransform xform = (AffineTransform) ctm.clone();
			xform.translate(center.X, center.Y);
			xform.scale(r, r);
			xform.transform(points, 0, points, 0, m);
			float x0 = (float) points[0].X;
			float y0 = (float) points[0].Y;
			if (currentpoint_Renamed != null && (currentpoint_Renamed.X != x0 || currentpoint_Renamed.Y != y0))
			{
				currentpath_Renamed.lineTo(x0, y0);
				currentpoint_Renamed.setLocation(x0, y0);
			}
			else
			{
				currentpath_Renamed.moveTo(x0, y0);
				currentpoint_Renamed = new Point2D.Double(x0, y0);
			}
			int i, j, k;
			for (i = 1, j = 2, k = 3; i < m; i += 3, j += 3, k += 3)
			{
				currentpath_Renamed.curveTo((float) points[i].X, (float) points[i].Y, (float) points[j].X, (float) points[j].Y, (float) points[k].X, (float) points[k].Y);
			}
			currentpoint_Renamed.Location = points[n - 1];
		}

		public virtual void arct(Point2D pt1, Point2D pt2, double r)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "arct");
			}
			arcto(pt1, pt2, r);
		}

		public virtual double[] arcto(Point2D pt1, Point2D pt2, double r)
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "arcto");
			}
			Point2D pt0 = currentpoint();
			double p0x = pt0.X;
			double p0y = pt0.Y;
			double p1x = pt1.X;
			double p1y = pt1.Y;
			double p2x = pt2.X;
			double p2y = pt2.Y;
			double v0x = p0x - p1x;
			double v0y = p0y - p1y;
			double v1x = p2x - p1x;
			double v1y = p2y - p1y;
	/*
	System.err.println("arcto pt0: " + pt0);
	System.err.println("arcto pt1: " + pt1);
	System.err.println("arcto pt2: " + pt2);
	System.err.println("arcto v0: " + v0x + "," + v0y);
	System.err.println("arcto v1: " + v1x + "," + v1y);
	*/
			double alpha = Math.Atan2(v0y * v1x - v0x * v1y, v0x * v1x + v0y * v1y);
	//System.err.println("arcto dy: " + (v0y*v1x - v0x*v1y) + " dx: " + (v0x*v1x + v0y*v1y));
			double a = 1.0 / Math.Tan(alpha / 2) * r;
			double f = a / Math.Sqrt(v0x * v0x + v0y * v0y);
			double g = a / Math.Sqrt(v1x * v1x + v1y * v1y);
	//System.err.println("arcto a: " + a + " alpha: " + alpha);
			double v0px = v0x * f;
			double v0py = v0y * f;
			double v1px = v1x * g;
			double v1py = v1y * g;
			double p0tx = p1x + v0px;
			double p0ty = p1y + v0py;
			double p1tx = p1x + v1px;
			double p1ty = p1y + v1py;
			double b = Math.Sqrt(a * a + r * r);
			double wx = (p1tx - p0tx) / 2;
			double wy = (p1ty - p0ty) / 2;
			double ux = v0px + wx;
			double uy = v0py + wy;
			double h = b / Math.Sqrt(ux * ux + uy * uy);
	//System.err.println("arcto b: " + b + " h: " + h);
			double mx = p1x + ux * h;
			double my = p1y + uy * h;
			double v0ppx = p0tx - mx;
			double v0ppy = p0ty - my;
			double v1ppx = p1tx - mx;
			double v1ppy = p1ty - my;
	//System.err.println("arcto atan2: " + v0ppy + " " + v0ppx);
	//System.err.println("arcto atan2: " + v1ppy + " " + v1ppx);
			double ang0 = Math.Atan2(v0ppy, v0ppx);
			double ang1 = Math.Atan2(v1ppy, v1ppx);
	//System.err.println("arcto angle: " + ang0 + " " + ang1 + " r: " + r);
			arc(new Point2D.Double(mx, my), r, ang0, ang1, true);
			if ((v1px * v1px + v1py * v1py) > 0)
			{
				lineto(p2x, p2y);
			}
			double[] result = new double[4];
			result[0] = p0tx;
			result[1] = p0ty;
			result[2] = p1tx;
			result[3] = p1ty;
			return result;
		}

		public virtual void closepath()
		{
			if (currentpoint_Renamed == null)
			{
				return;
			}
			currentpath_Renamed.closePath();
		}

		public virtual FontDecoder FontDecoder
		{
			set
			{
				this.fontdecoder = value;
			}
			get
			{
				return fontdecoder;
			}
		}


		public virtual void setRequestBitmapWidths(Interpreter ip, Any @bool)
		{
			if (!(@bool is BoolType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "invalid bitmap font request");
			}
			bitmapwidths = ((BoolType) @bool).booleanValue();
		}

		public virtual bool RequestBitmapWidths
		{
			get
			{
				return bitmapwidths;
			}
		}

		public virtual void setSystemFonts(Interpreter ip, Any assoc)
		{
			if (!(assoc is DictType))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "invalid bitmap font association");
			}
			systemfontdict = (DictType) assoc;
		}

		public virtual DictType SystemFonts
		{
			get
			{
				return systemfontdict;
			}
		}

		public virtual void show(Interpreter ip, StringType @string)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, c);
				rmoveto(cw.DeltaX, cw.DeltaY);
			}
		}

		public virtual void ashow(Interpreter ip, double ax, double ay, StringType @string)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, c);
				rmoveto(ax + cw.DeltaX, ay + cw.DeltaY);
			}
		}

		public virtual void widthshow(Interpreter ip, double cx, double cy, int c, StringType @string)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int cc = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, cc);
				if (c == cc)
				{
					rmoveto(cx + cw.DeltaX, cy + cw.DeltaY);
				}
				else
				{
					rmoveto(cw.DeltaX, cw.DeltaY);
				}
			}
		}

		public virtual void awidthshow(Interpreter ip, double cx, double cy, int c, double ax, double ay, StringType @string)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int cc = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, cc);
				if (c == cc)
				{
					rmoveto(ax + cx + cw.DeltaX, ay + cy + cw.DeltaY);
				}
				else
				{
					rmoveto(ax + cw.DeltaX, ay + cw.DeltaY);
				}
			}
		}

		public virtual void kshow(Interpreter ip, ArrayType proc, StringType @string)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			int i = 0, c0 = 0, c1;
			while (e.MoveNext())
			{
				c1 = ((IntegerType) e.Current).intValue();
				if (i++ >= 1)
				{
					ip.ostack.pushRef(new IntegerType(c0));
					ip.ostack.pushRef(new IntegerType(c1));
					ip.estack.run(ip, proc);
				}
				CharWidth cw = fontdecoder.show(ip, c1);
				rmoveto(cw.DeltaX, cw.DeltaY);
				c0 = c1;
			}
		}

		public virtual void cshow(Interpreter ip, ArrayType proc, StringType @string)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.charwidth(ip, c);
				ip.ostack.pushRef(new IntegerType(c));
				ip.ostack.pushRef(new RealType(cw.DeltaX));
				ip.ostack.pushRef(new RealType(cw.DeltaY));
				ip.estack.run(ip, proc);
				// TODO:
			}
		}

		public virtual void xshow(Interpreter ip, StringType @string, Any displacement)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, c);
				// TODO:
				rmoveto(cw.DeltaX, cw.DeltaY);
			}
		}

		public virtual void yshow(Interpreter ip, StringType @string, Any displacement)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, c);
				// TODO:
				rmoveto(cw.DeltaX, cw.DeltaY);
			}
		}

		public virtual void xyshow(Interpreter ip, StringType @string, Any displacement)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.show(ip, c);
				// TODO:
				rmoveto(cw.DeltaX, cw.DeltaY);
			}
		}

		public virtual float[] stringwidth(Interpreter ip, StringType @string)
		{
			if (fontdecoder == null)
			{
				throw new Stop(Stoppable_Fields.INVALIDFONT, "no current font");
			}
			float[] result = new float[2];
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.charwidth(ip, c);
				result[0] += cw.DeltaX;
				result[1] += cw.DeltaY;
			}
			return result;
		}

		public virtual void charpath(Interpreter ip, StringType @string, bool stroked)
		{
			checkShowState();
			System.Collections.IEnumerator e = @string.elements();
			while (e.MoveNext())
			{
				int c = ((IntegerType) e.Current).intValue();
				CharWidth cw = fontdecoder.charpath(ip, c);
				rmoveto(cw.DeltaX, cw.DeltaY);
			}
	//		if (stroked) {
	//			appendShape(currentpath, new GeneralPath(createStroke().createStrokedShape(currentpath)));
	//		}
		}

		public virtual void checkShowState()
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT, "checkShowState");
			}
			if (fontdecoder == null)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "no current font");
			}
		}

		public virtual void image(Interpreter ip, int width, int height, int bits, AffineTransform ximg, ArrayType proc)
		{
			Bitmap bitmap;
			switch (ColorSpace)
			{
			case ColorSpace.TYPE_GRAY:
				GrayPixelSource gps = PixelSourceFactory.createGrayPixelSource(ip, proc, bits);
				bitmap = createBitmap(ip, width, height, gps);
				break;
			case ColorSpace.TYPE_RGB:
				RGBPixelSource cps = PixelSourceFactory.createRGBPixelSource(ip, proc, bits);
				bitmap = createColorBitmap(ip, width, height, cps);
				break;
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR, "unsupported colorspace");
			}
			AffineTransform xform = createImageXform(ximg);
			currentdevice_Renamed.image(bitmap, xform);
		}

		public virtual void imagemask(Interpreter ip, int width, int height, int bits, bool polarity, AffineTransform ximg, ArrayType proc)
		{
			GrayPixelSource ps = PixelSourceFactory.createGrayPixelSource(ip, proc, bits);
			Bitmap bitmap = createBitmapMask(ip, width, height, polarity, ps);
			AffineTransform xform = createImageXform(ximg);
			currentdevice_Renamed.image(bitmap, xform);
		}

		public virtual void image(Interpreter ip, int width, int height, int bits, AffineTransform ximg, CharStream src)
		{
			Bitmap bitmap;
			switch (ColorSpace)
			{
			case ColorSpace.TYPE_GRAY:
				GrayPixelSource gps = PixelSourceFactory.createGrayPixelSource(src, bits);
				bitmap = createBitmap(ip, width, height, gps);
				break;
			case ColorSpace.TYPE_RGB:
				RGBPixelSource cps = PixelSourceFactory.createRGBPixelSource(src, bits);
				bitmap = createColorBitmap(ip, width, height, cps);
				break;
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR, "unsupported colorspace");
			}
			AffineTransform xform = createImageXform(ximg);
			currentdevice_Renamed.image(bitmap, xform);
		}

		public virtual void imagemask(Interpreter ip, int width, int height, int bits, bool polarity, AffineTransform ximg, CharStream src)
		{
			Bitmap bitmap;
			switch (ColorSpace)
			{
			case ColorSpace.TYPE_GRAY:
				GrayPixelSource gps = PixelSourceFactory.createGrayPixelSource(src, bits);
				bitmap = createBitmapMask(ip, width, height, polarity, gps);
				break;
			case ColorSpace.TYPE_RGB:
				RGBPixelSource cps = PixelSourceFactory.createRGBPixelSource(src, bits);
				bitmap = createColorBitmapMask(ip, width, height, polarity, cps);
				break;
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR, "unsupported colorspace");
			}
			AffineTransform xform = createImageXform(ximg);
			currentdevice_Renamed.image(bitmap, xform);
		}

		public virtual void colorimage(Interpreter ip, int w, int h, int bits, int ncomp, bool multi, AffineTransform ximg, Any[] src)
		{
			RGBPixelSource ps;
			Bitmap bitmap;
			switch (ncomp)
			{
			case 1:
				if (src[0] is ArrayType)
				{
					if (multi)
					{
						ps = PixelSourceFactory.createRGBPixelSource(ip, src, bits);
					}
					else
					{
						ps = PixelSourceFactory.createRGBPixelSource(ip, src[0], bits);
					}
				}
				else if (src[0] is CharStream)
				{
					if (multi)
					{
						ps = PixelSourceFactory.createRGBPixelSource((CharStream[]) src, bits);
					}
					else
					{
						ps = PixelSourceFactory.createRGBPixelSource((CharStream) src[0], bits);
					}
				}
				else
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "bad source");
				}
				bitmap = createColorBitmap(ip, w, h, ps);
				break;
			case 3:
				if (arrayTypeCheck(src, typeof(ArrayType)))
				{
					if (multi)
					{
						ps = PixelSourceFactory.createRGBPixelSource(ip, src, bits);
					}
					else
					{
						ps = PixelSourceFactory.createRGBPixelSource(ip, src[0], bits);
					}
				}
				else if (arrayTypeCheck(src, typeof(CharStream)))
				{
					if (multi)
					{
						ps = PixelSourceFactory.createRGBPixelSource((CharStream[]) src, bits);
					}
					else
					{
						ps = PixelSourceFactory.createRGBPixelSource((CharStream) src[0], bits);
					}
				}
				else
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "bad RGB source");
				}
				bitmap = createColorBitmap(ip, w, h, ps);
				break;
			case 4:
				CMYKPixelSource cps;
				if (arrayTypeCheck(src, typeof(ArrayType)))
				{
					if (multi)
					{
						cps = PixelSourceFactory.createCMYKPixelSource(ip, src, bits);
					}
					else
					{
						cps = PixelSourceFactory.createCMYKPixelSource(ip, src[0], bits);
					}
				}
				else if (arrayTypeCheck(src, typeof(CharStream)))
				{
					if (multi)
					{
						cps = PixelSourceFactory.createCMYKPixelSource((CharStream[]) src, bits);
					}
					else
					{
						cps = PixelSourceFactory.createCMYKPixelSource((CharStream) src[0], bits);
					}
				}
				else
				{
					throw new Stop(Stoppable_Fields.TYPECHECK, "bad CMYK source");
				}
				bitmap = createColorBitmap(ip, w, h, cps);
				break;
			default:
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT);
			}
			AffineTransform xform = createImageXform(ximg);
			currentdevice_Renamed.image(bitmap, xform);
		}

		private Bitmap createBitmap(Interpreter ip, int w, int h, GrayPixelSource ps)
		{
			Bitmap bitmap = currentdevice_Renamed.createBitmap(w, h);
			int x = 0;
			int y = 0, i = 0, count = w * h;
			try
			{
				while (i < count)
				{
					int pixel = ps.nextPixel();
					if (pixel < 0)
					{
						break;
					}
					if (currentgraytransfer != null)
					{
						ip.ostack.push(new RealType((float) pixel / 255f));
						ip.estack.run(ip, currentgraytransfer);
						pixel = (int)Math.Round(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue() * 255f, MidpointRounding.AwayFromZero);
					}
					bitmap.draw(x, y, 0xff000000 | (pixel << 16) | (pixel << 8) | pixel);
					if (++x >= w)
					{
						x = 0;
						y++;
					}
					i++;
				}
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR, "createBitmap");
			}
			return bitmap;
		}

		private Bitmap createBitmapMask(Interpreter ip, int w, int h, bool polarity, GrayPixelSource ps)
		{
			w = (w + 7) / 8 * 8; // some type-3 fonts rely on that
			Bitmap bitmap = currentdevice_Renamed.createBitmap(w, h);
			int x = 0;
			int y = 0, i = 0, count = w * h;
			int curcolor = RGB;
			try
			{
				while (i < count)
				{
					int pixel = ps.nextPixel();
					if (pixel < 0)
					{
						break;
					}
					if ((pixel != 0) == polarity)
					{
						bitmap.draw(x, y, curcolor);
					}
					if (++x >= w)
					{
						x = 0;
						y++;
					}
					i++;
				}
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR, "createBitmapMask");
			}
			return bitmap;
		}

		private Bitmap createColorBitmap(Interpreter ip, int w, int h, RGBPixelSource ps)
		{
			Bitmap bitmap = currentdevice_Renamed.createBitmap(w, h);
			int x = 0;
			int y = 0, i = 0, count = w * h;
			int curcolor = RGB;
			try
			{
				while (i < count)
				{
					int r = ps.nextRedComponent();
					if (r < 0)
					{
						break;
					}
					int g = ps.nextGreenComponent();
					if (g < 0)
					{
						break;
					}
					int b = ps.nextBlueComponent();
					if (b < 0)
					{
						break;
					}
					if (currentredtransfer_Renamed != null)
					{
						ip.ostack.push(new RealType((float) r / 255f));
						ip.estack.run(ip, currentredtransfer_Renamed);
						float val = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
						r = (int)Math.Round(val * 255f, MidpointRounding.AwayFromZero) & 0xff;
					}
					if (currentgreentransfer_Renamed != null)
					{
						ip.ostack.push(new RealType((float) g / 255f));
						ip.estack.run(ip, currentgreentransfer_Renamed);
						float val = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
						g = (int)Math.Round(val * 255f, MidpointRounding.AwayFromZero) & 0xff;
					}
					if (currentbluetransfer_Renamed != null)
					{
						ip.ostack.push(new RealType((float) b / 255f));
						ip.estack.run(ip, currentbluetransfer_Renamed);
						float val = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
						b = (int)Math.Round(val * 255f, MidpointRounding.AwayFromZero) & 0xff;
					}
					bitmap.draw(x, y, 0xff000000 | (r << 16) | (g << 8) | b);
					if (++x >= w)
					{
						x = 0;
						y++;
					}
					i++;
				}
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, ex.Message);
			}
			return bitmap;
		}

		private Bitmap createColorBitmapMask(Interpreter ip, int w, int h, bool polarity, RGBPixelSource ps)
		{
			Bitmap bitmap = currentdevice_Renamed.createBitmap(w, h);
			int x = 0;
			int y = 0, i = 0, count = w * h;
			int curcolor = RGB;
			try
			{
				while (i < count)
				{
					int r = ps.nextRedComponent();
					if (r < 0)
					{
						break;
					}
					int g = ps.nextGreenComponent();
					if (g < 0)
					{
						break;
					}
					int b = ps.nextBlueComponent();
					if (b < 0)
					{
						break;
					}
					bitmap.draw(x, y, 0xff000000 | r | g | b);
					if (++x >= w)
					{
						x = 0;
						y++;
					}
					i++;
				}
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR, "createColorBitmapMask");
			}
			return bitmap;
		}

		private Bitmap createColorBitmap(Interpreter ip, int w, int h, CMYKPixelSource ps)
		{
			Bitmap bitmap = currentdevice_Renamed.createBitmap(w, h);
			int x = 0;
			int y = 0, i = 0, count = w * h;
			try
			{
				while (i < count)
				{
					int cyan = ps.nextCyanComponent();
					if (cyan < 0)
					{
						break;
					}
					int magenta = ps.nextMagentaComponent();
					if (magenta < 0)
					{
						break;
					}
					int yellow = ps.nextYellowComponent();
					if (yellow < 0)
					{
						break;
					}
					int black = ps.nextBlackComponent();
					if (black < 0)
					{
						break;
					}
					Color color = convertCMYK2RGB(cyan, magenta, yellow, black);
					bitmap.draw(x, y, color);
					if (++x >= w)
					{
						x = 0;
						y++;
					}
					i++;
				}
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR, "createColorBitmap");
			}
			return bitmap;
		}

		private static Color convertCMYK2RGB(int c, int m, int y, int k)
		{
			float c0 = c / 255f;
			float m0 = m / 255f;
			float y0 = y / 255f;
			float k0 = k / 255f;
			float r = 1f - (c0 * (1 - k0) + k0);
			float g = 1f - (m0 * (1 - k0) + k0);
			float b = 1f - (y0 * (1 - k0) + k0);
			return new Color(r, g, b);
		}

		private bool arrayTypeCheck(object[] array, Type type)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (!type.IsInstanceOfType(array[i]))
				{
					return false;
				}
			}
			return true;
		}

		private AffineTransform createImageXform(AffineTransform ximg)
		{
			AffineTransform xinv = createInverse(ximg);
			AffineTransform xform = (AffineTransform) ctm.clone();
			xform.concatenate(xinv);
			return xform;
		}

		public virtual void fill(int rule)
		{
			currentpath_Renamed.WindingRule = rule;
			if (currentflat_Renamed != DEFAULT_FLATNESS)
			{
				currentdevice_Renamed.fill(createFlatShape(currentpath_Renamed, currentflat_Renamed));
			}
			else
			{
				currentdevice_Renamed.fill(currentpath_Renamed);
			}
			newpath();
		}

		public virtual void rectfill(float llx, float lly, float width, float height)
		{
			currentdevice_Renamed.fill(createTransformedRect(llx, lly, width, height));
		}

		public virtual void stroke()
		{
			if (strokeChanged)
			{
				Stroke stroke = createStroke();
				if (stroke != null)
				{
					currentdevice_Renamed.Stroke = stroke;
				}
				strokeChanged = false;
			}
			if (currentflat_Renamed != DEFAULT_FLATNESS)
			{
				currentdevice_Renamed.stroke(createFlatShape(currentpath_Renamed, currentflat_Renamed));
			}
			else
			{
				currentdevice_Renamed.stroke(currentpath_Renamed);
			}
			newpath();
		}

		public virtual void rectstroke(float llx, float lly, float width, float height)
		{
			currentdevice_Renamed.stroke(createTransformedRect(llx, lly, width, height));
		}

		private static Shape createFlatShape(Shape shape, float flatness)
		{
			GeneralPath path = new GeneralPath();
			float flatfix = flatness / 20; // TODO: this fixes "poster.ps"
			path.append(shape.getPathIterator(IDENT_MATRIX, flatfix), false);
			return path;
		}

		public virtual Stroke createStroke()
		{
			float width = transformLength(currentwidth);
			float[] array = transformLength(currentdasharray_Renamed);
			float phase = transformLength(currentdashphase_Renamed);
			return currentdevice_Renamed.createStroke(width, currentcap, currentjoin, currentmiterlimit_Renamed, array, phase);
		}

		public virtual void strokepath()
		{
			Stroke stroke = createStroke();
			currentpath_Renamed = new GeneralPath(stroke.createStrokedShape(currentpath_Renamed));
		}

		public virtual double[] pathbbox()
		{
			if (currentpoint_Renamed == null)
			{
				throw new Stop(Stoppable_Fields.NOCURRENTPOINT);
			}
			AffineTransform ixform;
			ixform = createInverse(ctm);
			Rectangle2D box = currentpath_Renamed.Bounds2D;
			double x = box.X;
			double y = box.Y;
			double w = box.Width;
			double h = box.Height;
			Point2D lb = new Point2D.Double(x, y + h);
			Point2D rt = new Point2D.Double(x + w, y);
			ixform.transform(lb, lb);
			ixform.transform(rt, rt);
			double[] result = new double[4];
			result[0] = lb.X;
			result[1] = lb.Y;
			result[2] = rt.X;
			result[3] = rt.Y;
			return result;
		}

		private float transformLength(double length)
		{
			Point2D v = ctm.deltaTransform(new Point2D.Double(length, 0), null);
			double x = v.X;
			double y = v.Y;
			return (float) Math.Sqrt(x * x + y * y);
		}

		public virtual float[] transformLength(ArrayType array)
		{
			if (array == null || array.length() == 0)
			{
				return null;
			}
			int i, n = array.length();
			float[] result = new float[n];
			for (i = 0; i < n; i++)
			{
				result[i] = transformLength(((NumberType) array.get(i)).realValue());
			}
			return result;
		}

		public virtual void setflat(float tol)
		{
			currentflat_Renamed = tol;
		}

		public virtual float currentflat()
		{
			return currentflat_Renamed;
		}

		public virtual void setlinewidth(float width)
		{
			strokeChanged |= width != currentwidth;
			currentwidth = width;
		}

		public virtual double currentlinewidth()
		{
			return currentwidth;
		}

		public virtual void setlinecap(int cap)
		{
			strokeChanged |= cap != currentcap;
			currentcap = cap;
		}

		public virtual int currentlinecap()
		{
			return currentcap;
		}

		public virtual void setlinejoin(int join)
		{
			strokeChanged |= join != currentjoin;
			currentjoin = join;
		}

		public virtual int currentlinejoin()
		{
			return currentjoin;
		}

		public virtual void setmiterlimit(float limit)
		{
			strokeChanged |= limit != currentmiterlimit_Renamed;
			currentmiterlimit_Renamed = limit;
		}

		public virtual float currentmiterlimit()
		{
			return currentmiterlimit_Renamed;
		}

		public virtual bool StrokeAdjustment
		{
			set
			{
				this.strokeAdjustment = value;
			}
			get
			{
				return strokeAdjustment;
			}
		}


		public virtual bool Overprint
		{
			set
			{
				this.overprint = value;
			}
			get
			{
				return overprint;
			}
		}


		public virtual void setdash(ArrayType array, float phase)
		{
			if (array == null)
			{
				strokeChanged |= currentdasharray_Renamed != null && currentdasharray_Renamed.length() > 0;
				currentdasharray_Renamed = null;
				currentdashphase_Renamed = 0;
			}
			else if (currentdasharray_Renamed == null)
			{
				strokeChanged = true;
				currentdasharray_Renamed = array;
				currentdashphase_Renamed = phase;
			}
			else
			{
				int arraylen = array.length();
				if (arraylen > 0)
				{
					int currentlen = currentdasharray_Renamed.length();
					strokeChanged |= arraylen != currentlen;
					for (int i = 0; !strokeChanged && i < arraylen; i++)
					{
						strokeChanged = array.get(i) != currentdasharray_Renamed.get(i);
					}
					strokeChanged |= phase != currentdashphase_Renamed;
					if (!strokeChanged)
					{
						currentdasharray_Renamed = array;
						currentdashphase_Renamed = phase;
					}
				}
				else
				{
					currentdasharray_Renamed = null;
					currentdashphase_Renamed = phase;
					strokeChanged = true;
				}
			}
		}

		public virtual double currentdashphase()
		{
			return currentdashphase_Renamed;
		}

		public virtual ArrayType currentdasharray()
		{
			return currentdasharray_Renamed;
		}

		public virtual void setColorScreen(Screen red, Screen green, Screen blue)
		{
			currentredscreen_Renamed = red;
			currentgreenscreen_Renamed = green;
			currentbluescreen_Renamed = blue;
		}

		public virtual Screen currentredscreen()
		{
			return currentredscreen_Renamed != null ? currentredscreen_Renamed : NULL_SCREEN;
		}

		public virtual Screen currentgreenscreen()
		{
			return currentgreenscreen_Renamed != null ? currentgreenscreen_Renamed : NULL_SCREEN;
		}

		public virtual Screen currentbluescreen()
		{
			return currentbluescreen_Renamed != null ? currentbluescreen_Renamed : NULL_SCREEN;
		}

		public virtual void setHalftonePhase(int x, int y)
		{
			halftonePhaseX = x;
			halftonePhaseY = y;
		}

		public virtual int[] HalftonePhase
		{
			get
			{
				int[] phase = new int[2];
				phase[0] = halftonePhaseX;
				phase[1] = halftonePhaseY;
				return phase;
			}
		}

		public virtual DictType Halftone
		{
			set
			{
				this.halftone = value;
		/*
				int type = ((IntegerType) value.get("HalftoneType", INTEGER)).intValue();
				if (type != 1) // TODO: value type 3
					return;
				double freq = ((NumberType) value.get("Frequency", NUMBER)).realValue();
				if (freq <= 1e-4)
					throw new Stop(TYPECHECK, "Frequency: " + freq);
				double angle = ((NumberType) value.get("Angle", NUMBER)).realValue();
				ArrayType proc = (ArrayType) value.get("SpotFunction", ARRAY);
				double res = currentdevice.getResolution();
				double scale = currentdevice.getScale();
				double dpi = res * scale;
				if (dpi <= 1)
					throw new Stop(INTERNALERROR, "dpi: " + dpi);
				double phi = Math.IEEEremainder(angle + 45, 90);
				double rad = phi * Math.PI / 180;
				double dots = dpi * Math.sqrt(2) * Math.cos(rad) / freq;
				double width = dots - 1;
				int ndots = ((int) Math.abs(width) - 1) | 1; // make sure ndots is odd
				if (ndots <= 1)
					throw new Stop(TYPECHECK, "ndots: " + ndots);
				double step = 2.0 / width;
				double norm = width / dots;
				double dy = -norm;
		System.out.println("ndots: " + ndots + " dpi: " + dpi + " step: " + step + " norm: " + norm);
				for (int j = 0; j < ndots; j++) {
					for (int i = 0; i < ndots; i++) {
					}
				}
		*/
			}
			get
			{
				return halftone;
			}
		}


		public virtual DictType Pattern
		{
			set
			{
				this.pattern = value;
			}
			get
			{
				return pattern;
			}
		}


		public virtual int RGB
		{
			get
			{
				Color color = Color;
				return color != null ? color.RGB : 0;
			}
		}

		public virtual Color Color
		{
			get
			{
				Color color;
				switch (ColorSpace)
				{
				case ColorSpace.TYPE_GRAY:
					color = currentdevice_Renamed.createColor(currentgray_Renamed, currentgray_Renamed, currentgray_Renamed);
					break;
				default:
					color = currentdevice_Renamed.createColor(currentred, currentgreen, currentblue);
					break;
				}
				return color;
			}
		}

		public virtual void setgray(float gray)
		{
			currentgray_Renamed = gray > 1 ? 1 : (gray < 0 ? 0 : gray);
			Color color = currentdevice_Renamed.createColor(currentgray_Renamed, currentgray_Renamed, currentgray_Renamed);
			if (color != null)
			{
				currentdevice_Renamed.Color = color;
			}
			ColorSpace = ColorSpace.CS_GRAY;
		}

		public virtual float currentgray()
		{
			return currentgray_Renamed;
		}

		public virtual void setrgbcolor(float red, float green, float blue)
		{
			currentred = red > 1 ? 1 : (red < 0 ? 0 : red);
			currentgreen = green > 1 ? 1 : (green < 0 ? 0 : green);
			currentblue = blue > 1 ? 1 : (blue < 0 ? 0 : blue);
			Color color = currentdevice_Renamed.createColor(currentred, currentgreen, currentblue);
			if (color != null)
			{
				currentdevice_Renamed.Color = color;
			}
			ColorSpace = ColorSpace.CS_sRGB;
		}

		public virtual float[] currentrgbcolor()
		{
			float[] color = new float[3];
			color[0] = currentred;
			color[1] = currentgreen;
			color[2] = currentblue;
			return color;
		}

		public virtual void sethsbcolor(float h, float s, float b)
		{
			Color color = Color.getHSBColor(h, s, b);
			currentred = color.Red / 255f;
			currentgreen = color.Green / 255f;
			currentblue = color.Blue / 255f;
			currentdevice_Renamed.Color = color;
			ColorSpace = ColorSpace.CS_sRGB;
		}

		public virtual float[] currenthsbcolor()
		{
			int red = (int)(currentred * 255);
			int green = (int)(currentgreen * 255);
			int blue = (int)(currentblue * 255);
			return Color.RGBtoHSB(red, green, blue, null);
		}

		public virtual void setcmykcolor(float cyan, float magenta, float yellow, float black)
		{
			currentred = 1f - Math.Min(1f, cyan + black);
			currentgreen = 1f - Math.Min(1f, magenta + black);
			currentblue = 1f - Math.Min(1f, yellow + black);
			Color color = currentdevice_Renamed.createColor(currentred, currentgreen, currentblue);
			if (color != null)
			{
				currentdevice_Renamed.Color = color;
			}
			ColorSpace = ColorSpace.CS_sRGB;
		}

		public virtual float[] currentcmykcolor()
		{
			float[] color = new float[4];
			color[0] = 1f - currentred;
			color[1] = 1f - currentgreen;
			color[2] = 1f - currentblue;
			color[3] = 1f - Math.Min(Math.Min(color[0], color[1]), color[2]);
			return color;
		}

		public virtual DictType ColorRendering
		{
			set
			{
				this.colorRendering = value;
			}
			get
			{
				return colorRendering;
			}
		}


		public virtual void settransfer(ArrayType proc)
		{
			currentgraytransfer = proc.length() > 0 ? proc : null;
		}

		public virtual ArrayType currenttransfer()
		{
			return currentgraytransfer;
		}

		public virtual void setcolortransfer(ArrayType red, ArrayType green, ArrayType blue, ArrayType gray)
		{
			currentredtransfer_Renamed = red.length() > 0 ? red : null;
			currentgreentransfer_Renamed = green.length() > 0 ? green : null;
			currentbluetransfer_Renamed = blue.length() > 0 ? blue : null;
			currentgraytransfer = gray.length() > 0 ? gray : null;
		}

		public virtual ArrayType currentredtransfer()
		{
			return currentredtransfer_Renamed;
		}

		public virtual ArrayType currentgreentransfer()
		{
			return currentgreentransfer_Renamed;
		}

		public virtual ArrayType currentbluetransfer()
		{
			return currentbluetransfer_Renamed;
		}

		public virtual void setblackgeneration(ArrayType proc)
		{
			currentblackgeneration_Renamed = proc;
		}

		public virtual ArrayType currentblackgeneration()
		{
			return currentblackgeneration_Renamed;
		}

		public virtual void setundercolorremoval(ArrayType proc)
		{
			currentundercolorremoval_Renamed = proc;
		}

		public virtual ArrayType currentundercolorremoval()
		{
			return currentundercolorremoval_Renamed;
		}

		public virtual int ColorSpace
		{
			set
			{
				this.colorspace = ColorSpace.getInstance(value);
			}
			get
			{
				return colorspace.Type;
			}
		}


		public virtual void initclip(Device device)
		{
			device.initclip();
		}

		public virtual void clippath()
		{
			currentpath_Renamed.reset();
			appendShape(currentpath_Renamed, currentdevice_Renamed.clippath());
			currentpoint_Renamed = currentpath_Renamed.CurrentPoint;
		}

		private static void appendShape(GeneralPath path, Shape shape)
		{
			float[] coords = new float[6];
			if (shape != null)
			{
				PathIterator e = shape.getPathIterator(IDENT_MATRIX);
				while (!e.Done)
				{
					switch (e.currentSegment(coords))
					{
					case PathIterator.SEG_MOVETO:
						path.moveTo(coords[0], coords[1]);
						break;
					case PathIterator.SEG_LINETO:
						path.lineTo(coords[0], coords[1]);
						break;
					case PathIterator.SEG_CUBICTO:
						path.curveTo(coords[0], coords[1], coords[2], coords[3], coords[4], coords[5]);
						break;
					case PathIterator.SEG_CLOSE:
						path.closePath();
						break;
					}
					e.next();
				}
			}
		}

		private static Shape reduceShape(Shape shape)
		{
			if (shape is Rectangle2D)
			{
				return shape;
			}
			bool simple = true;
			int i = 0;
			float[] rect = new float[8];
			float[] coords = new float[6];
			PathIterator e = shape.getPathIterator(IDENT_MATRIX);
			while (!e.Done)
			{
				switch (e.currentSegment(coords))
				{
				case PathIterator.SEG_MOVETO:
					if (i > 0)
					{
						return shape;
					}
					rect[i++] = coords[0];
					rect[i++] = coords[1];
					break;
				case PathIterator.SEG_LINETO:
					if (i >= 8)
					{
						return shape;
					}
					rect[i++] = coords[0];
					rect[i++] = coords[1];
					break;
				case PathIterator.SEG_CUBICTO:
					return shape;
				case PathIterator.SEG_CLOSE:
					break;
				}
				e.next();
			}
			if (i != 8)
			{
				return shape;
			}
			for (i = 0; i < 4; i++)
			{
				int j = i + 4;
				if (rect[i] > rect[j])
				{
					float tmp = rect[i];
					rect[i] = rect[j];
					rect[j] = tmp;
				}
			}
			if (Math.Abs(rect[0] - rect[2]) < SMALL_NUM && Math.Abs(rect[1] - rect[3]) < SMALL_NUM && Math.Abs(rect[4] - rect[6]) < SMALL_NUM && Math.Abs(rect[5] - rect[7]) < SMALL_NUM)
			{
				return new Rectangle2D.Float(rect[0], rect[1], rect[4] - rect[0], rect[5] - rect[1]);
			}
			return shape;
		}

		public virtual void clip(int rule)
		{
			currentpath_Renamed.WindingRule = rule;
			currentdevice_Renamed.clip(reduceShape(currentpath_Renamed));
		}

		public virtual void rectclip(float llx, float lly, float width, float height)
		{
			currentdevice_Renamed.clip(createTransformedRect(llx, lly, width, height));
			newpath();
		}

		public virtual void rectclip(float[] array)
		{
			newpath();
			currentpath_Renamed.WindingRule = GeneralPath.WIND_NON_ZERO;
			int i, n = array.Length - 3;
			for (i = 0; i < n; i += 4)
			{
				moveto(array[0], array[1]);
				lineto(array[2], array[1]);
				lineto(array[2], array[3]);
				lineto(array[0], array[3]);
				closepath();
			}
			currentdevice_Renamed.clip(currentpath_Renamed);
			newpath();
		}

		public virtual void nulldevice()
		{
			setdevice(nulldevice_Renamed);
		}

		public virtual void setcharwidth(float wx, float wy)
		{
			fontdecoder.CharWidth = new CharWidth(0, 0, wx, wy);
		}

		public virtual void setcachedevice(CharWidth cw, Rectangle2D box)
		{
			setcachedevice(cw, box.X, box.Y, box.Width, box.Height);
		}

		private void setcachedevice(CharWidth cw, double llx, double lly, double urx, double ury)
		{
			Point2D ll = new Point2D.Double(llx, ury);
			Point2D ur = new Point2D.Double(urx, lly);
			ctm.transform(ll, ll);
			ctm.transform(ur, ur);
			double x = ll.X;
			double y = ll.Y;
			double w = ur.X - x;
			double h = ur.Y - y;
			fontdecoder.CharWidth = cw;
			cachedevice_Renamed.CharWidth = cw;
			cachedevice_Renamed.CharBounds = new Rectangle2D.Double(x, y, w, h);
			cachedevice_Renamed.Target = currentdevice_Renamed;
			setdevice(cachedevice_Renamed);
		}

		public virtual void setcachedevice2(double w0x, double w0y, double llx, double lly, double urx, double ury, double w1x, double w1y, double vx, double vy)
		{
		}

		private static AffineTransform createInverse(AffineTransform xform)
		{
			try
			{
				return xform.createInverse();
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT, "non invertible matrix");
			}
		}

		private Rectangle2D createTransformedRect(float llx, float lly, float width, float height)
		{
			Point2D p = transform(llx, lly);
			Point2D v = deltaTransform(width, height);
			double x = p.X;
			double y = p.Y;
			double w = v.X;
			double h = v.Y;
			if (w < 0)
			{
				x += w;
				w = -w;
			}
			if (h < 0)
			{
				y += h;
				h = -h;
			}
			return new Rectangle2D.Double(x, y, w, h);
		}

		private Point2D transform(float x, float y)
		{
			Point2D pt = new Point2D.Float(x, y);
			return ctm.transform(pt, pt);
		}

		private Point2D deltaTransform(float dx, float dy)
		{
			Point2D v = new Point2D.Float(dx, dy);
			return ctm.deltaTransform(v, v);
		}

	}

}