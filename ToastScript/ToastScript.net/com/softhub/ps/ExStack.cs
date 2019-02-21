using System;
using System.Threading;

namespace com.softhub.ps
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

	public class ExStack : System.Collections.Stack
	{

		internal const int YIELD_COUNT = 5000;

		private int yieldCount;
		private int ostackcount;
		private Any currentobject;
		private bool interrupted;

		/// <summary>
		/// Construct an execution stack. </summary>
		/// <param name="size"> the size of the stack </param>
		public ExStack(int size) : base(size)
		{
		}

		/// <summary>
		/// Run the PostScript engine until the execution stack
		/// is empty. </summary>
		/// <param name="ip"> the interpreter </param>
		public virtual void run(Interpreter ip)
		{
			run(ip, 0);
		}

		/// <summary>
		/// Push an object onto the execution stack and run the
		/// PostScript engine until the object is popped off.
		/// This is used for recursive invocation by some operators
		/// which expect a procedure parameter. </summary>
		/// <param name="ip"> the interpreter </param>
		/// <param name="any"> the object to execute </param>
		public virtual void run(Interpreter ip, Any any)
		{
			Any tmp = currentobject;
			this.push(any);
			run(ip, count_Renamed - 1);
			currentobject = tmp;
		}

		/// <summary>
		/// Run the PostScript engine until the execution stack
		/// reaches estackcount. </summary>
		/// <param name="ip"> the interpreter </param>
		private void run(Interpreter ip, int estackcount)
		{
			do
			{
				try
				{
					runSave(ip, estackcount);
				}
				catch (Stop ex)
				{
					switch (ex.ExceptionId)
					{
					case Stoppable_Fields.EXSTACKOVERFLOW:
						// remove topmost element to make
						// space for the stop object
						this.remove(1);
						break;
					case Stoppable_Fields.DICTSTACKOVERFLOW:
						// remove 2 elements from dict stack to make
						// space for dictionaries the error handler
						// will push
						ip.dstack.remove(2);
						// fall through
						goto default;
					default:
						// push the object which caused the error
						// onto execution stack
						pushRef(currentobject);
					break;
					}
					// restore the operand stack to the state
					// before the error occured
					ip.ostack.count_Renamed = ostackcount;
					// record the error
					ex.recorderror(ip);
					// execute the stop operator
					this.push(ip.systemdict.get("stop"));
				}
			} while (count_Renamed > estackcount);
		}

		/// <summary>
		/// Execute the objects on the stack until index 'estackcount'
		/// is reached or and error occured. </summary>
		/// <param name="ip"> the interpreter </param>
		/// <param name="estackcount"> the 'low water mark' </param>
		private void runSave(Interpreter ip, int estackcount)
		{
			try
			{
				while (count_Renamed > estackcount)
				{
					ostackcount = ip.ostack.count_Renamed;
					currentobject = array[--count_Renamed];
					array[count_Renamed] = null;
					if (yieldCount++ >= YIELD_COUNT)
					{
						if (interrupted)
						{
							interrupted = false;
							throw new Stop(Stoppable_Fields.INTERRUPT);
						}
						Thread.yield();
						yieldCount = 0;
					}
					currentobject.exec(ip);
				}
			}
			catch (System.OutOfMemoryException)
			{
				System.GC.Collect();
				throw new Stop(Stoppable_Fields.INTERNALERROR, "out of memory");
			}
			catch (Stop ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine("internal error in " + currentobject);
				throw new Stop(Stoppable_Fields.INTERNALERROR, ex.Message);
			}
		}

		internal virtual void interrupt(bool state)
		{
			interrupted = state;
		}

		protected internal override int overflow()
		{
			return Stoppable_Fields.EXSTACKOVERFLOW;
		}

		protected internal override int underflow()
		{
			return Stoppable_Fields.INTERNALERROR;
		}

	}

}