using System;

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

	public class ReflectionOperator : OperatorType
	{

		private static object[] param = new object[1];

		private Type clazz;
		private System.Reflection.MethodInfo method;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ReflectionOperator(String name, Class clazz) throws NoSuchMethodException
		public ReflectionOperator(string name, Type clazz) : base(name)
		{
			this.clazz = clazz;
			Type[] paramTypes = new Type[1];
			paramTypes[0] = typeof(Interpreter);
			this.method = clazz.getDeclaredMethod(name, paramTypes);
		}

		public override void exec(Interpreter ip)
		{
			try
			{
				lock (param)
				{
					param[0] = ip;
					method.invoke(clazz, param);
				}
			}
			catch (InvocationTargetException ex)
			{
				Exception tex = ex.TargetException;
				if (tex is Stop)
				{
					throw (Stop) tex;
				}
				System.Console.Error.WriteLine("internal error in " + method);
				System.Console.WriteLine(tex.ToString());
				System.Console.Write(tex.StackTrace);
				throw new Stop(Stoppable_Fields.INTERNALERROR, ex + " target: " + tex + " method: " + method);
			}
			catch (IllegalAccessException ex)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "exec failed: " + ex.Message);
			}
		}

	}

}