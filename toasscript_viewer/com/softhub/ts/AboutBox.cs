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



	public class AboutBox : JDialog, ActionListener
	{

		private JPanel panel1 = new JPanel();
		private JPanel panel2 = new JPanel();
		private JPanel insetsPanel1 = new JPanel();
		private JPanel insetsPanel2 = new JPanel();
		private JPanel insetsPanel3 = new JPanel();
		private JButton okButton = new JButton();
		private JLabel imageLabel = new JLabel();
		private JLabel label1 = new JLabel();
		private JLabel label2 = new JLabel();
		private JLabel label3 = new JLabel();
		private JLabel label4 = new JLabel();
		private BorderLayout borderLayout1 = new BorderLayout();
		private BorderLayout borderLayout2 = new BorderLayout();
		private FlowLayout flowLayout1 = new FlowLayout();
		private GridLayout gridLayout1 = new GridLayout();
		private string product = "ToastScript";
		private string version = "1.0";
		private string copyright = "Copyright (c) by Christian Lehner";
		private string comments = "Java PostScript Interpreter";
		private JButton printButton = new JButton();
		private PostScriptPane postScriptPane;

		public AboutBox(Frame parent, PostScriptPane postScriptPane) : base(parent)
		{
			this.postScriptPane = postScriptPane;
			enableEvents(AWTEvent.WINDOW_EVENT_MASK);
			try
			{
				jbInit();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
			pack();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void jbInit() throws Exception
		private void jbInit()
		{
			//imageLabel.setIcon(new ImageIcon(ViewFrame_AboutBox.class.getResource("[Your Image]")));
			this.Title = "About";
			version = postScriptPane.Version;
			Resizable = false;
			panel1.Layout = borderLayout1;
			panel2.Layout = borderLayout2;
			insetsPanel1.setLayout(flowLayout1);
			insetsPanel2.setLayout(flowLayout1);
			insetsPanel2.setBorder(BorderFactory.createEmptyBorder(10, 10, 10, 10));
			gridLayout1.Rows = 4;
			gridLayout1.Columns = 1;
			label1.Text = product;
			label2.Text = version;
			label3.Text = copyright;
			label4.Text = comments;
			insetsPanel3.setLayout(gridLayout1);
			insetsPanel3.setBorder(BorderFactory.createEmptyBorder(10, 60, 10, 10));
			okButton.Text = "Ok";
			okButton.addActionListener(this);
			printButton.Text = "Print";
			printButton.addActionListener(new ActionListenerAnonymousInnerClass(this));
			insetsPanel2.add(imageLabel, null);
			panel2.add(insetsPanel3, BorderLayout.CENTER);
			insetsPanel3.add(label1, null);
			insetsPanel3.add(label2, null);
			insetsPanel3.add(label3, null);
			insetsPanel3.add(label4, null);
			panel2.add(insetsPanel2, BorderLayout.WEST);
			this.ContentPane.add(panel1, null);
			insetsPanel1.add(okButton, null);
			insetsPanel1.add(printButton, null);
			panel1.add(insetsPanel1, BorderLayout.SOUTH);
			panel1.add(panel2, BorderLayout.NORTH);
		}

		private class ActionListenerAnonymousInnerClass : ActionListener
		{
			private readonly AboutBox outerInstance;

			public ActionListenerAnonymousInnerClass(AboutBox outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.printButtonAction(e);
			}
		}

		protected internal virtual void processWindowEvent(WindowEvent e)
		{
			if (e.ID == WindowEvent.WINDOW_CLOSING)
			{
				cancel();
			}
			base.processWindowEvent(e);
		}

		internal virtual void cancel()
		{
			dispose();
		}

		public virtual void actionPerformed(ActionEvent evt)
		{
			if (evt.Source == okButton)
			{
				cancel();
			}
		}

		internal virtual void printButtonAction(ActionEvent evt)
		{
			if (evt.Source == printButton)
			{
				postScriptPane.exec("statusdict /about get exec");
				cancel();
			}
		}

	}

}