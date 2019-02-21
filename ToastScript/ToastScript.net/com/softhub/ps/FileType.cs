using System;
using System.Collections.Generic;
using System.IO;

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

	using Archive = com.softhub.ps.util.Archive;

	public class FileType : CharSequenceType
	{

		protected internal static List<object> nodes = new List<object>();

		/// <summary>
		/// The shared data.
		/// </summary>
		protected internal FileNode node;

		public FileType(VM vm, FileType parent, URL @base, StringType name, int mode) : base(vm)
		{
			switch (mode)
			{
			case util.CharStream_Fields.READ_MODE:
				node = new ReadFileNode(vm, parent.node, @base, name.ToString());
				break;
			case util.CharStream_Fields.WRITE_MODE:
				node = new WriteFileNode(vm, parent.node, @base, name.ToString());
				break;
			default:
				throw new Stop(Stoppable_Fields.INTERNALERROR);
			}
			nodes.Add(node);
		}

		protected internal FileType(VM vm, string name, Stream stream) : base(vm)
		{
			node = new ReadFileNode(vm, name, stream);
			nodes.Add(node);
		}

		protected internal FileType(VM vm, string name, Stream stream) : base(vm)
		{
			node = new WriteFileNode(vm, name, stream);
			nodes.Add(node);
		}

		protected internal FileType(VM vm, FileNode node) : base(vm)
		{
			this.node = node;
			nodes.Add(node);
		}

		public override int typeCode()
		{
			return Types_Fields.FILE;
		}

		public override string typeName()
		{
			return "filetype";
		}

		public virtual string Name
		{
			get
			{
				return node.name;
			}
		}

		protected internal override bool Global
		{
			get
			{
				return true;
			}
		}

		public override int LineNo
		{
			get
			{
				return node.lineno;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getchar() throws java.io.IOException
		public override int getchar()
		{
			return node.getchar();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void putchar(int c) throws java.io.IOException
		public override void putchar(int c)
		{
			node.putchar(c);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ungetchar(int c) throws java.io.IOException
		public override void ungetchar(int c)
		{
			node.ungetchar(c);
		}

		public override void exec(Interpreter ip)
		{
			if (Literal)
			{
				ip.ostack.pushRef(this);
			}
			else
			{
				if (node.WriteMode)
				{
					throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
				}
				if (node.scanner == null)
				{
					node.scanner = new Scanner();
				}
				ip.estack.pushRef(this);
				if (ip.scan(this, node.scanner, true) == Scanner.EOF)
				{
					ip.estack.pop();
					close();
				}
			}
		}

		public override bool token(Interpreter ip)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "token(file,rcheck)");
			}
			if (node.WriteMode)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}
			if (node.scanner == null)
			{
				node.scanner = new Scanner();
			}
			return ip.scan(this, node.scanner, false) != Scanner.EOF;
		}

		public virtual void flush()
		{
			try
			{
				node.flush();
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, ex.ToString());
			}
		}

		public virtual int bytesavailable()
		{
			try
			{
				return node.bytesavailable();
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, ex.ToString());
			}
		}

		public virtual int FilePosition
		{
			set
			{
				try
				{
					node.FilePosition = value;
				}
				catch (IOException ex)
				{
					throw new Stop(Stoppable_Fields.IOERROR, ex.ToString());
				}
			}
			get
			{
				return node.FilePosition;
			}
		}


		public virtual void resetfile(URL @base)
		{
			try
			{
				node.resetfile(@base);
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, ex.ToString());
			}
		}

		public virtual int read()
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS, "read(file,rcheck)");
			}
			try
			{
				return node.getchar();
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public virtual void write(int c)
		{
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				node.putchar(c);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public virtual bool read(VM vm, StringType s, StringType[] result)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				return node.read(vm, s, result);
			}
			catch (IOException ex)
			{
				System.Console.WriteLine(ex.ToString());
				System.Console.Write(ex.StackTrace);
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public virtual void write(StringType s)
		{
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				node.write(s);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public virtual bool readhex(VM vm, StringType s, StringType[] result)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				return node.readhex(vm, s, result);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public virtual void writehex(StringType s)
		{
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				node.writehex(s);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public virtual bool readline(VM vm, StringType s, StringType[] result)
		{
			if (!rcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				return node.readline(vm, s, result);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		public override void close()
		{
			try
			{
				node.close();
				nodes.Remove(node);
			}
			catch (IOException ex)
			{
				throw new Stop(Stoppable_Fields.IOERROR, ex.ToString());
			}
		}

		public virtual void print(StringType @string)
		{
			if (!wcheck())
			{
				throw new Stop(Stoppable_Fields.INVALIDACCESS);
			}
			try
			{
				node.print(@string);
			}
			catch (IOException)
			{
				throw new Stop(Stoppable_Fields.IOERROR);
			}
		}

		internal virtual bool Closed
		{
			get
			{
				return node.Closed;
			}
		}

		/// <returns> the shared data's save level </returns>
		public override int SaveLevel
		{
			get
			{
				return node.SaveLevel;
			}
		}

		/// <summary>
		/// Compare file to another object. </summary>
		/// <param name="obj"> the other object </param>
		/// <returns> true if equal; false otherwise </returns>
		public override bool Equals(object obj)
		{
			return obj is FileType && node.name.Equals(((FileType) obj).node.name);
		}

		/// <returns> a string representation </returns>
		public override string ToString()
		{
			return "file<" + node.name + ">";
		}

		/// <returns> a hash code for this file </returns>
		public override int GetHashCode()
		{
			return node.name.GetHashCode();
		}

		public static void flushAllOpenFiles()
		{
			int i, n = nodes.Count;
			for (i = n - 1; i >= 0; i--)
			{
				// Backwards: Close new files before old ones.
				FileNode node = (FileNode) nodes[i];
				try
				{
					node.close();
				}
				catch (IOException)
				{
				}
			}
		}

		internal abstract class FileNode : Node
		{

			/// <summary>
			/// The file name.
			/// </summary>
			protected internal string name;

			/// <summary>
			/// The file.
			/// </summary>
			protected internal File file;

			/// <summary>
			/// The parent file, if any.
			/// </summary>
			protected internal File parent;

			/// <summary>
			/// The number of chars read/written.
			/// </summary>
			protected internal int charCount;

			/// <summary>
			/// Close flag.
			/// </summary>
			protected internal bool closed;

			/// <summary>
			/// The line number.
			/// </summary>
			protected internal int lineno = 1;

			/// <summary>
			/// The scanner.
			/// </summary>
			protected internal Scanner scanner;

			protected internal FileNode(VM vm, string name) : base(vm)
			{
				this.name = name;
			}

			protected internal FileNode(VM vm, FileNode parent, URL @base, string name) : this(vm, name)
			{
				this.parent = parent.file;
			}

			protected internal virtual File createFile(string name)
			{
				File file = new File(name);
				if (!file.Absolute)
				{
					string parentName = parent == null ? null : parent.Parent;
					file = new File(parentName, name);
				}
				return file;
			}

			protected internal virtual int FilePosition
			{
				get
				{
					return charCount;
				}
			}

			protected internal virtual bool Closed
			{
				get
				{
					return closed;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int getchar() throws java.io.IOException;
			protected internal abstract int getchar();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void putchar(int c) throws java.io.IOException;
			protected internal abstract void putchar(int c);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void ungetchar(int c) throws java.io.IOException;
			protected internal abstract void ungetchar(int c);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int rawRead() throws java.io.IOException;
			protected internal abstract int rawRead();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void rawWrite(int c) throws java.io.IOException;
			protected internal abstract void rawWrite(int c);

			protected internal abstract bool ReadMode {get;}

			protected internal abstract bool WriteMode {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void flush() throws java.io.IOException;
			protected internal abstract void flush();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract int bytesavailable() throws java.io.IOException;
			protected internal abstract int bytesavailable();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void setFilePosition(int pos) throws java.io.IOException;
			protected internal abstract int FilePosition {set;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void resetfile(java.net.URL super) throws java.io.IOException;
			protected internal abstract void resetfile(URL @base);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract boolean read(VM vm, StringType s, StringType result[]) throws java.io.IOException;
			protected internal abstract bool read(VM vm, StringType s, StringType[] result);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void write(StringType s) throws java.io.IOException;
			protected internal abstract void write(StringType s);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract boolean readhex(VM vm, StringType s, StringType result[]) throws java.io.IOException;
			protected internal abstract bool readhex(VM vm, StringType s, StringType[] result);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void writehex(StringType s) throws java.io.IOException;
			protected internal abstract void writehex(StringType s);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract boolean readline(VM vm, StringType s, StringType result[]) throws java.io.IOException;
			protected internal abstract bool readline(VM vm, StringType s, StringType[] result);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void close() throws java.io.IOException;
			protected internal abstract void close();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void print(StringType string) throws java.io.IOException;
			protected internal abstract void print(StringType @string);

		}

		internal class ReadFileNode : FileNode
		{

			/// <summary>
			/// The input stream.
			/// </summary>
			internal Stream istream;

			/// <summary>
			/// The push back flag.
			/// </summary>
			internal bool pushed;

			/// <summary>
			/// The push back character.
			/// </summary>
			protected internal int pushedchar;

			internal ReadFileNode(VM vm, string name) : base(vm, name)
			{
			}

			internal ReadFileNode(VM vm, FileNode parent, URL @base, string name) : base(vm, parent, @base, name)
			{
				try
				{
					openForRead(@base);
				}
				catch (IOException ex)
				{
					throw new Stop(Stoppable_Fields.IOERROR, "open " + name + " " + ex);
				}
			}

			internal ReadFileNode(VM vm, string name, Stream istream) : this(vm, name)
			{
				this.istream = istream;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void openForRead(java.net.URL super) throws java.io.IOException
			internal virtual void openForRead(URL @base)
			{
				Stream @in = null;
				try
				{
					// open as resource from jar file
					@in = this.GetType().ClassLoader.getResource(name).openStream();
				}
				catch (Exception)
				{
					try
					{
						// open stream from URL
						@in = (new URL(@base, name)).openStream();
					}
					catch (Exception)
					{
					}
					if (@in == null)
					{
						try
						{
							// open from archive
							@in = (new Archive(name)).openStream();
						}
						catch (Exception)
						{
							try
							{
								// open as file
								file = createFile(name);
								@in = new FileStream(file, FileMode.Open, FileAccess.Read);
							}
							catch (Exception)
							{
							}
						}
					}
				}
				if (@in == null)
				{
					throw new Stop(Stoppable_Fields.UNDEFINEDFILENAME, parent + ", " + name);
				}
				istream = new BufferedInputStream(@in);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int getchar() throws java.io.IOException
			protected internal override int getchar()
			{
				if (closed)
				{
					return -1;
				}
				charCount++;
				if (pushed)
				{
					pushed = false;
					return pushedchar;
				}
				int c = rawRead();
				switch (c)
				{
				case '\r':
					lineno++;
					// fall through
					goto case '\n';
				case '\n':
					break;
				}
				return c;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void putchar(int c) throws java.io.IOException
			protected internal override void putchar(int c)
			{
				throw new IOException();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void ungetchar(int c) throws java.io.IOException
			protected internal override void ungetchar(int c)
			{
				pushed = true;
				pushedchar = c;
			}

			protected internal override bool ReadMode
			{
				get
				{
					return true;
				}
			}

			protected internal override bool WriteMode
			{
				get
				{
					return false;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int rawRead() throws java.io.IOException
			protected internal override int rawRead()
			{
				return istream.Read();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void rawWrite(int c) throws java.io.IOException
			protected internal override void rawWrite(int c)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void flush() throws java.io.IOException
			protected internal override void flush()
			{
				int i, n, c = 0;
				while ((n = bytesavailable()) > 0)
				{
					for (i = 0; i < n && c >= 0; i++)
					{
						c = getchar();
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int bytesavailable() throws java.io.IOException
			protected internal override int bytesavailable()
			{
				int n = istream.available();
				return n == 0 ? -1 : n;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setFilePosition(int pos) throws java.io.IOException
			protected internal override int FilePosition
			{
				set
				{
					resetfile(null);
					if (value > 0)
					{
						istream.skip(value);
						charCount = value;
					}
					pushed = false;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void resetfile(java.net.URL super) throws java.io.IOException
			protected internal override void resetfile(URL @base)
			{
				istream.Close();
				openForRead(@base);
				pushed = false;
				charCount = 0;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean read(VM vm, StringType s, StringType result[]) throws java.io.IOException
			protected internal override bool read(VM vm, StringType s, StringType[] result)
			{
				int i = 0, c = 0, len = s.length();
				if (len <= 0)
				{
					throw new Stop(Stoppable_Fields.RANGECHECK);
				}
				while (c >= 0 && i < len)
				{
					if ((c = getchar()) >= 0)
					{
						s.put(vm, i++, c);
					}
				}
				result[0] = new StringType(s, 0, i);
				return c >= 0;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void write(StringType s) throws java.io.IOException
			protected internal override void write(StringType s)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean readhex(VM vm, StringType s, StringType result[]) throws java.io.IOException
			protected internal override bool readhex(VM vm, StringType s, StringType[] result)
			{
				int[] buffer = new int[2];
				bool eof = false;
				int i = 0, j = 0, k = 1, c, len = s.length();
				if (len > 0)
				{
					while (!eof && i < len)
					{
						int cc = getchar();
						if (cc >= 0)
						{
							if ((c = Scanner.hexValue(cc)) >= 0)
							{
								k = j++ % 2;
								buffer[k] = c;
								if (k != 0)
								{
									s.put(vm, i++, 16 * buffer[0] + buffer[1]);
								}
							}
						}
						else
						{
							eof = true;
						}
					}
					if (k == 0)
					{
						s.put(vm, i++, 16 * buffer[0]);
					}
				}
				result[0] = new StringType(s, 0, i);
				return !eof;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writehex(StringType s) throws java.io.IOException
			protected internal override void writehex(StringType s)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean readline(VM vm, StringType s, StringType result[]) throws java.io.IOException
			protected internal override bool readline(VM vm, StringType s, StringType[] result)
			{
				int i = 0, c = 0, len = s.length();
				bool eol = false;
				while (c >= 0 && !eol)
				{
					if (i >= len)
					{
						throw new Stop(Stoppable_Fields.RANGECHECK);
					}
					if ((c = getchar()) >= 0)
					{
						if (c != '\r' && c != '\n')
						{
							s.put(vm, i++, c);
						}
						else
						{
							eol = true;
						}
					}
				}
				s.LineNo = lineno;
				result[0] = new StringType(s, 0, i);
				return c >= 0;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void close() throws java.io.IOException
			protected internal override void close()
			{
				istream.Close();
				closed = true;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void print(StringType string) throws java.io.IOException
			protected internal override void print(StringType @string)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

		}

		internal class WriteFileNode : FileNode
		{

			/// <summary>
			/// The output stream.
			/// </summary>
			internal Stream ostream;

			internal WriteFileNode(VM vm, string name) : base(vm, name)
			{
			}

			internal WriteFileNode(VM vm, FileNode parent, URL @base, string name) : base(vm, parent, @base, name)
			{
				try
				{
					openForWrite(@base);
				}
				catch (IOException ex)
				{
					throw new Stop(Stoppable_Fields.IOERROR, "open " + name + " " + ex);
				}
			}

			internal WriteFileNode(VM vm, string name, Stream ostream) : this(vm, name)
			{
				this.ostream = new BufferedOutputStream(ostream);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void openForWrite(java.net.URL super) throws java.io.IOException
			internal virtual void openForWrite(URL @base)
			{
				try
				{
					Stream @out;
					if (@base == null)
					{
						file = createFile(name);
						@out = new FileStream(file, FileMode.Create, FileAccess.Write);
					}
					else
					{
						URL url = new URL(@base, name);
						URLConnection connection = url.openConnection();
						connection.DoOutput = true;
						@out = connection.OutputStream;
					}
					ostream = new BufferedOutputStream(@out);
				}
				catch (IOException ex)
				{
					throw new Stop(Stoppable_Fields.UNDEFINEDFILENAME, ex.Message);
				}
				catch (SecurityException ex)
				{
					throw new Stop(Stoppable_Fields.SECURITYCHECK, ex.Message);
				}
			}

			protected internal override bool ReadMode
			{
				get
				{
					return false;
				}
			}

			protected internal override bool WriteMode
			{
				get
				{
					return true;
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int getchar() throws java.io.IOException
			protected internal override int getchar()
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void putchar(int c) throws java.io.IOException
			protected internal override void putchar(int c)
			{
				charCount++;
				rawWrite(c);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void ungetchar(int c) throws java.io.IOException
			protected internal override void ungetchar(int c)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int rawRead() throws java.io.IOException
			protected internal override int rawRead()
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void rawWrite(int c) throws java.io.IOException
			protected internal override void rawWrite(int c)
			{
				ostream.WriteByte(c);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void flush() throws java.io.IOException
			protected internal override void flush()
			{
				ostream.Flush();
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int bytesavailable() throws java.io.IOException
			protected internal override int bytesavailable()
			{
				return -1;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setFilePosition(int pos) throws java.io.IOException
			protected internal override int FilePosition
			{
				set
				{
					ostream.Flush();
					// TODO: support setFilePosition
					throw new Stop(Stoppable_Fields.INTERNALERROR, "not supported");
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void resetfile(java.net.URL super) throws java.io.IOException
			protected internal override void resetfile(URL @base)
			{
				ostream.Close();
				openForWrite(@base);
				charCount = 0;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean read(VM vm, StringType s, StringType result[]) throws java.io.IOException
			protected internal override bool read(VM vm, StringType s, StringType[] result)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void write(StringType s) throws java.io.IOException
			protected internal override void write(StringType s)
			{
				int len = s.length();
				for (int i = 0; i < len; i++)
				{
					putchar(s.get(i));
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean readhex(VM vm, StringType s, StringType result[]) throws java.io.IOException
			protected internal override bool readhex(VM vm, StringType s, StringType[] result)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writehex(StringType s) throws java.io.IOException
			protected internal override void writehex(StringType s)
			{
				int c, c0, c1, len = s.length();
				for (int i = 0; i < len; i++)
				{
					c = s.get(i);
					c0 = c / 16;
					c1 = c % 16;
					putchar(c0 >= 0 && c0 <= 9 ? c0 + '0' : c0 + 'a');
					putchar(c1 >= 0 && c1 <= 9 ? c1 + '0' : c1 + 'a');
				}
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean readline(VM vm, StringType s, StringType result[]) throws java.io.IOException
			protected internal override bool readline(VM vm, StringType s, StringType[] result)
			{
				throw new Stop(Stoppable_Fields.INVALIDFILEACCESS);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void close() throws java.io.IOException
			protected internal override void close()
			{
				ostream.Flush();
				ostream.Close();
				closed = true;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void print(StringType string) throws java.io.IOException
			protected internal override void print(StringType @string)
			{
				string s = @string.ToString();
				for (int i = 0; i < s.Length; i++)
				{
					putchar(s[i]);
				}
			}

		}

	}

}