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
	/// 
	/// A matrix is defined as an array of 6 numbers.
	/// 
	/// The matrix
	///				| a c tx |
	///				| b d ty |
	///				| 0 0 1  |
	/// 
	/// is representet by this array:
	/// 
	///				[ a b c d tx ty ]
	/// </summary>

	internal sealed class MatrixOp : Stoppable, Types
	{

		private static readonly string[] OPNAMES = new string[] {"matrix", "initmatrix", "identmatrix", "defaultmatrix", "currentmatrix", "setmatrix", "translate", "scale", "rotate", "concat", "concatmatrix", "transform", "itransform", "dtransform", "idtransform", "invertmatrix"};

		internal static void install(Interpreter ip)
		{
			ip.installOp(OPNAMES, typeof(MatrixOp));
		}

		internal static void matrix(Interpreter ip)
		{
			ArrayType matrix = new ArrayType(ip.vm, 6);
			matrix.put(ip.vm, 0, new IntegerType(1));
			matrix.put(ip.vm, 1, new IntegerType(0));
			matrix.put(ip.vm, 2, new IntegerType(0));
			matrix.put(ip.vm, 3, new IntegerType(1));
			matrix.put(ip.vm, 4, new IntegerType(0));
			matrix.put(ip.vm, 5, new IntegerType(0));
			ip.ostack.pushRef(matrix);
		}

		internal static void initmatrix(Interpreter ip)
		{
			GraphicsState gstate = ip.GraphicsState;
			gstate.initmatrix(gstate.currentdevice());
		}

		internal static void identmatrix(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.top(Types_Fields.ARRAY);
			if (matrix.length() != 6)
			{
				throw new Stop(Stoppable_Fields.RANGECHECK);
			}
			if (!matrix.check(Types_Fields.NUMBER | Types_Fields.NULL))
			{
				throw new Stop(Stoppable_Fields.TYPECHECK);
			}
			matrix.put(ip.vm, 0, new RealType(1));
			matrix.put(ip.vm, 1, new RealType(0));
			matrix.put(ip.vm, 2, new RealType(0));
			matrix.put(ip.vm, 3, new RealType(1));
			matrix.put(ip.vm, 4, new RealType(0));
			matrix.put(ip.vm, 5, new RealType(0));
		}

		internal static void defaultmatrix(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.top(Types_Fields.ARRAY);
			matrix.put(ip.vm, ip.GraphicsState.currentdevice().DefaultMatrix);
		}

		internal static void currentmatrix(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.top(Types_Fields.ARRAY);
			matrix.put(ip.vm, ip.GraphicsState.currentmatrix());
		}

		internal static void setmatrix(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ip.GraphicsState.setmatrix(matrix.toTransform());
		}

		internal static void translate(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			if (any is ArrayType)
			{
				ArrayType matrix = (ArrayType) any;
				if (!matrix.check(Types_Fields.NUMBER | Types_Fields.NULL, 6))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				NumberType ty = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType tx = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				matrix.put(ip.vm, 0, new IntegerType(1));
				matrix.put(ip.vm, 1, new IntegerType(0));
				matrix.put(ip.vm, 2, new IntegerType(0));
				matrix.put(ip.vm, 3, new IntegerType(1));
				matrix.put(ip.vm, 4, tx);
				matrix.put(ip.vm, 5, ty);

				ip.ostack.pushRef(matrix);
			}
			else
			{
				NumberType ty = (NumberType) any;
				NumberType tx = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				ip.GraphicsState.translate(tx.realValue(), ty.realValue());
			}
		}

		internal static void scale(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			if (any is ArrayType)
			{
				ArrayType matrix = (ArrayType) any;
				if (!matrix.check(Types_Fields.NUMBER | Types_Fields.NULL, 6))
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				NumberType sy = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType sx = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				matrix.put(ip.vm, 0, sx);
				matrix.put(ip.vm, 1, new IntegerType(0));
				matrix.put(ip.vm, 2, new IntegerType(0));
				matrix.put(ip.vm, 3, sy);
				matrix.put(ip.vm, 4, new IntegerType(0));
				matrix.put(ip.vm, 5, new IntegerType(0));
				ip.ostack.pushRef(matrix);
			}
			else
			{
				NumberType sy = (NumberType) any;
				NumberType sx = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				ip.GraphicsState.scale(sx.realValue(), sy.realValue());
			}
		}

		internal static void scale(Interpreter ip, ArrayType matrix, NumberType sx, NumberType sy)
		{
			NumberType m0 = (NumberType) matrix.get(0);
			NumberType m1 = (NumberType) matrix.get(1);
			NumberType m2 = (NumberType) matrix.get(2);
			NumberType m3 = (NumberType) matrix.get(3);
			matrix.put(ip.vm, 0, ArithOp.mul(sx, m0));
			matrix.put(ip.vm, 1, ArithOp.mul(sx, m1));
			matrix.put(ip.vm, 2, ArithOp.mul(sy, m2));
			matrix.put(ip.vm, 3, ArithOp.mul(sy, m3));
		}

		internal static void rotate(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			if (any is ArrayType)
			{
				NumberType degree = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				AffineTransform xform = ((ArrayType) any).toTransform();
				xform.rotate((float)(degree.realValue() * Math.PI / 180));
				ip.ostack.pushRef(((ArrayType) any).put(ip.vm, xform));
			}
			else
			{
				ip.GraphicsState.rotate((float)(((NumberType) any).realValue() * Math.PI / 180));
			}
		}

		internal static void concat(Interpreter ip)
		{
			ArrayType matrix = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ip.GraphicsState.concat(matrix.toTransform());
		}

		internal static void concatmatrix(Interpreter ip)
		{
			ArrayType matrix3 = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType matrix2 = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType matrix1 = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			concat(ip, matrix1, matrix2, matrix3);
			ip.ostack.pushRef(matrix3);
		}

		internal static void concat(Interpreter ip, ArrayType m1, ArrayType m2, ArrayType m3)
		{
			AffineTransform xform1 = m1.toTransform();
			AffineTransform xform2 = m2.toTransform();
			xform2.concatenate(xform1);
			m3.put(ip.vm, xform2);
		}

		internal static void transform(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			if (any is ArrayType)
			{
				ArrayType matrix = (ArrayType) any;
				if (!matrix.Matrix)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				NumberType ty = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType tx = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType m0 = (NumberType) matrix.get(0);
				NumberType m1 = (NumberType) matrix.get(1);
				NumberType m2 = (NumberType) matrix.get(2);
				NumberType m3 = (NumberType) matrix.get(3);
				NumberType m4 = (NumberType) matrix.get(4);
				NumberType m5 = (NumberType) matrix.get(5);
				ip.ostack.pushRef(ArithOp.add(ArithOp.add(ArithOp.mul(m0, tx), ArithOp.mul(m2, ty)), m4));
				ip.ostack.pushRef(ArithOp.add(ArithOp.add(ArithOp.mul(m1, tx), ArithOp.mul(m3, ty)), m5));
			}
			else
			{
				float ty = ((NumberType) any).floatValue();
				float tx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				AffineTransform ctm = ip.GraphicsState.currentmatrix();
				Point2D t = new Point2D.Double(tx, ty);
				Point2D tt = ctm.transform(t, null);
				ip.ostack.pushRef(new RealType(tt.X));
				ip.ostack.pushRef(new RealType(tt.Y));
			}
		}

		internal static void dtransform(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			if (any is ArrayType)
			{
				ArrayType matrix = (ArrayType) any;
				if (!matrix.Matrix)
				{
					throw new Stop(Stoppable_Fields.TYPECHECK);
				}
				NumberType ty = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType tx = (NumberType) ip.ostack.pop(Types_Fields.NUMBER);
				NumberType m0 = (NumberType) matrix.get(0);
				NumberType m1 = (NumberType) matrix.get(1);
				NumberType m2 = (NumberType) matrix.get(2);
				NumberType m3 = (NumberType) matrix.get(3);
				ip.ostack.pushRef(ArithOp.add(ArithOp.mul(m0, tx), ArithOp.mul(m2, ty)));
				ip.ostack.pushRef(ArithOp.add(ArithOp.mul(m1, tx), ArithOp.mul(m3, ty)));
			}
			else
			{
				float ty = ((NumberType) any).floatValue();
				float tx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				AffineTransform ctm = ip.GraphicsState.currentmatrix();
				Point2D t = new Point2D.Double(tx, ty);
				Point2D tt = ctm.deltaTransform(t, null);
				ip.ostack.pushRef(new RealType(tt.X));
				ip.ostack.pushRef(new RealType(tt.Y));
			}
		}

		internal static void itransform(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			AffineTransform xform;
			float tx, ty;
			if (any is ArrayType)
			{
				ty = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				tx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				xform = ((ArrayType) any).toTransform();
			}
			else
			{
				ty = ((NumberType) any).floatValue();
				tx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				xform = ip.GraphicsState.currentmatrix();
			}
			try
			{
				Point2D t = new Point2D.Double(tx, ty);
				Point2D tt = xform.inverseTransform(t, null);
				ip.ostack.pushRef(new RealType(tt.X));
				ip.ostack.pushRef(new RealType(tt.Y));
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT, "non invertible matrix");
			}
		}

		internal static void idtransform(Interpreter ip)
		{
			Any any = ip.ostack.pop(Types_Fields.NUMBER | Types_Fields.ARRAY);
			AffineTransform xform;
			float tx, ty;
			if (any is ArrayType)
			{
				ty = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				tx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				xform = ((ArrayType) any).toTransform();
			}
			else
			{
				ty = ((NumberType) any).floatValue();
				tx = ((NumberType) ip.ostack.pop(Types_Fields.NUMBER)).floatValue();
				xform = ip.GraphicsState.currentmatrix();
			}
			try
			{
				double[] flatmatrix = new double[6];
				xform.getMatrix(flatmatrix);
				flatmatrix[4] = flatmatrix[5] = 0;
				xform = new AffineTransform(flatmatrix);
				Point2D t = new Point2D.Double(tx, ty);
				Point2D tt = xform.inverseTransform(t, null);
				ip.ostack.pushRef(new RealType(tt.X));
				ip.ostack.pushRef(new RealType(tt.Y));
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT, "non invertible matrix");
			}
		}

		internal static void invertmatrix(Interpreter ip)
		{
			ArrayType matrix2 = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			ArrayType matrix1 = (ArrayType) ip.ostack.pop(Types_Fields.ARRAY);
			AffineTransform xform1 = matrix1.toTransform();
			AffineTransform xform2 = null;
			try
			{
				xform2 = xform1.createInverse();
			}
			catch (NoninvertibleTransformException)
			{
				throw new Stop(Stoppable_Fields.UNDEFINEDRESULT, "non invertible matrix");
			}
			ip.ostack.pushRef(matrix2.put(ip.vm, xform2));
		}

	}

}