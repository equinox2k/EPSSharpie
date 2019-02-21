using System;
using System.Collections.Generic;

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

	using NavigationEvent = com.softhub.ts.@event.NavigationEvent;
	using NavigationListener = com.softhub.ts.@event.NavigationListener;
	using ViewEvent = com.softhub.ts.@event.ViewEvent;
	using ViewEventListener = com.softhub.ts.@event.ViewEventListener;

	public class NavigationPane : JPanel, ViewEventListener
	{
		private BorderLayout navigationPaneLayout = new BorderLayout(3, 0);
		private BorderLayout centerPaneLayout = new BorderLayout();
		private JLabel label = new JLabel();
		private ImageIcon leftImage;
		private ImageIcon rightImage;
		private JComboBox comboBox = new JComboBox();
		private List<object> listeners = new List<object>();
		private JPanel centerPane = new JPanel();
		private JButton leftPane = new JButton();
		private JButton rightPane = new JButton();
		private bool actionLock;

		public NavigationPane()
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
			label.Font = new java.awt.Font("Dialog", 0, 10);
			label.Border = BorderFactory.createEtchedBorder();
			label.MaximumSize = new Dimension(80, 16);
			label.MinimumSize = new Dimension(40, 16);
			label.PreferredSize = new Dimension(50, 16);
			label.HorizontalAlignment = SwingConstants.CENTER;
			label.Text = "Page:";
			this.Layout = navigationPaneLayout;
			this.MinimumSize = new Dimension(76, 16);
			this.PreferredSize = new Dimension(100, 16);
			comboBox.Font = new java.awt.Font("SansSerif", 0, 10);
			comboBox.MaximumSize = new Dimension(32767, 16);
			comboBox.MinimumSize = new Dimension(100, 16);
			comboBox.PreferredSize = new Dimension(155, 16);
			comboBox.ActionCommand = "pageNumberChanged";
			comboBox.MaximumRowCount = 6;
			comboBox.addActionListener(new ActionListenerAnonymousInnerClass(this));
			centerPane.Layout = centerPaneLayout;
			leftPane.Border = BorderFactory.createEtchedBorder();
			leftPane.PreferredSize = new Dimension(16, 16);
			leftPane.FocusPainted = false;
			leftPane.addMouseListener(new MouseAdapterAnonymousInnerClass(this));
			rightPane.Border = BorderFactory.createEtchedBorder();
			rightPane.PreferredSize = new Dimension(16, 16);
			rightPane.FocusPainted = false;
			rightPane.addMouseListener(new MouseAdapterAnonymousInnerClass2(this));
			centerPane.add(label, BorderLayout.WEST);
			centerPane.add(comboBox, BorderLayout.CENTER);
			this.add(centerPane, BorderLayout.CENTER);
			this.add(leftPane, BorderLayout.WEST);
			this.add(rightPane, BorderLayout.EAST);
			leftImage = new ImageIcon(typeof(com.softhub.ts.ViewFrame).getResource("left.gif"));
			rightImage = new ImageIcon(typeof(com.softhub.ts.ViewFrame).getResource("right.gif"));
			leftPane.Icon = leftImage;
			leftPane.ToolTipText = "Previous";
			rightPane.Icon = rightImage;
			rightPane.ToolTipText = "Next";
		}

		private class ActionListenerAnonymousInnerClass : java.awt.@event.ActionListener
		{
			private readonly NavigationPane outerInstance;

			public ActionListenerAnonymousInnerClass(NavigationPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.comboBoxAction(e);
			}
		}

		private class MouseAdapterAnonymousInnerClass : java.awt.@event.MouseAdapter
		{
			private readonly NavigationPane outerInstance;

			public MouseAdapterAnonymousInnerClass(NavigationPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void mousePressed(MouseEvent e)
			{
				outerInstance.leftPaneMousePressed(e);
			}
		}

		private class MouseAdapterAnonymousInnerClass2 : java.awt.@event.MouseAdapter
		{
			private readonly NavigationPane outerInstance;

			public MouseAdapterAnonymousInnerClass2(NavigationPane outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void mousePressed(MouseEvent e)
			{
				outerInstance.rightPaneMousePressed(e);
			}
		}

		public virtual void addNavigationListener(NavigationListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeNavigationListener(NavigationListener listener)
		{
			listeners.Remove(listener);
		}

		protected internal virtual void fireNavigationEvent(int pageIndex)
		{
			NavigationEvent evt = new NavigationEvent(this, pageIndex);
			System.Collections.IEnumerator e = listeners.elements();
			while (e.MoveNext())
			{
				NavigationListener listener = (NavigationListener) e.Current;
				listener.pageChange(evt);
			}
		}

		public virtual void viewChanged(ViewEvent evt)
		{
			try
			{
				actionLock = true;
				switch (evt.EventType)
				{
				case ViewEvent.PAGE_CHANGE:
				case ViewEvent.PAGE_ADJUST:
					pageChanged((Viewable) evt.Source);
					break;
				}
			}
			finally
			{
				actionLock = false;
			}
		}

		private void comboBoxAction(ActionEvent evt)
		{
			if (!actionLock)
			{
				JComboBox source = (JComboBox) evt.Source;
				int pageIndex = source.SelectedIndex;
				if (pageIndex >= 0)
				{
					fireNavigationEvent(pageIndex);
				}
			}
		}

		private void pageChanged(Viewable page)
		{
			int i, n = page.PageCount;
			if (comboBox.ItemCount != n)
			{
				comboBox.removeAllItems();
				for (i = 1; i <= n; i++)
				{
					comboBox.addItem("" + i);
				}
			}
			int selIndex = page.PageIndex;
			if (0 <= selIndex && selIndex < n)
			{
				comboBox.SelectedIndex = selIndex;
			}
		}

		private void leftPaneMousePressed(MouseEvent evt)
		{
			int pageIndex = comboBox.SelectedIndex;
			int pageCount = comboBox.ItemCount;
			if (0 < pageIndex && pageIndex < pageCount)
			{
				fireNavigationEvent(pageIndex - 1);
			}
		}

		private void rightPaneMousePressed(MouseEvent evt)
		{
			int pageIndex = comboBox.SelectedIndex;
			int pageCount = comboBox.ItemCount;
			if (0 <= pageIndex && pageIndex < pageCount - 1)
			{
				fireNavigationEvent(pageIndex + 1);
			}
		}

	}

}