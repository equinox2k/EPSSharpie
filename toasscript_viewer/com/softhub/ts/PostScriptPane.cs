using System;
using System.Collections.Generic;
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

	using Interpreter = com.softhub.ps.Interpreter;
	using DefaultPageDevice = com.softhub.ps.device.DefaultPageDevice;
	using PageDevice = com.softhub.ps.device.PageDevice;
	using PageEvent = com.softhub.ps.device.PageEvent;
	using PageEventListener = com.softhub.ps.device.PageEventListener;
	using Drawable = com.softhub.ps.graphics.Drawable;
	using Console = com.softhub.ps.util.Console;
	using StreamAdapter = com.softhub.ps.util.StreamAdapter;
	using NavigationEvent = com.softhub.ts.@event.NavigationEvent;
	using NavigationListener = com.softhub.ts.@event.NavigationListener;
	using ViewEvent = com.softhub.ts.@event.ViewEvent;
	using ViewEventListener = com.softhub.ts.@event.ViewEventListener;

	public class PostScriptPane : JPanel, Viewable, Pageable, Printable, PageEventListener, NavigationListener, ActionListener
	{
		private const float DEFAULT_PAGE_WIDTH = 576;
		private const float DEFAULT_PAGE_HEIGHT = 720;
		private const float DEFAULT_PAGE_SCALE = 1;

		private PageFlowLayout layout = new PageFlowLayout();
		private StreamAdapter consoleAdapter = new StreamAdapter();
		private StreamAdapter interpreterAdapter = new StreamAdapter();
		private Console console;
		private Interpreter interpreter;
		private PageDevice device;
		private PageFormat pageFormat = new PageFormat();
		private List<object> pages = new List<object>();
		private List<object> listeners = new List<object>();
		private int currentPageIndex;
		private float currentPageWidth = DEFAULT_PAGE_WIDTH;
		private float currentPageHeight = DEFAULT_PAGE_HEIGHT;
		private float currentPageScale = DEFAULT_PAGE_SCALE;
		private int currentPageOrientation;
		private bool firstPage;

		public PostScriptPane()
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
			Layout = layout;
		}

		public virtual void init()
		{
			try
			{
				device = createPageDevice();
				device.addPageEventListener(this);
				consoleAdapter.connect(interpreterAdapter);
				console = new Console(consoleAdapter.@in, consoleAdapter.@out);
				interpreter = new Interpreter(interpreterAdapter.@in, interpreterAdapter.@out);
				float[] size = new float[] {currentPageWidth, currentPageHeight};
				device.PageSize = size;
				device.Orientation = currentPageOrientation;
				device.Scale = currentPageScale;
				interpreter.init(device);
				// set the initial page margins for printing
				Paper paper = pageFormat.Paper;
				double w = paper.Width;
				double h = paper.Height;
				paper.setImageableArea(0, 0, w, h);
				pageFormat.Paper = paper;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
		}

		public virtual void addViewEventListener(ViewEventListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeViewEventListener(ViewEventListener listener)
		{
			listeners.Remove(listener);
		}

		protected internal virtual void fireViewEvent(ViewEvent evt)
		{
			System.Collections.IEnumerator e = listeners.elements();
			while (e.MoveNext())
			{
				ViewEventListener listener = (ViewEventListener) e.Current;
				listener.viewChanged(evt);
			}
		}

		protected internal virtual void fireViewChangeEvent()
		{
			fireViewEvent(new ViewEvent(this, ViewEvent.PAGE_CHANGE));
		}

		protected internal virtual void fireViewAdjustEvent()
		{
			fireViewEvent(new ViewEvent(this, ViewEvent.PAGE_ADJUST));
		}

		protected internal virtual void fireViewResizeEvent()
		{
			fireViewEvent(new ViewEvent(this, ViewEvent.PAGE_RESIZE));
		}

		protected internal virtual PageDevice createPageDevice()
		{
			return new DefaultPageDevice();
		}

		public virtual PagePane createPage()
		{
			PagePane pagePane = new PagePane();
			float width, height;
			if (currentPageOrientation == 0)
			{
				width = currentPageWidth;
				height = currentPageHeight;
			}
			else
			{
				width = currentPageHeight;
				height = currentPageWidth;
			}
			pagePane.updatePageSize(width, height, currentPageScale);
			add(pagePane, null);
			pages.Add(pagePane);
			doLayout();
			return pagePane;
		}

		public virtual void addPage(Drawable content)
		{
			PagePane pagePane = CurrentPage;
			PageCanvas canvas = pagePane.PageCanvas;
			if (canvas.Content != null)
			{
				pagePane = createPage();
				canvas = pagePane.PageCanvas;
			}
			canvas.setContent(content, Scale);
			fireViewResizeEvent();
		}

		public virtual void removePage(PagePane pagePane)
		{
			pages.Remove(pagePane);
			remove(pagePane);
			if (PageCount <= 0)
			{
				createPage();
			}
			fireViewResizeEvent();
		}

		public virtual void removeAllPages()
		{
			System.Collections.IEnumerator e = pages.elements();
			while (e.MoveNext())
			{
				remove((PagePane) e.Current);
			}
			pages.Clear();
			currentPageIndex = 0;
			createPage();
			fireViewResizeEvent();
		}

		protected internal virtual void resizePage(PageDevice device)
		{
			currentPageWidth = device.PageWidth;
			currentPageHeight = device.PageHeight;
			PagePane pagePane = CurrentPage;
			currentPageScale = device.Scale;
			float width, height;
			if (currentPageOrientation == 0)
			{
				width = currentPageWidth;
				height = currentPageHeight;
			}
			else
			{
				width = currentPageHeight;
				height = currentPageWidth;
			}
			pagePane.updatePageSize(width, height, currentPageScale);
			doLayout();
			fireViewResizeEvent();
		}

		public virtual void saveProfile(Profile profile)
		{
			profile.setFloat("page.width", currentPageWidth);
			profile.setFloat("page.height", currentPageHeight);
			profile.setFloat("page.scale", currentPageScale);
			profile.setInteger("page.orientation", currentPageOrientation);
		}

		public virtual void restoreProfile(Profile profile)
		{
			currentPageWidth = profile.getFloat("page.width", DEFAULT_PAGE_WIDTH);
			currentPageHeight = profile.getFloat("page.height", DEFAULT_PAGE_HEIGHT);
			currentPageScale = profile.getFloat("page.scale", DEFAULT_PAGE_SCALE);
			currentPageOrientation = profile.getInteger("page.orientation", 0);
		}

		public virtual Console Console
		{
			get
			{
				return console;
			}
		}

		public virtual void actionPerformed(ActionEvent evt)
		{
			exec(evt.ActionCommand);
		}

		public virtual void exec(string code)
		{
			consoleAdapter.@out.println(code);
		}

		public virtual void run(File file)
		{
			try
			{
				string path = file.Path.replace('\\', '/');
				exec("(" + path + ") statusdict /jobserver get exec");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void save(java.io.OutputStream stream, String format) throws java.io.IOException
		public virtual void save(Stream stream, string format)
		{
			PagePane pagePane = CurrentPage;
			PageCanvas canvas = pagePane.PageCanvas;
			canvas.save(stream, format);
		}

		public virtual void pageDeviceChanged(PageEvent evt)
		{
			PageDevice dev = evt.PageDevice;
			switch (evt.Type)
			{
			case PageEvent.BEGINJOB:
				Cursor = new Cursor(Cursor.WAIT_CURSOR);
				firstPage = true;
				break;
			case PageEvent.ENDJOB:
				Cursor = new Cursor(Cursor.DEFAULT_CURSOR);
				break;
			case PageEvent.SHOWPAGE:
			case PageEvent.COPYPAGE:
				addPage(dev.Content);
				if (firstPage)
				{
					showCurrentPage();
					firstPage = false;
				}
				break;
			case PageEvent.RESIZE:
				resizePage(dev);
				 break;
			case PageEvent.ERROR:
				console.Visible = true;
				break;
			}
		}

		public virtual void pageChange(NavigationEvent evt)
		{
			PageIndex = evt.PageIndex;
		}

		public virtual int NumberOfPages
		{
			get
			{
				return PageCount;
			}
		}

		public virtual PageFormat getPageFormat(int pageIndex)
		{
			return pageFormat;
		}

		public virtual Printable getPrintable(int pageIndex)
		{
			return this;
		}

		public virtual void printSetup()
		{
			PrinterJob job = PrinterJob.PrinterJob;
			pageFormat = job.pageDialog(pageFormat);
		}

		public virtual void print()
		{
			PrinterJob job = PrinterJob.PrinterJob;
			job.setPrintable(this, pageFormat);
	//		job.setPageable(this);
			if (job.printDialog())
			{
				try
				{
					job.print();
				}
				catch (PrinterException ex)
				{
					Console.WriteLine(ex.ToString());
					Console.Write(ex.StackTrace);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int print(Graphics g, PageFormat format, int pageIndex) throws PrinterException
		public virtual int print(Graphics g, PageFormat format, int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= PageCount)
			{
				return NO_SUCH_PAGE;
			}
			PagePane pagePane = (PagePane) pages[pageIndex];
			PageCanvas canvas = pagePane.PageCanvas;
			canvas.print(g);
			return PAGE_EXISTS;
		}

		public virtual void updatePages(Rectangle viewBounds)
		{
			int count = 1;
			currentPageIndex = 0;
			bool active = false;
			System.Collections.IEnumerator e = pages.elements();
			while (e.MoveNext())
			{
				PagePane pagePane = (PagePane) e.Current;
				pagePane.ToolTipText = "Page " + count++;
				active |= pagePane.updatePage(viewBounds);
				if (!active)
				{
					currentPageIndex++;
				}
			}
			int n = PageCount;
			if (currentPageIndex >= n)
			{
				currentPageIndex = n - 1;
			}
		}

		public virtual void interrupt()
		{
			interpreter.interrupt(true);
		}

		public virtual float Scale
		{
			set
			{
				device.Scale = value;
				System.Collections.IEnumerator e = pages.elements();
				while (e.MoveNext())
				{
					PagePane pagePane = (PagePane) e.Current;
					pagePane.updatePageScale(value);
				}
				currentPageScale = value;
				validate();
			}
			get
			{
				return device.Scale;
			}
		}


		public virtual int Orientation
		{
			set
			{
				if (currentPageOrientation == value)
				{
					return;
				}
				exec("<</Orientation " + value + ">> setpagedevice");
				currentPageOrientation = value;
				System.Collections.IEnumerator e = pages.elements();
				while (e.MoveNext())
				{
					PagePane pagePane = (PagePane) e.Current;
					float width = pagePane.PageWidth;
					float height = pagePane.PageHeight;
					pagePane.updatePageSize(height, width, currentPageScale);
				}
				doLayout();
				fireViewResizeEvent();
			}
		}

		public virtual int PageCount
		{
			get
			{
				return pages.Count;
			}
		}

		public virtual int PageIndex
		{
			get
			{
				return currentPageIndex;
			}
			set
			{
				int n = PageCount;
				if (value < 0 || value >= n)
				{
					throw new System.ArgumentException("no such page: " + value);
				}
				currentPageIndex = value;
				fireViewChangeEvent();
			}
		}

		private PagePane CurrentPage
		{
			get
			{
				return (PagePane) pages[currentPageIndex];
			}
		}

		public virtual Point PageOffset
		{
			get
			{
				int i, x = 0, y = 0, vgap = layout.GapV;
				for (i = 0; i < currentPageIndex; i++)
				{
					PagePane pane = (PagePane) pages[i];
					Dimension d = pane.Size;
					y += d.height + vgap;
				}
				return new Point(x, y);
			}
		}

		public virtual void showCurrentPage()
		{
			if (currentPageIndex > 0)
			{
				PageIndex = currentPageIndex;
			}
		}

		public virtual void showFirstPage()
		{
			if (currentPageIndex > 0)
			{
				PageIndex = 0;
			}
		}

		public virtual void showLastPage()
		{
			int n = PageCount - 1;
			if (n >= 0)
			{
				PageIndex = n;
			}
		}

		public virtual void showNextPage()
		{
			int pageno = currentPageIndex + 1;
			if (pageno < PageCount)
			{
				PageIndex = pageno;
			}
		}

		public virtual void showPreviousPage()
		{
			if (currentPageIndex > 0)
			{
				PageIndex = currentPageIndex - 1;
			}
		}


		public virtual void deleteCurrentPage()
		{
			int n = PageCount;
			if (n > 0)
			{
				int pageno = currentPageIndex;
				if (0 < currentPageIndex && currentPageIndex < n)
				{
					currentPageIndex--;
				}
				removePage((PagePane) pages[pageno]);
			}
		}

		public virtual void deleteAllPages()
		{
			if (PageCount > 0)
			{
				removeAllPages();
			}
		}

		public virtual string Version
		{
			get
			{
				return interpreter.load("version").ToString();
			}
		}

	}

}