﻿using System;

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

	public class Viewer
	{

		private bool packFrame = false;

		public Viewer()
		{
			ViewFrame frame = new ViewFrame();
			//Validate frames that have preset sizes
			//Pack frames that have useful preferred size info, e.g. from their layout
			if (packFrame)
			{
				frame.pack();
			}
			else
			{
				frame.validate();
			}
			frame.Visible = true;
		}

		public static void Main(string[] args)
		{
			try
			{
				UIManager.LookAndFeel = UIManager.SystemLookAndFeelClassName;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
			new Viewer();
		}

	}

}