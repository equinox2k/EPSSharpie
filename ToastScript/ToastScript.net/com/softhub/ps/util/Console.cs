using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace com.softhub.ps.util
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


	public class Console : Frame, ThreadStart
	{

		public const string SHOW = "Show Console";
		public const string HIDE = "Hide Console";

		private static string EOL = "\n";

		private Thread thread;
		private Stream @in;
		private PrintStream @out;
		private TextArea textarea = new TextArea();
		private StringBuilder buffer = new StringBuilder();
		private int bufferindex;
		private List<object> listeners = new List<object>();

		public Console(Stream @in, PrintStream @out) : base("Console")
		{
			this.@in = @in;
			this.@out = @out;
			this.enableEvents(AWTEvent.WINDOW_EVENT_MASK);
			Layout = new GridLayout(1,1);
			textarea.Editable = true;
			add(textarea);
			Dimension d = Toolkit.ScreenSize;
			setLocation(d.width - 480, d.height - 288);
			setSize(460, 288);
		}

		public virtual void addNotify()
		{
			base.addNotify();
			thread = new Thread(this, "ps-console");
			thread.Start();
		}

		public virtual void addActionListener(ActionListener listener)
		{
			listeners.Add(listener);
		}

		public virtual void removeActionListener(ActionListener listener)
		{
			listeners.Remove(listener);
		}

		protected internal virtual void fireEvent(ActionEvent evt)
		{
			System.Collections.IEnumerator e = listeners.elements();
			while (e.MoveNext())
			{
				((ActionListener) e.Current).actionPerformed(evt);
			}
		}

		public virtual bool Visible
		{
			set
			{
				base.Visible = value;
				string arg = value ? SHOW : HIDE;
				fireEvent(new ActionEvent(this, ActionEvent.ACTION_PERFORMED, arg));
			}
		}

		public virtual void run()
		{
			try
			{
				while (true)
				{
					int c = @in.Read();
					if (c >= 0)
					{
						buffer.Append((char) c);
						if (c == '\r' || c == '\n')
						{
							flush();
						}
					}
				}
			}
			catch (IOException ex)
			{
				System.Console.Error.WriteLine("i/o ex. in console: " + ex);
			}
		}

		public virtual void flush()
		{
			string text = buffer.ToString();
			int textlen = textarea.Text.length();
			if (bufferindex > textlen)
			{
				textarea.append(text);
			}
			else
			{
				textarea.insert(text, bufferindex);
				bufferindex += text.Length;
			}
			buffer = new StringBuilder();
		}

		public virtual bool keyDown(Event evt, int key)
		{
			if (key != '\n' && key != '\r')
			{
				return false;
			}
			int selstart = textarea.SelectionStart;
			int selend = textarea.SelectionEnd;
			string text = textarea.Text;
			textarea.append(EOL);
			int startindex = text.LastIndexOf(EOL, selstart - 1, StringComparison.Ordinal);
			int endindex = text.IndexOf(EOL, selstart, StringComparison.Ordinal);
			int si = startindex < 0 ? 0 : startindex;
			int ei = endindex < 0 ? text.Length : endindex;
			bufferindex = ei + 1;
			@out.println(text.Substring(si, ei - si));
			@out.flush();
			return true;
		}

		public virtual void processEvent(AWTEvent @event)
		{
			switch (@event.ID)
			{
			case Event.WINDOW_DESTROY:
				Visible = false;
				break;
			default:
				break;
			}
			base.processEvent(@event);
		}

	}

}