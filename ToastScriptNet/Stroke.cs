using com.softhub.ps.device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToastScriptNet
{
    public class Stroke
    {
        public Stroke(float width, int cap, int join, float miter, float[] array, float phase)
        {

        }
    }

    public class AffineTransform
    {
        public AffineTransform(float a, float b, float c, float d, float e, float f)
        {

        }

        public AffineTransform Transform
        {
            get; set;
        }

        public AffineTransform createInverse()
        {
            return null;
        }

        public void concatenate(AffineTransform affineTransform)
        {
        }

        public void translate(float x, float y)
        {

        }
    }


    public class Color
    {
        public Color(float r, float g, float b)
        {
        }
    }

    public class Paint
    {
    }

    public class Shape
    {
    }

    public class Rectangle2D : Shape
    {
    }

    public class Graphics2D
    {
        public Shape Clip;
        
        public void clip(Shape shape)
        {

        }
    }

    public class Dimension
    {
        public int width;
        public int height;

        public Dimension(int width, int height)
        {

        }
    }

    public class Image
    {
        public int width;
        public int height;

        public Image(int width, int height)
        {

        }

    }

    public class BufferedImage : Bitmap
    {
        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public Image Image => throw new NotImplementedException();

        public BufferedImage(int width, int height)
        {

        }

        public Graphics2D createGraphics()
        {
            return null;
        }

        public void draw(int x, int y, int color)
        {
            throw new NotImplementedException();
        }

        public void draw(int x, int y, Color color)
        {
            throw new NotImplementedException();
        }

        public void drawImage(Graphics2D g, AffineTransform xform)
        {
            throw new NotImplementedException();
        }
    }

    public class Point2D<T>
    {
        public T X;
        public T Y;

        public Point2D(T x, T y)
        {

        }
    }

    public class PropertyDescriptor
    {

    }
}
