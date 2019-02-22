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
        public Stroke()
        {

        }

        public Stroke(float width, int cap, int join, float miter, float[] array, float phase)
        {

        }
    }

    public class AffineTransform
    {
        public AffineTransform()
        {

        }
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
        public float Width => throw new NotImplementedException();

        public float Height => throw new NotImplementedException();

        public Rectangle2D Bounds2D => throw new NotImplementedException();

        public Rectangle2D(float x, float y, float width, float height)
        {

        }

        public Rectangle2D createUnion(Rectangle2D rectangle2D)
        {
            return null;
        }
    }

    public class Graphics2D 
    {
        public Shape Clip;

        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public Image Image => throw new NotImplementedException();

        public void clip(Shape shape)
        {

        }

        public void draw(int x, int y, int color)
        {
            throw new NotImplementedException();
        }

        public void draw(int x, int y, Color color)
        {
            throw new NotImplementedException();
        }

        public void drawImage(BufferedImage g, AffineTransform xform)
        {
            throw new NotImplementedException();
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

    public class Image : BufferedImage
    {
        
        public Image(int width, int height) : base(width, height)
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

        public void setRGB(int x, int y, Color color)
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

    public class Point2D
    {
        public float X;
        public float Y;

        public Point2D(float x, float y)
        {

        }
    }

    public class PropertyDescriptor
    {
        public PropertyDescriptor(string a, Type type)
        {

        }
    }
}
