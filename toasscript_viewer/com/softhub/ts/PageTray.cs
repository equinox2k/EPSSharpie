using System;

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



	using TrayControlEvent = com.softhub.ts.@event.TrayControlEvent;
	using TrayControlListener = com.softhub.ts.@event.TrayControlListener;
	using ViewEvent = com.softhub.ts.@event.ViewEvent;
	using ViewEventListener = com.softhub.ts.@event.ViewEventListener;

	public class PageTray : JPanel, ViewEventListener, TrayConstants, TrayControlListener, AdjustmentListener
	{
		private BorderLayout pageTrayLayout = new BorderLayout();
		private PageLayout portPaneLayout = new PageLayout();
		private FlowLayout trayControlLayout = new FlowLayout(FlowLayout.LEFT, 2, 2);
		private TrayScrollLayout scrollLayout = new TrayScrollLayout();
		private JScrollPane scrollPane = new JScrollPane();
		private PostScriptPane postScriptPane = new PostScriptPane();
		private NavigationPane navigationPane = new NavigationPane();
		private TrayControl trayControlPane = new TrayControl();
		private JPanel controlPane = new JPanel();
		private JViewport viewport = new JViewport();
		private JPanel portPane = new JPanel();
		private JPanel separatorPane = new JPanel();

		public PageTray()
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
			this.Layout = pageTrayLayout;
			controlPane.Layout = trayControlLayout;
			scrollPane.Layout = scrollLayout;
			portPane.Layout = portPaneLayout;
			portPane.PreferredSize = new Dimension(80, 80);
			scrollPane.HorizontalScrollBarPolicy = JScrollPane.HORIZONTAL_SCROLLBAR_ALWAYS;
			//scrollPane.setViewportBorder(BorderFactory.createEtchedBorder());
			navigationPane.PreferredSize = new Dimension(125, 16);
			portPane.add(postScriptPane, PageLayout.CENTER);
			viewport.add(portPane, BorderLayout.CENTER);
			//controlPane.setBorder(BorderFactory.createBevelBorder(BevelBorder.LOWERED));
			controlPane.PreferredSize = new Dimension(10, 19);
			trayControlPane.PreferredSize = new Dimension(110, 16);
			separatorPane.PreferredSize = new Dimension(3, 16);
			controlPane.add(trayControlPane, null);
			controlPane.add(separatorPane, null);
			controlPane.add(navigationPane, null);
			scrollPane.add(controlPane, TrayConstants_Fields.CONTROL_BAR);
			scrollPane.VerticalScrollBar.UnitIncrement = 10;
			scrollPane.Viewport = viewport;
			scrollLayout.syncWithScrollPane(scrollPane);
			scrollPane.HorizontalScrollBar.addAdjustmentListener(this);
			scrollPane.VerticalScrollBar.addAdjustmentListener(this);
			this.add(scrollPane, BorderLayout.CENTER);
			trayControlPane.addTrayControlListener(this);
			navigationPane.addNavigationListener(postScriptPane);
			postScriptPane.addViewEventListener(trayControlPane);
			postScriptPane.addViewEventListener(navigationPane);
			postScriptPane.addViewEventListener(this);
		}

		public virtual void init()
		{
			postScriptPane.createPage();
			postScriptPane.init();
		}

		public virtual void doLayout()
		{
			Dimension d = postScriptPane.Size;
			int hgap = portPaneLayout.GapH * 2;
			int vgap = portPaneLayout.GapV * 2;
			Dimension t = new Dimension(d.width + hgap, d.height + vgap);
			portPane.PreferredSize = t;
			base.doLayout();
		}

		public virtual void saveProfile(Profile profile)
		{
			postScriptPane.saveProfile(profile);
		}

		public virtual void restoreProfile(Profile profile)
		{
			postScriptPane.restoreProfile(profile);
		}

		public virtual PostScriptPane PostScriptPane
		{
			get
			{
				return postScriptPane;
			}
		}

		public virtual void adjustmentValueChanged(AdjustmentEvent evt)
		{
			postScriptPane.updatePages(viewport.ViewRect);
			postScriptPane.fireViewAdjustEvent();
		}

		public virtual void trayChange(TrayControlEvent evt)
		{
			postScriptPane.Scale = evt.Scale;
		}

		public virtual void viewChanged(ViewEvent evt)
		{
			Viewable page = (Viewable) evt.Source;
			switch (evt.EventType)
			{
			case ViewEvent.PAGE_RESIZE:
				resizePage(page);
				break;
			case ViewEvent.PAGE_CHANGE:
				changePage(page);
				break;
			}
		}

		private void resizePage(Viewable page)
		{
			doLayout();
			scrollLayout.syncWithScrollPane(scrollPane);
			scrollPane.Viewport = viewport;
			page.updatePages(viewport.ViewRect);
		}

		private void changePage(Viewable page)
		{
			viewport.doLayout();
			Point pt = page.PageOffset;
			viewport.ViewPosition = pt;
			page.updatePages(viewport.ViewRect);
		}

	}

}