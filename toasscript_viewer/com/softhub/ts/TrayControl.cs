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

	using ViewEvent = com.softhub.ts.@event.ViewEvent;
	using ViewEventListener = com.softhub.ts.@event.ViewEventListener;
	using TrayControlEvent = com.softhub.ts.@event.TrayControlEvent;
	using TrayControlListener = com.softhub.ts.@event.TrayControlListener;

	public class TrayControl : JPanel, ViewEventListener
	{
		private static readonly float[] scaleFactors = new float[] {2.50f, 2.00f, 1.75f, 1.50f, 1.25f, 1.00f, 0.90f, 0.75f, 0.50f};

		private List<object> listeners = new List<object>();
		private JComboBox comboBox = new JComboBox();
		private JPanel leftPane = new JPanel();
		private BorderLayout trayControlLayout = new BorderLayout(3, 0);
		private JPanel rightPane = new JPanel();
		private bool actionLock;

		public TrayControl()
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
			this.Layout = trayControlLayout;
			comboBox.Font = new java.awt.Font("SansSerif", 0, 10);
			comboBox.MinimumSize = new Dimension(140, 16);
			comboBox.PreferredSize = new Dimension(160, 16);
			comboBox.MaximumRowCount = 12;
			comboBox.addActionListener(new ActionListenerAnonymousInnerClass(this));
			this.MinimumSize = new Dimension(80, 16);
			this.PreferredSize = new Dimension(220, 16);
			leftPane.Border = BorderFactory.createEtchedBorder();
			leftPane.PreferredSize = new Dimension(24, 16);
			rightPane.Border = BorderFactory.createEtchedBorder();
			rightPane.PreferredSize = new Dimension(24, 16);
			this.add(leftPane, BorderLayout.WEST);
			this.add(comboBox, BorderLayout.CENTER);
			this.add(rightPane, BorderLayout.EAST);
			initScaleFactors();
		}

		private class ActionListenerAnonymousInnerClass : java.awt.@event.ActionListener
		{
			private readonly TrayControl outerInstance;

			public ActionListenerAnonymousInnerClass(TrayControl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public void actionPerformed(ActionEvent e)
			{
				outerInstance.comboBoxAction(e);
			}
		}

		private void initScaleFactors()
		{
			int i, n = scaleFactors.Length;
			for (i = 0; i < n; i++)
			{
				addScaleFactor(scaleFactors[i]);
			}
	//		comboBox.addItem("---------");
	//		comboBox.addItem("Fit Window");
	//		comboBox.addItem("Actual Size");
	//		comboBox.addItem("Fit Width");
		}

		private void addScaleFactor(float scale)
		{
			string text = scaleFactorToString(scale);
			comboBox.addItem(text);
		}

		private static string scaleFactorToString(float scale)
		{
			return (int)Math.Round(scale * 100, MidpointRounding.AwayFromZero) + "%";
		}

		public virtual void addTrayControlListener(TrayControlListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeTrayControlListener(TrayControlListener listener)
		{
			listeners.Remove(listener);
		}

		protected internal virtual void fireTrayControlEvent(TrayControlEvent evt)
		{
			System.Collections.IEnumerator e = listeners.elements();
			while (e.MoveNext())
			{
				TrayControlListener listener = (TrayControlListener) e.Current;
				listener.trayChange(evt);
			}
		}

		protected internal virtual void fireTrayScaleEvent(int index)
		{
			float scale = scaleFactors[index];
			fireTrayControlEvent(new TrayControlEvent(this, scale));
		}

		public virtual void viewChanged(ViewEvent evt)
		{
			try
			{
				actionLock = true;
				switch (evt.EventType)
				{
				case ViewEvent.PAGE_RESIZE:
					viewScaleChange((Viewable) evt.Source);
					break;
				}
			}
			finally
			{
				actionLock = false;
			}
		}

		private void viewScaleChange(Viewable page)
		{
			float scale = page.Scale;
			int i, n = scaleFactors.Length, sel = -1;
			for (i = 0; i < n && sel < 0; i++)
			{
				if (Math.Abs(scale - scaleFactors[i]) < 1e-3)
				{
					sel = i;
				}
			}
			if (sel >= 0)
			{
				comboBox.SelectedIndex = sel;
			}
			else
			{
				int count = comboBox.ItemCount;
				if (count > scaleFactors.Length)
				{
					comboBox.removeItemAt(count - 1);
				}
				addScaleFactor(scale);
			}
		}

		private void comboBoxAction(ActionEvent evt)
		{
			if (!actionLock)
			{
				JComboBox source = (JComboBox) evt.Source;
				int index = source.SelectedIndex;
				if (index >= 0)
				{
					fireTrayScaleEvent(index);
				}
			}
		}

	}

}