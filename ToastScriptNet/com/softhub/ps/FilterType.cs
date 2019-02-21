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

	using Codec = com.softhub.ps.filter.Codec;

	public class FilterType : FileType
	{

		/// <summary>
		/// Construct a filter object.
		/// </summary>
		public FilterType(VM vm, CharSequenceType stream, Codec codec, int mode) : base(vm, createNode(vm, stream, codec, mode))
		{
			try
			{
				codec.open(stream, mode);
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, ex.ToString());
			}
		}

		public virtual CharSequenceType SourceStream
		{
			get
			{
				if (node is ReadFilterNode)
				{
					return ((ReadFilterNode) node).stream;
				}
				if (node is WriteFilterNode)
				{
					return ((WriteFilterNode) node).stream;
				}
				throw new Stop(Stoppable_Fields.INTERNALERROR);
			}
		}

		private static FileNode createNode(VM vm, CharSequenceType stream, Codec codec, int mode)
		{
			switch (mode)
			{
			case util.CharStream_Fields.READ_MODE:
				return new ReadFilterNode(vm, stream, codec);
			case util.CharStream_Fields.WRITE_MODE:
				return new WriteFilterNode(vm, stream, codec);
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR);
			}
		}

		internal class ReadFilterNode : ReadFileNode
		{

			/// <summary>
			/// The stream we are reading from.
			/// </summary>
			internal CharSequenceType stream;

			/// <summary>
			/// The encoder/decoder.
			/// </summary>
			internal Codec codec;

			internal ReadFilterNode(VM vm, CharSequenceType stream, Codec codec) : base(vm, "filter")
			{
				this.stream = stream;
				this.codec = codec;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int rawRead() throws java.io.IOException
			protected internal override int rawRead()
			{
				return codec.decode();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void rawWrite(int c) throws java.io.IOException
			protected internal override void rawWrite(int c)
			{
				throw new IOException();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void close() throws java.io.IOException
			protected internal override void close()
			{
				codec.close();
				closed = true;
			}

		}

		internal class WriteFilterNode : WriteFileNode
		{

			/// <summary>
			/// The stream we are reading from.
			/// </summary>
			internal CharSequenceType stream;

			/// <summary>
			/// The encoder/decoder.
			/// </summary>
			internal Codec codec;

			internal WriteFilterNode(VM vm, CharSequenceType stream, Codec codec) : base(vm, "filter")
			{
				this.stream = stream;
				this.codec = codec;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int rawRead() throws java.io.IOException
			protected internal override int rawRead()
			{
				throw new IOException();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void rawWrite(int c) throws java.io.IOException
			protected internal override void rawWrite(int c)
			{
				codec.encode(c);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void close() throws java.io.IOException
			protected internal override void close()
			{
				codec.close();
				closed = true;
			}

		}

	}

}