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

	public abstract class Any : ICloneable, Stoppable, Types
	{

		public const int TYPEMASK = 0x0000FFFF;
		public static readonly int STOPPEDCONTEXT = 1 << 16;
		public static readonly int LOOPCONTEXT = 1 << 17;
		public static readonly int QUITCONTEXT = 1 << 18;

		internal const int EXEC_BIT = 1;
		internal const int RMODE_BIT = 2;
		internal const int WMODE_BIT = 4;
		internal const int XMODE_BIT = 8;
		internal const int GLOBAL_BIT = 16;
		internal const int PACKED_BIT = 32;
		internal const int BIND_BIT = 64;
		internal const int LINENOSHIFT = 10;
		internal static readonly int LINENOMASK = -1 << LINENOSHIFT;

		/// <summary>
		/// Basic object flags.
		/// </summary>
		private int flags = RMODE_BIT | WMODE_BIT | XMODE_BIT | LINENOMASK;

		/// <summary>
		/// Construct a new object.
		/// </summary>
		public Any()
		{
		}

		/// <summary>
		/// Construct a new object. </summary>
		/// <param name="any"> the template </param>
		public Any(Any any)
		{
			flags = any.flags;
		}

		/// <returns> a clone of the object </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
			return base.clone();
		}

		/// <returns> a type code for this object </returns>
		public abstract int typeCode();

		/// <returns> a type name for this object </returns>
		public abstract string typeName();

		/// <returns> java representation </returns>
		public virtual object cvj()
		{
			throw new Stop(Stoppable_Fields.TYPECHECK);
		}

		/// <returns> the type id </returns>
		public virtual int typeID()
		{
			return typeCode() & TYPEMASK;
		}

		/// <param name="bits"> the type mask </param>
		/// <returns> true if this object matches this type </returns>
		public virtual bool typeOf(int bits)
		{
			return (typeCode() & bits) != 0;
		}

		/// <summary>
		/// Execute the object. </summary>
		/// <param name="ip"> the ps-interpreter </param>
		public virtual void exec(Interpreter ip)
		{
			ip.ostack.push(this);
		}

		/// <summary>
		/// Temporarily overwrite the access attributes. </summary>
		/// <returns> the access attributes </returns>
		public virtual int saveAccessFlags()
		{
			int save = this.flags;
			this.flags |= RMODE_BIT | WMODE_BIT | XMODE_BIT;
			return save;
		}

		/// <summary>
		/// Restore the access attributes. </summary>
		/// <param name="flags"> the access attributes </param>
		public virtual void restoreAccessFlags(int flags)
		{
			this.flags = flags;
		}

		public virtual bool Literal
		{
			get
			{
				return (flags & EXEC_BIT) == 0;
			}
		}

		public virtual bool Executable
		{
			get
			{
				return (flags & EXEC_BIT) != 0;
			}
		}

		public virtual Any cvx()
		{
			flags |= EXEC_BIT;
			return this;
		}

		public virtual Any cvlit()
		{
			flags &= ~EXEC_BIT;
			return this;
		}

		public virtual bool rcheck()
		{
			return (flags & RMODE_BIT) != 0;
		}

		public virtual bool wcheck()
		{
			return (flags & WMODE_BIT) != 0;
		}

		public virtual Any noaccess()
		{
			flags &= ~(WMODE_BIT | RMODE_BIT | XMODE_BIT);
			return this;
		}

		public virtual Any executeonly()
		{
			flags &= ~(WMODE_BIT | RMODE_BIT);
			return this;
		}

		public virtual Any @readonly()
		{
			flags &= ~WMODE_BIT;
			return this;
		}

		protected internal virtual void setBound()
		{
			flags &= ~WMODE_BIT;
			flags |= BIND_BIT;
		}

		protected internal virtual bool Bound
		{
			get
			{
				return (flags & BIND_BIT) != 0;
			}
		}

		protected internal virtual void setGlobal()
		{
			flags |= GLOBAL_BIT;
		}

		protected internal virtual bool Global
		{
			get
			{
				return (flags & GLOBAL_BIT) != 0;
			}
		}

		protected internal virtual bool Packed
		{
			set
			{
				if (value)
				{
					flags |= PACKED_BIT;
		//			flags &= ~WMODE_BIT;	// this breaks "bind"
				}
				else
				{
					flags &= ~PACKED_BIT;
				}
			}
			get
			{
				return (flags & PACKED_BIT) != 0;
			}
		}


		protected internal virtual int LineNo
		{
			set
			{
				flags = (flags & ~LINENOMASK) | (value << LINENOSHIFT);
			}
			get
			{
				return flags >> LINENOSHIFT;
			}
		}

		protected internal virtual Any LineNo
		{
			set
			{
				flags = (flags & ~LINENOMASK) | (value.flags & LINENOMASK);
			}
		}


	}

}