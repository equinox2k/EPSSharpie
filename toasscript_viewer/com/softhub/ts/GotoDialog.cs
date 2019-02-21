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

	public class GotoDialog : JDialog, ViewEventListener
	{

		private JPanel controlPane = new JPanel();
		private FlowLayout flowLayout1 = new FlowLayout();
		private JButton okButton = new JButton();
		private JButton cancelButton = new JButton();
		private JTextField textField = new JTextField();
		private JLabel label = new JLabel();
		private FlowLayout flowLayout2 = new FlowLayout();
		private JPanel editPane = new JPanel();
		private List<object> listeners = new List<object>();

		public GotoDialog(Frame frame, string title, bool modal) : base(frame, title, modal)
		{
			try
			{
				jbInit();
				pack();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
		}

		public GotoDialog() : this(null, "", false)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void jbInit() throws Exception
		private void jbInit()
		{
			editPane.Layout = flowLayout2;
			flowLayout2.Vgap = 20;
			label.Text = "Page:";
			textField.PreferredSize = new Dimension(40, 21);
			controlPane.PreferredSize = new Dimension(180, 40);
			controlPane.Layout = flowLayout1;
			okButton.MaximumSize = new Dimension(75, 27);
			okButton.MinimumSize = new Dimension(75, 27);
			okButton.PreferredSize = new Dimension(75, 27);
			okButton.Text = "OK";
			okButton.addActionListener(new ActionListenerAnonymousInnerClass(this));
			cancelButton.MaximumSize = new Dimension(75, 27);
			cancelButton.MinimumSize = new Dimension(75, 27);
			cancelButton.Text = "Cancel";
			cancelButton.addActionListener(new ActionListenerAnonymousInnerClass2(this));
			this.Resizable = false;
			editPane.PreferredSize = new Dimension(180, 60);
			this.ContentPane.add(controlPane, BorderLayout.SOUTH);
			controlPane.add(okButton, null);
			controlPane.add(cancelButton, null);
			this.ContentPane.add(editPane, BorderLayout.NORTH);
			editPane.add(label, null);
			editPane.add(textField, null);
		}

		private class ActionListenerAnonymousInnerClass : java.awt.@event.ActionListener
		{
			private readonly GotoDialog outerInstance;

			public ActionListenerAnonymousInnerClass(GotoDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.okButtonAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass2 : java.awt.@event.ActionListener
		{
			private readonly GotoDialog outerInstance;

			public ActionListenerAnonymousInnerClass2(GotoDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.cancelButtonAction(e);
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
			switch (evt.EventType)
			{
			case ViewEvent.PAGE_ADJUST:
			case ViewEvent.PAGE_CHANGE:
				Viewable page = (Viewable) evt.Source;
				PageNumber = page.PageIndex + 1;
				break;
			}
		}

		public virtual int PageNumber
		{
			set
			{
				textField.Text = value.ToString();
			}
		}

		private void cancelButtonAction(ActionEvent evt)
		{
			Visible = false;
		}

		private void okButtonAction(ActionEvent evt)
		{
			try
			{
				int index = Convert.ToInt32(textField.Text);
				fireNavigationEvent(index - 1);
			}
			finally
			{
				Visible = false;
			}
		}

	}

}