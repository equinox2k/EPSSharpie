using System;
using System.Threading;
using System.IO;

namespace com.softhub.ts
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

	using Drawable = com.softhub.ps.graphics.Drawable;

	public class PageCanvas : JPanel
	{

		private Painter painter;
		private Drawable content;
		private BufferedImage image;
		private float scale = 1;
		private float contentScale = 1;
		private bool printing;

		public PageCanvas()
		{
			try
			{
				jbInit();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void jbInit() throws Exception
		private void jbInit()
		{
			Background = Color.white;
		}

		public virtual void setContent(Drawable content, float contentScale)
		{
			this.content = content;
			this.contentScale = contentScale;
		}

		public virtual object Content
		{
			get
			{
				return content;
			}
		}

		public virtual float Scale
		{
			set
			{
				this.scale = value;
				image = null;
			}
			get
			{
				return scale;
			}
		}


		public virtual void activate(bool active)
		{
			if (active)
			{
				if (image != null)
				{
					int w = image.getWidth(this);
					int h = image.getHeight(this);
					Dimension d = Size;
					if (w != d.width || h != d.height)
					{
						image = null;
					}
				}
			}
			else
			{
				image = null;
			}
			if (image == null && painter != null)
			{
				content.interrupt();
			}
		}

		public virtual void paint(Graphics g)
		{
			base.paint(g);
			if (content != null)
			{
				Graphics2D g2d = (Graphics2D) g;
				if (printing)
				{
					AffineTransform xform = g2d.Transform;
					if (!xform.Identity)
					{
						// Why are we called twice? First call is ident matrix.
						float dpi = Toolkit.ScreenResolution;
						float factor = 72 / dpi;
						g2d.scale(factor, factor);
						draw(g2d);
					}
				}
				else
				{
					drawImage(g2d);
				}
			}
		}

		public virtual void print(Graphics g)
		{
			try
			{
				printing = true;
				base.print(g);
			}
			finally
			{
				printing = false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.OutputStream stream, String format) throws java.io.IOException
		public virtual void save(Stream stream, string format)
		{
			BufferedImage image = createImage(BufferedImage.TYPE_INT_RGB);
			Graphics2D g = image.createGraphics();
			Dimension d = Size;
			g.setClip(0, 0, d.width, d.height);
			g.Color = Color.white;
			g.fillRect(0, 0, d.width, d.height);
			draw(g);
			g.dispose();
			System.Collections.IEnumerator iterator = ImageIO.getImageWritersByFormatName(format);
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (iterator.hasNext())
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ImageWriter encoder = (ImageWriter) iterator.next();
				JPEGImageWriteParam param = new JPEGImageWriteParam(null);
				param.CompressionMode = ImageWriteParam.MODE_EXPLICIT;
				param.CompressionQuality = 0.9f;
				encoder.Output = ImageIO.createImageOutputStream(stream);
				IIOImage iioImage = new IIOImage(image, null, null);
				encoder.write(null, iioImage, param);
			}
			else
			{
				throw new Exception(format + " not supported");
			}
		}

		protected internal virtual void draw(Graphics2D g)
		{
			if (content == null)
			{
				return;
			}
			float factor = scale / contentScale;
			g.Clip = Bounds;
			g.scale(factor, factor);
			g.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
			content.draw(g);
		}

		protected internal virtual void drawImage(Graphics2D g)
		{
			if (image == null)
			{
				image = createImage();
				if (painter == null)
				{
					Graphics2D g2d = image.createGraphics();
					painter = new Painter(this, g2d);
				}
			}
			g.drawImage(image, null, 0, 0);
		}

		protected internal virtual BufferedImage createImage()
		{
			return createImage(BufferedImage.TYPE_INT_ARGB);
		}

		protected internal virtual BufferedImage createImage(int type)
		{
			Dimension d = Size;
			return new BufferedImage(d.width, d.height, type);
		}

		internal class Painter : ThreadStart
		{
			private readonly PageCanvas outerInstance;


			internal Graphics2D graphics;

			internal Painter(PageCanvas outerInstance, Graphics2D graphics)
			{
				this.outerInstance = outerInstance;
				this.graphics = graphics;
				Thread thread = new Thread(this);
				thread.Priority = Thread.NORM_PRIORITY;
				thread.Start();
			}

			public virtual void run()
			{
				outerInstance.draw(graphics);
				graphics.dispose();
				outerInstance.painter = null;
				repaint();
			}

		}

	}

}