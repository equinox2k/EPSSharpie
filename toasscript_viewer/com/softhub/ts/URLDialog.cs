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


	public class URLDialog : JDialog
	{

		private List<object> listeners = new List<object>();
		private JPanel controlPane = new JPanel();
		private FlowLayout flowLayout1 = new FlowLayout();
		private JButton cancelButton = new JButton();
		private JButton okButton = new JButton();
		private JTextField textField = new JTextField();
		private JLabel label = new JLabel();
		private FlowLayout flowLayout2 = new FlowLayout();
		private JPanel editPane = new JPanel();
		private JPanel messagePane = new JPanel();
		private JLabel messageLabel = new JLabel();
		private BorderLayout borderLayout1 = new BorderLayout();


		public URLDialog(Frame frame, string title, bool modal) : base(frame, title, modal)
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

		public URLDialog() : this(null, "", false)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void jbInit() throws Exception
		private void jbInit()
		{
			controlPane.PreferredSize = new Dimension(10, 40);
			controlPane.Layout = flowLayout1;
			cancelButton.PreferredSize = new Dimension(75, 27);
			cancelButton.Text = "Cancel";
			cancelButton.addActionListener(new ActionListenerAnonymousInnerClass(this));
			okButton.PreferredSize = new Dimension(75, 27);
			okButton.Text = "OK";
			okButton.addActionListener(new ActionListenerAnonymousInnerClass2(this));
			textField.PreferredSize = new Dimension(280, 21);
			label.Text = "URL:";
			flowLayout2.Vgap = 15;
			editPane.Layout = flowLayout2;
			this.addKeyListener(new KeyAdapterAnonymousInnerClass(this));
			messageLabel.PreferredSize = new Dimension(100, 17);
			messagePane.Layout = borderLayout1;
			this.ContentPane.add(controlPane, BorderLayout.SOUTH);
			controlPane.add(okButton, null);
			controlPane.add(cancelButton, null);
			this.ContentPane.add(editPane, BorderLayout.CENTER);
			editPane.add(label, null);
			editPane.add(textField, null);
			this.ContentPane.add(messagePane, BorderLayout.NORTH);
			messagePane.add(messageLabel, BorderLayout.CENTER);
		}

		private class ActionListenerAnonymousInnerClass : java.awt.@event.ActionListener
		{
			private readonly URLDialog outerInstance;

			public ActionListenerAnonymousInnerClass(URLDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.cancelButtonAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass2 : java.awt.@event.ActionListener
		{
			private readonly URLDialog outerInstance;

			public ActionListenerAnonymousInnerClass2(URLDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.okButtonAction(e);
			}
		}

		private class KeyAdapterAnonymousInnerClass : java.awt.@event.KeyAdapter
		{
			private readonly URLDialog outerInstance;

			public KeyAdapterAnonymousInnerClass(URLDialog outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void keyTyped(KeyEvent e)
			{
				outerInstance.keyPTypedAction(e);
			}
		}

		public virtual void addActionListener(ActionListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeActionListener(ActionListener listener)
		{
			listeners.Remove(listener);
		}

		protected internal virtual void fireActionEvent(ActionEvent evt)
		{
			System.Collections.IEnumerator e = listeners.elements();
			while (e.MoveNext())
			{
				ActionListener listener = (ActionListener) e.Current;
				listener.actionPerformed(evt);
			}
		}

		protected internal virtual void fireAction(string cmd)
		{
			fireActionEvent(new ActionEvent(this, ActionEvent.ACTION_PERFORMED, cmd)
		   );
		}

		protected internal virtual void onCancel()
		{
			Visible = false;
		}

		protected internal virtual void onError(string msg)
		{
			messageLabel.Text = msg;
		}

		protected internal virtual void onOK()
		{
			string text = textField.Text;
			try
			{
				URL url = new URL(text);
				Visible = false;
				fireAction("(" + text + ") statusdict /jobserver get exec");
			}
			catch (MalformedURLException)
			{
				onError("Invalid URL: " + text);
			}
		}

		private void cancelButtonAction(ActionEvent evt)
		{
			onCancel();
		}

		private void okButtonAction(ActionEvent evt)
		{
			onOK();
		}

		internal virtual void keyPTypedAction(KeyEvent evt)
		{
			if (evt.KeyChar == '\n')
			{
				onOK();
			}
			messageLabel.Text = "";
		}

	}

}