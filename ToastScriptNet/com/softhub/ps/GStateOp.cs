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
	using Device = com.softhub.ps.device.Device;
	using CharWidth = com.softhub.ps.util.CharWidth;

	internal sealed class GStateOp : Stoppable, Types
	{

		private const string DEVICE_GRAY = "DeviceGray";
		private const string DEVICE_RGB = "DeviceRGB";
		private const string DEVICE_CMYK = "DeviceCMYK";

		private static readonly string[] OPNAMES = new string[] {"initgraphics", "gsave", "grestore", "grestoreall", "currentpoint", "setlinewidth", "currentlinewidth", "setflat", "currentflat", "setlinejoin", "currentlinejoin", "setlinecap", "currentlinecap", "setmiterlimit", "currentmiterlimit", "setgray", "setcolor", "currentcolor", "currentgray", "setrgbcolor", "currentrgbcolor", "sethsbcolor", "setcolorspace", "currentcolorspace", "currenthsbcolor", "setcmykcolor", "currentcmykcolor", "setdash", "currentdash", "setscreen", "currentscreen", "settransfer", "currenttransfer", "setcolortransfer", "currentcolortransfer", "setcolorscreen", "currentcolorscreen", "setblackgeneration", "setundercolorremoval", "currentblackgeneration", "currentundercolorremoval", "setpagedevice", "currentpagedevice", "setcharwidth", "setcachedevice", "setcachedevice2", "nulldevice", "cachestatus", "setcachelimit", "setcacheparams", "currentcacheparams", "setstrokeadjust", "currentstrokeadjustment", "setoverprint", "currentoverprint", "sethalftone", "currenthalftone", "setcolorrendering", "currentcolorrendering", "sethalftonephase", "currenthalftonephase"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(GStateOp));
		}

		/// <summary>
		/// Init graphics state.
		/// </summary>
		internal static void initgraphics(Interpreter ip)
		{
			ip.GraphicsState.initgraphics();
		}

		/// <summary>
		/// Save the current graphics context.
		/// </summary>
		internal static void gsave(Interpreter ip)
		{
			ip.gsave();
		}

		/// <summary>
		/// Restore the graphics context.
		/// </summary>
		internal static void grestore(Interpreter ip)
		{
			ip.grestore();
		}

		/// <summary>
		/// Restore the graphics context down to the initial state.
		/// </summary>
		internal static void grestoreall(Interpreter ip)
		{
			ip.grestoreAll();
		}

		/// <summary>
		/// Get the currentpoint.
		/// </summary>
		internal static void currentpoint(Interpreter ip)
		{
			Point2D pt = ip.GraphicsState.currentpoint();
			ip.ostack.pushRef(new RealType(pt.X));
			ip.ostack.pushRef(new RealType(pt.Y));
		}

		/// <summary>
		/// Set the current line width.
		/// </summary>
		internal static void setlinewidth(Interpreter ip)
		{
			ip.GraphicsState.setlinewidth(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue());
		}

		/// <summary>
		/// Get the current line width.
		/// </summary>
		internal static void currentlinewidth(Interpreter ip)
		{
			ip.ostack.pushRef(new RealType(ip.GraphicsState.currentlinewidth()));
		}

		/// <summary>
		/// Set the current flatness.
		/// </summary>
		internal static void setflat(Interpreter ip)
		{
			ip.GraphicsState.setflat(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue());
		}

		/// <summary>
		/// Get the current flatness.
		/// </summary>
		internal static void currentflat(Interpreter ip)
		{
			ip.ostack.pushRef(new RealType(ip.GraphicsState.currentflat()));
		}

		/// <summary>
		/// Set the current line join.
		/// </summary>
		internal static void setlinejoin(Interpreter ip)
		{
			ip.GraphicsState.setlinejoin(ip.ostack.popInteger());
		}

		/// <summary>
		/// Get the current line join.
		/// </summary>
		internal static void currentlinejoin(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.GraphicsState.currentlinejoin()));
		}

		/// <summary>
		/// Set the current line cap.
		/// </summary>
		internal static void setlinecap(Interpreter ip)
		{
			ip.GraphicsState.setlinecap(ip.ostack.popInteger());
		}

		/// <summary>
		/// Get the current line cap.
		/// </summary>
		internal static void currentlinecap(Interpreter ip)
		{
			ip.ostack.pushRef(new IntegerType(ip.GraphicsState.currentlinecap()));
		}

		/// <summary>
		/// Set the current line miterlimit.
		/// </summary>
		internal static void setmiterlimit(Interpreter ip)
		{
			ip.GraphicsState.setmiterlimit(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue());
		}

		/// <summary>
		/// Get the current line miterlimit.
		/// </summary>
		internal static void currentmiterlimit(Interpreter ip)
		{
			ip.ostack.pushRef(new RealType(ip.GraphicsState.currentmiterlimit()));
		}

		/// <summary>
		/// Set the current stroke adjustment.
		/// </summary>
		internal static void setstrokeadjust(Interpreter ip)
		{
			ip.GraphicsState.StrokeAdjustment = ip.ostack.popBoolean();
		}

		/// <summary>
		/// Get the current stroke adjustment.
		/// </summary>
		internal static void currentstrokeadjustment(Interpreter ip)
		{
			ip.ostack.pushRef(new BoolType(ip.GraphicsState.StrokeAdjustment));
		}

		/// <summary>
		/// Set the current overprint.
		/// </summary>
		internal static void setoverprint(Interpreter ip)
		{
			ip.GraphicsState.Overprint = ip.ostack.popBoolean();
		}

		/// <summary>
		/// Get the current overprint.
		/// </summary>
		internal static void currentoverprint(Interpreter ip)
		{
			ip.ostack.pushRef(new BoolType(ip.GraphicsState.Overprint));
		}

		/// <summary>
		/// Set the current gray value.
		/// </summary>
		internal static void setgray(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			ArrayType transferproc = gc.currenttransfer();
			if (transferproc != null)
			{
				ip.estack.run(ip, transferproc);
			}
			gc.setgray(((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue());
		}

		/// <summary>
		/// Get the current gray value.
		/// </summary>
		internal static void currentgray(Interpreter ip)
		{
			ip.ostack.pushRef(new RealType(ip.GraphicsState.currentgray()));
		}

		/// <summary>
		/// Set the current color.
		/// </summary>
		internal static void setcolor(Interpreter ip)
		{
			Any any = ip.ostack.top();
			if (any is DictType)
			{
				setpattern(ip);
			}
			else
			{
				GraphicsState gc = ip.GraphicsState;
				int colorSpaceCode = gc.ColorSpace;
				switch (colorSpaceCode)
				{
				case ColorSpace.TYPE_GRAY:
					setgray(ip);
					break;
				case ColorSpace.TYPE_RGB:
					setrgbcolor(ip);
					break;
				case ColorSpace.TYPE_CMYK:
					setcmykcolor(ip);
					break;
				default:
					throw new Stop(Stoppable_Fields.INTERNALERROR);
				}
			}
		}

		/// <summary>
		/// Get the current color.
		/// </summary>
		internal static void currentcolor(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			int colorSpaceCode = gc.ColorSpace;
			switch (colorSpaceCode)
			{
			case ColorSpace.TYPE_GRAY:
				currentgray(ip);
				break;
			case ColorSpace.TYPE_RGB:
				currentrgbcolor(ip);
				break;
			case ColorSpace.TYPE_CMYK:
				currentcmykcolor(ip);
				break;
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR);
			}
		}

		/// <summary>
		/// Set the current rgb color.
		/// </summary>
		internal static void setrgbcolor(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			ArrayType transferproc = gc.currentbluetransfer();
			if (transferproc != null)
			{
				ip.estack.run(ip, transferproc);
			}
			float blue = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			transferproc = gc.currentgreentransfer();
			if (transferproc != null)
			{
				ip.estack.run(ip, transferproc);
			}
			float green = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			transferproc = gc.currentredtransfer();
			if (transferproc != null)
			{
				ip.estack.run(ip, transferproc);
			}
			float red = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			gc.setrgbcolor(red, green, blue);
		}

		/// <summary>
		/// Get the current rgb color.
		/// </summary>
		internal static void currentrgbcolor(Interpreter ip)
		{
			float[] color = ip.GraphicsState.currentrgbcolor();
			ip.ostack.pushRef(new RealType(color[0]));
			ip.ostack.pushRef(new RealType(color[1]));
			ip.ostack.pushRef(new RealType(color[2]));
		}

		/// <summary>
		/// Set the current hsb color.
		/// </summary>
		internal static void sethsbcolor(Interpreter ip)
		{
			float b = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float s = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float h = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			ip.GraphicsState.sethsbcolor(h, s, b);
		}

		/// <summary>
		/// Get the current hsb color.
		/// </summary>
		internal static void currenthsbcolor(Interpreter ip)
		{
			float[] color = ip.GraphicsState.currenthsbcolor();
			ip.ostack.pushRef(new RealType(color[0]));
			ip.ostack.pushRef(new RealType(color[1]));
			ip.ostack.pushRef(new RealType(color[2]));
		}

		/// <summary>
		/// Set the current cmyk color.
		/// </summary>
		internal static void setcmykcolor(Interpreter ip)
		{
			float black = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float yellow = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float magenta = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float cyan = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			ip.GraphicsState.setcmykcolor(cyan, magenta, yellow, black);
		}

		/// <summary>
		/// Get the current cmyk color.
		/// </summary>
		internal static void currentcmykcolor(Interpreter ip)
		{
			float[] color = ip.GraphicsState.currentcmykcolor();
			ip.ostack.pushRef(new RealType(color[0]));
			ip.ostack.pushRef(new RealType(color[1]));
			ip.ostack.pushRef(new RealType(color[2]));
			ip.ostack.pushRef(new RealType(color[3]));
		}

		/// <summary>
		/// Set the current color rendering dictionary.
		/// </summary>
		internal static void setcolorrendering(Interpreter ip)
		{
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			ip.GraphicsState.ColorRendering = dict;
		}

		/// <summary>
		/// Get the current color rendering dictionary.
		/// </summary>
		internal static void currentcolorrendering(Interpreter ip)
		{
			ip.ostack.push(ip.GraphicsState.ColorRendering);
		}

		/// <summary>
		/// Set the current colorspace.
		/// </summary>
		internal static void setcolorspace(Interpreter ip)
		{
			Any colorSpace = ip.ostack.pop(Types_Fields.STRING | Types_Fields.NAME | Types_Fields.ARRAY);
			string colorSpaceName;
			if (colorSpace is ArrayType)
			{
				ArrayType array = (ArrayType) colorSpace;
				colorSpaceName = array.get(0).ToString();
			}
			else
			{
				colorSpaceName = colorSpace.ToString();
			}
			GraphicsState gc = ip.GraphicsState;
			if (colorSpaceName.Equals(DEVICE_GRAY))
			{
				gc.ColorSpace = ColorSpace.CS_GRAY;
			}
			else if (colorSpaceName.Equals(DEVICE_RGB))
			{
				gc.ColorSpace = ColorSpace.CS_sRGB;
			}
			else if (colorSpaceName.Equals(DEVICE_CMYK))
			{
				gc.ColorSpace = ColorSpace.TYPE_CMYK;
			}
			else if (colorSpaceName.Equals("CIEBasedABC"))
			{
				// TODO: setcolorspace
				gc.ColorSpace = ColorSpace.CS_sRGB;
			}
			else if (colorSpaceName.Equals("CIEBasedA"))
			{
				// TODO: setcolorspace
			}
			else if (colorSpaceName.Equals("Separation"))
			{
				// TODO: setcolorspace
			}
			else if (colorSpaceName.Equals("Indexed"))
			{
				// TODO: setcolorspace
			}
			else if (colorSpaceName.Equals("Pattern"))
			{
				// TODO: setcolorspace
			}
			else
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, colorSpaceName + " not implemented");
			}
		}

		/// <summary>
		/// Get the current colorspace.
		/// </summary>
		internal static void currentcolorspace(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			int colorSpaceCode = gc.ColorSpace;
			string colorSpaceName = null;
			switch (colorSpaceCode)
			{
			case ColorSpace.TYPE_GRAY:
				colorSpaceName = DEVICE_GRAY;
				break;
			case ColorSpace.TYPE_RGB:
				colorSpaceName = DEVICE_RGB;
				break;
			case ColorSpace.TYPE_CMYK:
				colorSpaceName = DEVICE_CMYK;
				break;
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR, colorSpaceName + " not yet implemented");
			}
			ArrayType array = new ArrayType(ip.vm, 1);
			array.put(ip.vm, 0, new NameType(colorSpaceName));
			ip.ostack.pushRef(array);
		}

		/// <summary>
		/// Set the current pattern.
		/// </summary>
		internal static void setpattern(Interpreter ip)
		{
			DictType pattern = (DictType) ip.ostack.pop(Types_Fields.DICT);
			// TODO: check pattern
			ip.GraphicsState.Pattern = pattern;
		}

		/// <summary>
		/// Get the current pattern.
		/// </summary>
		internal static void currentpattern(Interpreter ip)
		{
			ip.ostack.push(ip.GraphicsState.Pattern);
		}

		/// <summary>
		/// Set dash parameters.
		/// </summary>
		internal static void setdash(Interpreter ip)
		{
			float phase = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			ArrayType array = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ip.GraphicsState.setdash(array, phase);
		}

		/// <summary>
		/// Get dash parameters.
		/// </summary>
		internal static void currentdash(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			ArrayType array = gc.currentdasharray();
			if (array == null)
			{
				ip.ostack.pushRef(new ArrayType(ip.vm, 0));
			}
			else
			{
				ip.ostack.push(array);
			}
			ip.ostack.pushRef(new RealType(gc.currentdashphase()));
		}

		/// <summary>
		/// Set the current halftone screen function.
		/// </summary>
		internal static void setscreen(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			Any any = ip.ostack.pop(Types_Fields.ARRAY | Types_Fields.DICT);
			if (any is ArrayType)
			{
				ArrayType proc = (ArrayType) any;
				NumberType angle = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType freq = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				DictType dict = createHalftone(ip, freq, angle, proc);
				setHalftone(gstate, dict);
			}
			else
			{
				setHalftone(gstate, (DictType) any);
			}
		}

		/// <summary>
		/// Get the current halftone screen parameters.
		/// </summary>
		internal static void currentscreen(Interpreter ip)
		{
			DictType halftone = ip.GraphicsState.Halftone;
			if (halftone == null)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "no initial halftone defined");
			}
			ip.ostack.pushRef(halftone.get("Frequency"));
			ip.ostack.pushRef(halftone.get("Angle"));
			ip.ostack.push(halftone.get("SpotFunction"));
		}

		/// <summary>
		/// Set the current halftone dictionary.
		/// </summary>
		internal static void sethalftone(Interpreter ip)
		{
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			GraphicsState gstate = ip.GraphicsState;
			setHalftone(gstate, dict);
		}

		/// <summary>
		/// Set the current halftone dictionary.
		/// </summary>
		internal static void setHalftone(GraphicsState gstate, DictType dict)
		{
			if (!dict.known("HalftoneType"))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "HalftoneType");
			}
			if (!dict.known("Frequency"))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "Frequency");
			}
			if (!dict.known("Angle"))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "Angle");
			}
			if (!dict.known("SpotFunction"))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK, "SpotFunction");
			}
			gstate.Halftone = dict;
		}

		/// <summary>
		/// Set the current halftone dictionary.
		/// </summary>
		internal static DictType createHalftone(Interpreter ip, NumberType freq, NumberType angle, ArrayType proc)
		{
			DictType halftone = new DictType(ip.vm, 4);
			halftone.put(ip.vm, "HalftoneType", new IntegerType(1));
			halftone.put(ip.vm, "Frequency", freq);
			halftone.put(ip.vm, "Angle", angle);
			halftone.put(ip.vm, "SpotFunction", proc);
			return halftone;
		}

		/// <summary>
		/// Get the current halftone parameters.
		/// </summary>
		internal static void currenthalftone(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			DictType halftone = gstate.Halftone;
			if (halftone == null)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "no initial halftone defined");
			}
			ip.ostack.push(halftone);
		}

		/// <summary>
		/// Set the current halftone phase.
		/// </summary>
		internal static void sethalftonephase(Interpreter ip)
		{
			int y = ip.ostack.popInteger();
			int x = ip.ostack.popInteger();
			GraphicsState gstate = ip.GraphicsState;
			gstate.setHalftonePhase(x, y);
		}

		/// <summary>
		/// Get the current halftone phase.
		/// </summary>
		internal static void currenthalftonephase(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			int[] phase = gstate.HalftonePhase;
			ip.ostack.pushRef(new IntegerType(phase[0]));
			ip.ostack.pushRef(new IntegerType(phase[1]));
		}

		/// <summary>
		/// Set the current transfer function.
		/// </summary>
		internal static void settransfer(Interpreter ip)
		{
			ip.GraphicsState.settransfer((ArrayType) ip.ostack.pop(Types_Fields.ARRAY));
		}

		/// <summary>
		/// Get the current transfer function.
		/// </summary>
		internal static void currenttransfer(Interpreter ip)
		{
			pushTransferProc(ip, ip.GraphicsState.currenttransfer());
		}

		/// <summary>
		/// If proc is null, make one up and push onto stack.
		/// </summary>
		private static void pushTransferProc(Interpreter ip, ArrayType proc)
		{
			if (proc == null)
			{
				proc = new ArrayType(ip.vm, 0);
				proc.cvx();
			}
			ip.ostack.push(proc);
		}

		/// <summary>
		/// Set the current color transfer function.
		/// </summary>
		internal static void setcolortransfer(Interpreter ip)
		{
			ArrayType gray = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType blue = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType green = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType red = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ip.GraphicsState.setcolortransfer(red, green, blue, gray);
		}

		/// <summary>
		/// Get the current color transfer function.
		/// </summary>
		internal static void currentcolortransfer(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			pushTransferProc(ip, gc.currentredtransfer());
			pushTransferProc(ip, gc.currentgreentransfer());
			pushTransferProc(ip, gc.currentbluetransfer());
			pushTransferProc(ip, gc.currenttransfer());
		}

		/// <summary>
		/// Set the current color transfer functions.
		/// </summary>
		internal static void setcolorscreen(Interpreter ip)
		{
			ArrayType grayproc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			NumberType grayang = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			NumberType grayfreq = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
			ArrayType blueproc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			double blueang = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double bluefreq = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ArrayType greenproc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			double greenang = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double greenfreq = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ArrayType redproc = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			double redang = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double redfreq = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			Screen bluescreen = new Screen(bluefreq, blueang, blueproc);
			Screen greenscreen = new Screen(greenfreq, greenang, greenproc);
			Screen redscreen = new Screen(redfreq, redang, redproc);
			DictType dict = createHalftone(ip, grayfreq, grayang, grayproc);
			GraphicsState gstate = ip.GraphicsState;
			gstate.Halftone = dict;
			gstate.setColorScreen(redscreen, greenscreen, bluescreen);
		}

		/// <summary>
		/// Get the current color transfer functions.
		/// </summary>
		internal static void currentcolorscreen(Interpreter ip)
		{
			GraphicsState gc = ip.GraphicsState;
			Screen redscreen = gc.currentredscreen();
			ip.ostack.pushRef(new RealType(redscreen.Frequency));
			ip.ostack.pushRef(new RealType(redscreen.Angle));
			ip.ostack.push(redscreen.getProcedure(ip.vm));
			Screen greenscreen = gc.currentgreenscreen();
			ip.ostack.pushRef(new RealType(greenscreen.Frequency));
			ip.ostack.pushRef(new RealType(greenscreen.Angle));
			ip.ostack.push(greenscreen.getProcedure(ip.vm));
			Screen bluescreen = gc.currentbluescreen();
			ip.ostack.pushRef(new RealType(bluescreen.Frequency));
			ip.ostack.pushRef(new RealType(bluescreen.Angle));
			ip.ostack.push(bluescreen.getProcedure(ip.vm));
			DictType halftone = ip.GraphicsState.Halftone;
			ip.ostack.pushRef(halftone.get("Frequency"));
			ip.ostack.pushRef(halftone.get("Angle"));
			ip.ostack.push(halftone.get("SpotFunction"));
		}

		/// <summary>
		/// Set the current black generation function.
		/// </summary>
		internal static void setblackgeneration(Interpreter ip)
		{
			ip.GraphicsState.setblackgeneration((ArrayType) ip.ostack.pop(Types_Fields.ARRAY));
		}

		/// <summary>
		/// Get the current black generation function.
		/// </summary>
		internal static void currentblackgeneration(Interpreter ip)
		{
			pushTransferProc(ip, ip.GraphicsState.currentblackgeneration());
		}

		/// <summary>
		/// Set the current under color removal function.
		/// </summary>
		internal static void setundercolorremoval(Interpreter ip)
		{
			ip.GraphicsState.setundercolorremoval((ArrayType) ip.ostack.pop(Types_Fields.ARRAY));
		}

		/// <summary>
		/// Get the current under color removal function.
		/// </summary>
		internal static void currentundercolorremoval(Interpreter ip)
		{
			pushTransferProc(ip, ip.GraphicsState.currentundercolorremoval());
		}

		/// <summary>
		/// Set the page device.
		/// </summary>
		internal static void setpagedevice(Interpreter ip)
		{
			DictType dict = (DictType) ip.ostack.pop(Types_Fields.DICT);
			GraphicsState gstate = ip.GraphicsState;
			Device device = loadDevice(gstate, dict);
			PropertyDescriptor[] desc = getDeviceProperties(device);
			if (desc != null)
			{
				for (int i = 0; i < desc.Length; i++)
				{
					PropertyDescriptor pd = desc[i];
					string name = pd.Name;
					Any any = dict.get(name);
					if (any != null)
					{
						try
						{
							object val = device.convertType(name, any.cvj());
							object[] args = new object[1];
							args[0] = val;
							System.Reflection.MethodInfo setter = pd.WriteMethod;
							setter.invoke(device, args);
						}
						catch (Exception ex)
						{
							System.Console.WriteLine("setpagedevice: " + ex);
						}
					}
				}
			}
			gstate.setpagedevice(device);
		}

		/// <summary>
		/// Get the page device properties.
		/// </summary>
		internal static void currentpagedevice(Interpreter ip)
		{
			DictType dict = new DictType(ip.vm, 10);
			GraphicsState gstate = ip.GraphicsState;
			Device device = loadDevice(gstate, dict);
			PropertyDescriptor[] desc = getDeviceProperties(device);
			if (desc != null)
			{
				for (int i = 0; i < desc.Length; i++)
				{
					PropertyDescriptor pd = desc[i];
					string name = pd.Name;
					System.Reflection.MethodInfo getter = pd.ReadMethod;
					if (getter != null)
					{
						try
						{
							object obj = getter.invoke(device, null);
							Any value;
							if (obj is int?)
							{
								int val = ((int?) obj).Value;
								value = new IntegerType(val);
							}
							else if (obj is float?)
							{
								float val = ((float?) obj).Value;
								value = new RealType(val);
							}
							else if (obj is float[])
							{
								float[] val = (float[]) obj;
								int n = val.Length;
								ArrayType a = new ArrayType(ip.vm, n);
								for (int j = 0; j < n; j++)
								{
									a.put(ip.vm, j, new RealType(val[j]));
								}
								value = a;
							}
							else
							{
								value = new StringType(ip.vm, obj.ToString());
							}
							dict.put(ip.vm, name, value);
						}
						catch (Exception ex)
						{
							System.Console.WriteLine(ex);
						}
					}
				}
			}
			ip.ostack.pushRef(dict);
		}

		private static Device loadDevice(GraphicsState gstate, DictType dict)
		{
			try
			{
				Device device;
				Any deviceClassName = dict.get("class");
				if (deviceClassName != null)
				{
					Type clazz = Type.GetType(deviceClassName.ToString());
					device = (Device) System.Activator.CreateInstance(clazz);
				}
				else
				{
					device = gstate.defaultdevice();
				}
				return (Device) device;
			}
			catch (Exception ex)
			{
				throw new Stop(Stoppable_Fields.UNDEFINED, ex.ToString());
			}
		}

		private static PropertyDescriptor[] getDeviceProperties(Device device)
		{
			try
			{
				BeanInfo info = Introspector.getBeanInfo(device.GetType());
				return info.PropertyDescriptors;
			}
			catch (Exception ex)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, ex.ToString());
			}
		}

		/// <summary>
		/// Set the null device.
		/// </summary>
		internal static void nulldevice(Interpreter ip)
		{
			ip.GraphicsState.nulldevice();
		}

		/// <summary>
		/// Set width parameters.
		/// </summary>
		internal static void setcharwidth(Interpreter ip)
		{
			float wy = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float wx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			ip.GraphicsState.setcharwidth(wx, wy);
		}

		/// <summary>
		/// Set cache parameters.
		/// </summary>
		internal static void setcachedevice(Interpreter ip)
		{
			double ury = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double urx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double lly = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double llx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			float wy = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			float wx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
			CharWidth cw = new CharWidth(0, 0, wx, wy);
			Rectangle2D rect = new Rectangle2D.Double(llx, lly, urx - llx, ury - lly);
			ip.GraphicsState.setcachedevice(cw, rect);
		}

		/// <summary>
		/// Set cache parameters.
		/// </summary>
		internal static void setcachedevice2(Interpreter ip)
		{
			double vy = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double vx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double ury = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double urx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double lly = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double llx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double wy = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double wx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double w0y = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			double w0x = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).realValue();
			ip.GraphicsState.setcachedevice2(w0x, w0y, wx, wy, llx, lly, urx, ury, vx, vy);
		}

		internal static void cachestatus(Interpreter ip)
		{
			CacheDevice cdev = ip.GraphicsState.cachedevice();
			int bsize = 1024;
			int bmax = 1024;
			int msize = 100;
			int mmax = 100;
			int csize = cdev.CacheSize;
			int cmax = cdev.MaxCacheSize;
			int blimit = 1024;
			ip.ostack.pushRef(new IntegerType(bsize));
			ip.ostack.pushRef(new IntegerType(bmax));
			ip.ostack.pushRef(new IntegerType(msize));
			ip.ostack.pushRef(new IntegerType(mmax));
			ip.ostack.pushRef(new IntegerType(csize));
			ip.ostack.pushRef(new IntegerType(cmax));
			ip.ostack.pushRef(new IntegerType(blimit));
		}

		internal static void setcachelimit(Interpreter ip)
		{
			ip.ostack.pop(Types_Fields.INTEGER);
		}

		internal static void setcacheparams(Interpreter ip)
		{
			IntegerType upper = (IntegerType) ip.ostack.pop(Types_Fields.INTEGER);
			IntegerType lower = (IntegerType) ip.ostack.pop(Types_Fields.INTEGER);
			IntegerType size = (IntegerType) ip.ostack.pop(Types_Fields.INTEGER);
			ip.ostack.cleartomark();
			CacheDevice cdev = ip.GraphicsState.cachedevice();
			cdev.MaxCacheSize = size.intValue();
		}

		internal static void currentcacheparams(Interpreter ip)
		{
			CacheDevice cdev = ip.GraphicsState.cachedevice();
			int size = cdev.MaxCacheSize;
			ip.ostack.pushRef(new MarkType());
			ip.ostack.pushRef(new IntegerType(size));
			ip.ostack.pushRef(new IntegerType(100));
			ip.ostack.pushRef(new IntegerType(200));
		}

	}

}