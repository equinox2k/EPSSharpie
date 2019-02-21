using System;
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

	using Console = com.softhub.ps.util.Console;

	public class ViewFrame : JFrame
	{

		private JPanel contentPane;
		private JMenuBar menuBar = new JMenuBar();
		private JMenu fileMenu = new JMenu();
		private JMenuItem exitMenuItem = new JMenuItem();
		private JMenu helpMenu = new JMenu();
		private JMenuItem aboutMenuItem = new JMenuItem();
		private JToolBar toolBar = new JToolBar();
		private JButton fileOpenButton = new JButton();
		private JButton fileSaveButton = new JButton();
		private JButton printButton = new JButton();
		private JButton deleteButton = new JButton();
		private JButton prevButton = new JButton();
		private JButton nextButton = new JButton();
		private JButton showPageButton = new JButton();
		private JButton helpButton = new JButton();
		private ImageIcon openImage;
		private ImageIcon saveImage;
		private ImageIcon printImage;
		private ImageIcon deleteImage;
		private ImageIcon prevImage;
		private ImageIcon nextImage;
		private ImageIcon showPageImage;
		private ImageIcon helpImage;
		private JLabel statusBar = new JLabel();
		private PageTray pageTray = new PageTray();
		private URLDialog urlDialog;
		private GotoDialog gotoDialog;
		private BorderLayout contentLayout = new BorderLayout();
		private File propertiesFile;
		private Profile profile;
		private JMenuItem openMenuItem = new JMenuItem();
		private JMenuItem openURLMenuItem = new JMenuItem();
		private JMenu viewMenu = new JMenu();
		private JMenuItem gotoPageMenuItem = new JMenuItem();
		private JMenuItem firstPageMenuItem = new JMenuItem();
		private JMenuItem lastPageMenuItem = new JMenuItem();
		private JMenuItem previousPageMenuItem = new JMenuItem();
		private JMenuItem nextPageMenuItem = new JMenuItem();
		private JMenu controlMenu = new JMenu();
		private JMenuItem consoleMenuItem = new JMenuItem();
		private JMenuItem interruptMenuItem = new JMenuItem();
		private JMenuItem showPageMenuItem = new JMenuItem();
		private JMenuItem deleteAllPagesMenuItem = new JMenuItem();
		private JMenuItem deletePageMenuItem = new JMenuItem();
		private JFileChooser fileChooser = new JFileChooser();
		private JMenuItem saveAsMenuItem = new JMenuItem();
		private JMenuItem printMenuItem = new JMenuItem();
		private JMenu formatMenu = new JMenu();
		private JMenuItem formatLetterItem = new JMenuItem();
		private JMenuItem formatLegalItem = new JMenuItem();
		private JMenuItem formatA3Item = new JMenuItem();
		private JMenuItem formatA4Item = new JMenuItem();
		private JMenuItem formatA5Item = new JMenuItem();
		private JMenuItem formatB3Item = new JMenuItem();
		private JMenuItem formatB4Item = new JMenuItem();
		private JMenuItem formatB5Item = new JMenuItem();
		private JMenuItem printSetupItem = new JMenuItem();
		private JMenu orientationMenu = new JMenu();
		private JMenuItem normalOrientationItem = new JMenuItem();
		private JMenuItem landscapeOrientationItem = new JMenuItem();
		private Console console;

		public ViewFrame()
		{
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
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void jbInit() throws Exception
		private void jbInit()
		{
			try
			{
				restoreProfile();
			}
			catch (Exception)
			{
			}
			openImage = new ImageIcon(typeof(ViewFrame).getResource("open.gif"));
			saveImage = new ImageIcon(typeof(ViewFrame).getResource("save.gif"));
			printImage = new ImageIcon(typeof(ViewFrame).getResource("print.gif"));
			deleteImage = new ImageIcon(typeof(ViewFrame).getResource("delete.gif"));
			prevImage = new ImageIcon(typeof(ViewFrame).getResource("leftarrow.gif"));
			nextImage = new ImageIcon(typeof(ViewFrame).getResource("rightarrow.gif"));
			showPageImage = new ImageIcon(typeof(ViewFrame).getResource("showpage.gif"));
			helpImage = new ImageIcon(typeof(ViewFrame).getResource("help.gif"));
			//setIconImage(Toolkit.getDefaultToolkit().createImage(ViewFrame.class.getResource("[Your Icon]")));
			contentPane = (JPanel) this.ContentPane;
			contentPane.Layout = contentLayout;
			this.Title = "ToastScript";
			statusBar.Text = " ";
			fileMenu.Text = "File";
			exitMenuItem.Text = "Exit";
			exitMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(81, java.awt.@event.KeyEvent.CTRL_MASK, false);
			exitMenuItem.addActionListener(new ActionListenerAnonymousInnerClass(this));
			helpMenu.Text = "Help";
			aboutMenuItem.Text = "About";
			aboutMenuItem.addActionListener(new ActionListenerAnonymousInnerClass2(this));
			fileOpenButton.Icon = openImage;
			fileOpenButton.Margin = new Insets(2, 2, 2, 2);
			fileOpenButton.addActionListener(new ActionListenerAnonymousInnerClass(this));
			fileOpenButton.MaximumSize = new Dimension(28, 28);
			fileOpenButton.MinimumSize = new Dimension(28, 28);
			fileOpenButton.PreferredSize = new Dimension(28, 28);
			fileOpenButton.ToolTipText = "Open File";
			fileOpenButton.BorderPainted = false;
			fileOpenButton.FocusPainted = false;
			fileSaveButton.Icon = saveImage;
			fileSaveButton.Margin = new Insets(2, 2, 2, 2);
			fileSaveButton.addActionListener(new ActionListenerAnonymousInnerClass2(this));
			fileSaveButton.MaximumSize = new Dimension(28, 28);
			fileSaveButton.MinimumSize = new Dimension(28, 28);
			fileSaveButton.PreferredSize = new Dimension(28, 28);
			fileSaveButton.ToolTipText = "Close File";
			fileSaveButton.BorderPainted = false;
			fileSaveButton.FocusPainted = false;
			prevButton.Icon = prevImage;
			prevButton.Margin = new Insets(2, 2, 2, 2);
			prevButton.addActionListener(new ActionListenerAnonymousInnerClass3(this));
			prevButton.MaximumSize = new Dimension(28, 28);
			prevButton.MinimumSize = new Dimension(28, 28);
			prevButton.PreferredSize = new Dimension(28, 28);
			prevButton.ToolTipText = "Previous";
			prevButton.BorderPainted = false;
			prevButton.FocusPainted = false;
			nextButton.Icon = nextImage;
			nextButton.Margin = new Insets(2, 2, 2, 2);
			nextButton.addActionListener(new ActionListenerAnonymousInnerClass4(this));
			nextButton.MaximumSize = new Dimension(28, 28);
			nextButton.MinimumSize = new Dimension(28, 28);
			nextButton.PreferredSize = new Dimension(28, 28);
			nextButton.ToolTipText = "Next";
			nextButton.BorderPainted = false;
			nextButton.FocusPainted = false;
			helpButton.Icon = helpImage;
			helpButton.Margin = new Insets(2, 2, 2, 2);
			helpButton.addActionListener(new ActionListenerAnonymousInnerClass5(this));
			helpButton.MaximumSize = new Dimension(28, 28);
			helpButton.MinimumSize = new Dimension(28, 28);
			helpButton.PreferredSize = new Dimension(28, 28);
			helpButton.ToolTipText = "Help";
			helpButton.BorderPainted = false;
			helpButton.FocusPainted = false;
			toolBar.Border = BorderFactory.createEtchedBorder();
			openMenuItem.Text = "Open...";
			openMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(48, java.awt.@event.KeyEvent.CTRL_MASK, false);
			openMenuItem.addActionListener(new ActionListenerAnonymousInnerClass6(this));
			openURLMenuItem.Text = "Open URL...";
			openURLMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(85, java.awt.@event.KeyEvent.CTRL_MASK, false);
			openURLMenuItem.addActionListener(new ActionListenerAnonymousInnerClass7(this));
			viewMenu.Text = "View";
			gotoPageMenuItem.Text = "Goto Page...";
			gotoPageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass8(this));
			firstPageMenuItem.Text = "First Page";
			firstPageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass9(this));
			lastPageMenuItem.Text = "Last Page";
			lastPageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass10(this));
			previousPageMenuItem.Text = "Previous Page";
			previousPageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass11(this));
			nextPageMenuItem.Text = "Next Page";
			nextPageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass12(this));
			controlMenu.Text = "Control";
			consoleMenuItem.Text = "Show Console";
			consoleMenuItem.addActionListener(new ActionListenerAnonymousInnerClass13(this));
			interruptMenuItem.Text = "Interrupt";
			interruptMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(73, java.awt.@event.KeyEvent.CTRL_MASK, false);
			interruptMenuItem.addActionListener(new ActionListenerAnonymousInnerClass14(this));
			showPageMenuItem.Text = "Show Page";
			showPageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass15(this));
			deleteAllPagesMenuItem.Text = "Delete All Pages";
			deleteAllPagesMenuItem.addActionListener(new ActionListenerAnonymousInnerClass16(this));
			deletePageMenuItem.Text = "Delete Page";
			deletePageMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(68, java.awt.@event.KeyEvent.CTRL_MASK, false);
			deletePageMenuItem.addActionListener(new ActionListenerAnonymousInnerClass17(this));
			saveAsMenuItem.Text = "Save As...";
			saveAsMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(83, java.awt.@event.KeyEvent.CTRL_MASK, false);
			saveAsMenuItem.addActionListener(new ActionListenerAnonymousInnerClass18(this));
			pageTray.MinimumSize = new Dimension(50, 50);
			pageTray.PreferredSize = new Dimension(600, 800);
			printMenuItem.Text = "Print...";
			printMenuItem.Accelerator = javax.swing.KeyStroke.getKeyStroke(80, java.awt.@event.KeyEvent.CTRL_MASK, false);
			printMenuItem.addActionListener(new ActionListenerAnonymousInnerClass19(this));
			formatMenu.Text = "Format";
			formatLetterItem.Text = "Letter";
			formatLetterItem.addActionListener(new FormatListener(this, "letter"));
			formatLegalItem.Text = "Legal";
			formatLegalItem.addActionListener(new FormatListener(this, "legal"));
			formatA3Item.Text = "A3";
			formatA3Item.addActionListener(new FormatListener(this, "a3"));
			formatA4Item.Text = "A4";
			formatA4Item.addActionListener(new FormatListener(this, "a4"));
			formatA5Item.Text = "A5";
			formatA5Item.addActionListener(new FormatListener(this, "a5"));
			formatB3Item.Text = "B3";
			formatB3Item.addActionListener(new FormatListener(this, "b3"));
			formatB4Item.Text = "B4";
			formatB4Item.addActionListener(new FormatListener(this, "b4"));
			formatB5Item.Text = "B5";
			formatB5Item.addActionListener(new FormatListener(this, "b5"));
			printSetupItem.Text = "Page Setup...";
			printSetupItem.addActionListener(new ActionListenerAnonymousInnerClass20(this));
			printButton.MaximumSize = new Dimension(28, 28);
			printButton.MinimumSize = new Dimension(28, 28);
			printButton.PreferredSize = new Dimension(28, 28);
			printButton.ToolTipText = "Print";
			printButton.BorderPainted = false;
			printButton.FocusPainted = false;
			printButton.Icon = printImage;
			printButton.Margin = new Insets(2, 2, 2, 2);
			printButton.addActionListener(new ActionListenerAnonymousInnerClass21(this));
			deleteButton.MaximumSize = new Dimension(28, 28);
			deleteButton.MinimumSize = new Dimension(28, 28);
			deleteButton.PreferredSize = new Dimension(28, 28);
			deleteButton.ToolTipText = "Delete All Pages";
			deleteButton.BorderPainted = false;
			deleteButton.FocusPainted = false;
			deleteButton.Icon = deleteImage;
			deleteButton.Margin = new Insets(2, 2, 2, 2);
			deleteButton.addActionListener(new ActionListenerAnonymousInnerClass22(this));
			showPageButton.MaximumSize = new Dimension(28, 28);
			showPageButton.MinimumSize = new Dimension(28, 28);
			showPageButton.PreferredSize = new Dimension(28, 28);
			showPageButton.ToolTipText = "Show Page";
			showPageButton.BorderPainted = false;
			showPageButton.FocusPainted = false;
			showPageButton.Icon = showPageImage;
			showPageButton.Margin = new Insets(2, 2, 2, 2);
			showPageButton.addActionListener(new ActionListenerAnonymousInnerClass23(this));
			orientationMenu.Text = "Orientation";
			normalOrientationItem.Text = "normal";
			normalOrientationItem.addActionListener(new OrientationListener(this, "normal"));
			landscapeOrientationItem.Text = "landscape";
			landscapeOrientationItem.addActionListener(new OrientationListener(this, "landscape"));
			toolBar.add(fileOpenButton);
			toolBar.add(fileSaveButton);
			toolBar.add(printButton);
			toolBar.add(showPageButton);
			toolBar.add(deleteButton);
			toolBar.add(prevButton);
			toolBar.add(nextButton);
			toolBar.add(helpButton);
			fileMenu.add(openMenuItem);
			fileMenu.add(openURLMenuItem);
			fileMenu.addSeparator();
			fileMenu.add(saveAsMenuItem);
			fileMenu.addSeparator();
			fileMenu.add(printSetupItem);
			fileMenu.add(printMenuItem);
			fileMenu.addSeparator();
			fileMenu.add(exitMenuItem);
			helpMenu.add(aboutMenuItem);
			menuBar.add(fileMenu);
			menuBar.add(viewMenu);
			menuBar.add(controlMenu);
			menuBar.add(helpMenu);
			this.JMenuBar = menuBar;
			contentPane.add(toolBar, BorderLayout.NORTH);
			contentPane.add(statusBar, BorderLayout.SOUTH);
			contentPane.add(pageTray, BorderLayout.CENTER);
			viewMenu.add(formatMenu);
			viewMenu.add(orientationMenu);
			viewMenu.addSeparator();
			viewMenu.add(gotoPageMenuItem);
			viewMenu.add(firstPageMenuItem);
			viewMenu.add(lastPageMenuItem);
			viewMenu.addSeparator();
			viewMenu.add(previousPageMenuItem);
			viewMenu.add(nextPageMenuItem);
			controlMenu.add(consoleMenuItem);
			controlMenu.addSeparator();
			controlMenu.add(interruptMenuItem);
			controlMenu.add(showPageMenuItem);
			controlMenu.add(deletePageMenuItem);
			controlMenu.add(deleteAllPagesMenuItem);
			formatMenu.add(formatLetterItem);
			formatMenu.add(formatLegalItem);
			formatMenu.addSeparator();
			formatMenu.add(formatA3Item);
			formatMenu.add(formatA4Item);
			formatMenu.add(formatA5Item);
			formatMenu.addSeparator();
			formatMenu.add(formatB3Item);
			formatMenu.add(formatB4Item);
			formatMenu.add(formatB5Item);
			orientationMenu.add(normalOrientationItem);
			orientationMenu.add(landscapeOrientationItem);
			init();
		}

		private class ActionListenerAnonymousInnerClass : ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.exitMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass2 : ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass2(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.aboutMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.openAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass2 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass2(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.saveAsAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass3 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass3(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.previousPageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass4 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass4(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.nextPageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass5 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass5(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.aboutMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass6 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass6(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.openAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass7 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass7(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.openURLMenuAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass8 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass8(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.gotoPageAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass9 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass9(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.firstPageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass10 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass10(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.lastPageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass11 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass11(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.previousPageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass12 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass12(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.nextPageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass13 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass13(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.showConsoleAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass14 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass14(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.interruptMenuAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass15 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass15(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.showPageAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass16 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass16(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.deleteAllPagesMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass17 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass17(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.deletePageMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass18 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass18(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.saveAsAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass19 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass19(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.printMenuAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass20 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass20(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.printSetupAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass21 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass21(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.printMenuAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass22 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass22(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.deleteAllPagesMenuItemAction(e);
			}
		}

		private class ActionListenerAnonymousInnerClass23 : java.awt.@event.ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ActionListenerAnonymousInnerClass23(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.showPageAction(e);
			}
		}

		public virtual void init()
		{
			pageTray.restoreProfile(profile);
			pageTray.init();
			console = PostScriptPane.Console;
			console.addActionListener(new ConsoleListener(this));
			console.pack();
		}

		private void prepareChooser()
		{
			string fileName = profile.getString("file", null);
			if (!string.ReferenceEquals(fileName, null))
			{
				try
				{
					File file = new File(fileName);
					fileChooser.CurrentDirectory = file;
				}
				catch (Exception)
				{
				}
			}
		}

		internal virtual void openAction(ActionEvent evt)
		{
			prepareChooser();
			fileChooser.DialogTitle = "Open PostScript File";
			if (fileChooser.showOpenDialog(this) == JFileChooser.APPROVE_OPTION)
			{
				File file = fileChooser.SelectedFile;
				profile.setString("file", file.Path);
				PostScriptPane.run(file);
			}
		}

		internal virtual void saveAsAction(ActionEvent evt)
		{
			prepareChooser();
			fileChooser.DialogTitle = "Save As JPEG";
			if (fileChooser.showSaveDialog(this) == JFileChooser.APPROVE_OPTION)
			{
				bool writeImage = true;
				File file = fileChooser.SelectedFile;
				if (file.exists())
				{
					writeImage = alert("Alert", "Overwrite");
				}
				profile.setString("file", file.Path);
				if (writeImage)
				{
					FileStream stream = null;
					try
					{
						stream = new FileStream(file, FileMode.Create, FileAccess.Write);
						PostScriptPane.save(stream, "jpeg");
					}
					catch (IOException ex)
					{
						Console.Error.WriteLine("export failed: " + ex);
					}
					finally
					{
						if (stream != null)
						{
							try
							{
								stream.Close();
							}
							catch (IOException)
							{
							}
						}
					}
				}
			}
		}

		internal virtual void gotoPageAction(ActionEvent evt)
		{
			if (gotoDialog == null)
			{
				gotoDialog = new GotoDialog(this, "Goto Page", false);
				PostScriptPane pane = PostScriptPane;
				pane.addViewEventListener(gotoDialog);
				gotoDialog.addNavigationListener(pane);
				gotoDialog.PageNumber = pane.PageIndex + 1;
			}
			postDialog(gotoDialog);
		}

		internal virtual void openURLMenuAction(ActionEvent evt)
		{
			if (urlDialog == null)
			{
				urlDialog = new URLDialog(this, "URL", false);
				urlDialog.addActionListener(PostScriptPane);
			}
			postDialog(urlDialog);
		}

		internal virtual void firstPageMenuItemAction(ActionEvent evt)
		{
			PostScriptPane.showFirstPage();
		}

		internal virtual void lastPageMenuItemAction(ActionEvent evt)
		{
			PostScriptPane.showLastPage();
		}

		internal virtual void previousPageMenuItemAction(ActionEvent evt)
		{
			PostScriptPane.showPreviousPage();
		}

		internal virtual void nextPageMenuItemAction(ActionEvent evt)
		{
			PostScriptPane.showNextPage();
		}

		internal virtual void deletePageMenuItemAction(ActionEvent evt)
		{
			PostScriptPane.deleteCurrentPage();
		}

		internal virtual void deleteAllPagesMenuItemAction(ActionEvent evt)
		{
			PostScriptPane.deleteAllPages();
		}

		internal virtual void interruptMenuAction(ActionEvent evt)
		{
			PostScriptPane.interrupt();
		}

		internal virtual void showPageAction(ActionEvent evt)
		{
			PostScriptPane.exec("showpage");
		}

		internal virtual void exitMenuItemAction(ActionEvent evt)
		{
			saveProfile();
			Environment.Exit(0);
		}

		internal virtual void printSetupAction(ActionEvent evt)
		{
			PostScriptPane.printSetup();
		}

		internal virtual void printMenuAction(ActionEvent evt)
		{
			PostScriptPane.print();
		}

		internal virtual void showConsoleAction(ActionEvent evt)
		{
			PostScriptPane pane = pageTray.PostScriptPane;
			string cmd = evt.ActionCommand;
			if ("Hide Console".Equals(cmd, StringComparison.OrdinalIgnoreCase))
			{
				console.Visible = false;
			}
			else
			{
				console.Visible = true;
			}
		}

		internal virtual void consoleAction(ActionEvent evt)
		{
			PostScriptPane pane = pageTray.PostScriptPane;
			string cmd = evt.ActionCommand;
			if (Console.HIDE.equalsIgnoreCase(cmd))
			{
				consoleMenuItem.Text = "Show Console";
			}
			else
			{
				consoleMenuItem.Text = "Hide Console";
			}
		}

		internal virtual void aboutMenuItemAction(ActionEvent evt)
		{
			postDialog(new AboutBox(this, PostScriptPane));
		}

		protected internal virtual PostScriptPane PostScriptPane
		{
			get
			{
				return pageTray.PostScriptPane;
			}
		}

		protected internal virtual void processWindowEvent(WindowEvent evt)
		{
			base.processWindowEvent(evt);
			if (evt.ID == WindowEvent.WINDOW_CLOSING)
			{
				exitMenuItemAction(null);
			}
		}

		protected internal virtual void restoreProfile()
		{
			pageTray.MinimumSize = new Dimension(80, 80);
			pageTray.PreferredSize = new Dimension(120, 120);
			try
			{
				propertiesFile = new File("toastscript.properties");
				profile = new PropertyProfile(propertiesFile);
			}
			catch (Exception)
			{
				profile = new PropertyProfile();
			}
			int width = profile.getInteger("frame.width", 400);
			int height = profile.getInteger("frame.height", 400);
			this.Size = new Dimension(width, height);
			int x = profile.getInteger("frame.x", -1);
			int y = profile.getInteger("frame.y", -1);
			if (x < 0 || y < 0)
			{
				Dimension screenSize = Toolkit.DefaultToolkit.ScreenSize;
				if (height > screenSize.height)
				{
					height = screenSize.height;
				}
				if (width > screenSize.width)
				{
					width = screenSize.width;
				}
				x = (screenSize.width - width) / 2;
				y = (screenSize.height - height) / 2;
			}
			this.setLocation(x, y);
		}

		protected internal virtual void saveProfile()
		{
			Dimension d = Size;
			profile.setInteger("frame.width", d.width);
			profile.setInteger("frame.height", d.height);
			Point pt = Location;
			profile.setInteger("frame.x", pt.x);
			profile.setInteger("frame.y", pt.y);
			pageTray.saveProfile(profile);
			try
			{
				profile.save(propertiesFile, "ToastScript Properties");
			}
			catch (Exception)
			{
			}
		}

		protected internal virtual void postDialog(JDialog dialog)
		{
			Dimension dlgSize = dialog.PreferredSize;
			Dimension frmSize = Size;
			Point loc = Location;
			int x = (frmSize.width - dlgSize.width) / 2 + loc.x;
			int y = (frmSize.height - dlgSize.height) / 2 + loc.y;
			dialog.setLocation(x, y);
			dialog.Visible = true;
		}

		internal bool modalResult;

		protected internal virtual bool alert(string title, string approveText)
		{
			modalResult = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final JDialog alert = new JDialog(this, title, true);
			JDialog alert = new JDialog(this, title, true);
			JButton ok = new JButton(approveText);
			JButton cancel = new JButton("Cancel");
			Container contentPane = alert.ContentPane;
			contentPane.Layout = new FlowLayout();
			contentPane.add(ok);
			contentPane.add(cancel);
			ok.addActionListener(new ActionListenerAnonymousInnerClass3(this, alert));
			cancel.addActionListener(new ActionListenerAnonymousInnerClass4(this, alert));
			alert.pack();
			postDialog(alert);
			return modalResult;
		}

		private class ActionListenerAnonymousInnerClass3 : ActionListener
		{
			private readonly ViewFrame outerInstance;

			private JDialog alert;

			public ActionListenerAnonymousInnerClass3(ViewFrame outerInstance, JDialog alert)
			{
				this.outerInstance = outerInstance;
				this.alert = alert;
			}

			public void actionPerformed(ActionEvent evt)
			{
				alert.Visible = false;
				outerInstance.modalResult = true;
			}
		}

		private class ActionListenerAnonymousInnerClass4 : ActionListener
		{
			private readonly ViewFrame outerInstance;

			private JDialog alert;

			public ActionListenerAnonymousInnerClass4(ViewFrame outerInstance, JDialog alert)
			{
				this.outerInstance = outerInstance;
				this.alert = alert;
			}

			public void actionPerformed(ActionEvent evt)
			{
				alert.Visible = false;
				outerInstance.modalResult = false;
			}
		}

		internal class FormatListener : ActionListener
		{
			private readonly ViewFrame outerInstance;


			internal string format;

			internal FormatListener(ViewFrame outerInstance, string format)
			{
				this.outerInstance = outerInstance;
				this.format = format;
			}

			public virtual void actionPerformed(ActionEvent evt)
			{
				outerInstance.PostScriptPane.exec(format);
			}

		}

		internal class OrientationListener : ActionListener
		{
			private readonly ViewFrame outerInstance;


			internal string orientation;

			internal OrientationListener(ViewFrame outerInstance, string orientation)
			{
				this.outerInstance = outerInstance;
				this.orientation = orientation;
			}

			public virtual void actionPerformed(ActionEvent evt)
			{
				int orient = 0;
				if (!orientation.Equals("normal"))
				{
					orient = 1;
				}
				outerInstance.PostScriptPane.Orientation = orient;
			}

		}

		internal class ConsoleListener : ActionListener
		{
			private readonly ViewFrame outerInstance;

			public ConsoleListener(ViewFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual void actionPerformed(ActionEvent evt)
			{
				outerInstance.consoleAction(evt);
			}

		}

	}

}