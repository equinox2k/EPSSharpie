using System;
using System.Threading;
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
	/// 
	/// To instanciate the interpreter, pass the stdin
	/// and stdout to the constructor and call init when
	/// ready to begin execution. Once init is called, the
	/// server is read from the file "server.ps". The interpreter
	/// runs in its own thread and starts listening on stdin.
	/// This behavour can be customized in the "server.ps"
	/// script. To communicate with the interpreter, write
	/// PostScript code to stdin. Currently, the job server
	/// is part of the statusdict (see server.ps). If you do
	/// not use it, there will be no save/restore between jobs.
	/// The client code should create a page device and implement
	/// the PageEventListener interface to get notified of changes
	/// to the page device. The client can keep a list of pages
	/// and draw the appropriate page.
	/// </summary>

	using ImageDataProducer = com.softhub.ps.image.ImageDataProducer;
	using PageDevice = com.softhub.ps.device.PageDevice;
	using CharStream = com.softhub.ps.util.CharStream;

	public class Interpreter : ThreadStart, Stoppable, Types, ImageDataProducer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			systemdict = new DictType(vm, 300, true);
		}


		/// <summary>
		/// Debug flag.
		/// </summary>
		private bool debug;

		/// <summary>
		/// Interpreter thread
		/// </summary>
		private Thread thread;

		/// <summary>
		/// The base time.
		/// </summary>
		private long basetime;

		/// <summary>
		/// The resource dictionary.
		/// </summary>
		private DictType resources;

		/// <summary>
		/// Virtual Memory
		/// </summary>
		internal VM vm = new VM();

		/// <summary>
		/// System dictionary.
		/// </summary>
		internal DictType systemdict;

		/// <summary>
		/// Operand Stack
		/// </summary>
		internal System.Collections.Stack ostack = new OpStack();

		/// <summary>
		/// Execution Stack
		/// </summary>
		internal ExStack estack = new ExStack(250);

		/// <summary>
		/// Dictionary Stack
		/// </summary>
		internal DictStack dstack = new DictStack(20);

		/// <summary>
		/// Graphics State Stack
		/// </summary>
		internal GStack gstack = new GStack(32);

		/// <summary>
		/// Current Packing Mode
		/// </summary>
		internal bool arraypacking;

		/// <summary>
		/// %stdin
		/// </summary>
		internal FileType stdin;

		/// <summary>
		/// %stdin
		/// </summary>
		internal FileType stdout;

		/// <summary>
		/// %stdin
		/// </summary>
		internal FileType stderr;

		/// <summary>
		/// %lineedit
		/// </summary>
		internal FileType lineedit;

		/// <summary>
		/// The current line number.
		/// </summary>
		internal int lineno;

		/// <summary>
		/// Construct a ps-interpreter.
		/// </summary>
		public Interpreter() : this(System.in, System.out)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Construct a ps-interpreter. </summary>
		/// <param name="in"> the standard input stream </param>
		/// <param name="out"> the standard output stream </param>
		public Interpreter(Stream @in, Stream @out) : this(@in, @out, System.err)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// Construct a ps-interpreter. </summary>
		/// <param name="in"> the standard input stream </param>
		/// <param name="out"> the standard output stream </param>
		/// <param name="err"> the standard error stream </param>
		public Interpreter(Stream @in, Stream @out, Stream err)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			stdin = new SpecialFileType(vm, "%stdin", @in);
			stdout = new SpecialFileType(vm, "%stdout", @out);
			stderr = new SpecialFileType(vm, "%stderr", err);
			lineedit = new SpecialFileType(vm, "%lineedit", System.in);
			dstack.push(systemdict);
			GStateType gstate = new GraphicsState();
			gstack.pushRef(gstate);
			gstate.save(vm.SaveLevel);
			initResources();
			installOp();
		}

		/// <summary>
		/// Initialize the interpreter. </summary>
		/// <param name="device"> the default output device </param>
		public virtual void init(PageDevice device)
		{
			basetime = DateTimeHelper.CurrentUnixTimeMillis();
			GraphicsState gstate = GraphicsState;
			device.addPageEventListener(gstate);
			gstate.install(device);
			string server = Server;
			// read the server loop
			try
			{
				run(server);
			}
			catch (Stop ex)
			{
				System.Console.Error.WriteLine("Error in job server: " + ex + " line: " + lineno);
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine("init failed " + server);
				System.Console.WriteLine(ex.ToString());
				System.Console.Write(ex.StackTrace);
			}
			// set the dict stack bottom
			dstack.setBottom();
			DictType statusdict = StatusDict;
			Any key = statusdict.get("debug");
			debug = key is BoolType && ((BoolType) key).booleanValue();
			// start the interpreter
			systemdict.@readonly();
			estack.pushRef((new NameType("start")).cvx());
			thread = new Thread(this, "ps-interpreter");
			thread.Priority = Thread.MIN_PRIORITY;
			thread.Start();
		}

		/// <summary>
		/// Initialize the resource dictionary.
		/// </summary>
		private void initResources()
		{
			resources = new DictType(vm, 22);
			resources.put(vm, "Font", new DictType(vm, 35));
			resources.put(vm, "Encoding", new DictType(vm, 5));
			resources.put(vm, "Form", new DictType(vm, 5));
			resources.put(vm, "Pattern", new DictType(vm, 5));
			resources.put(vm, "ProcSet", new DictType(vm, 5));
			resources.put(vm, "ColorSpace", new DictType(vm, 5));
			resources.put(vm, "Halftone", new DictType(vm, 5));
			resources.put(vm, "ColorRendering", new DictType(vm, 5));
			resources.put(vm, "Filter", new DictType(vm, 5));
			resources.put(vm, "ColorSpaceFamily", new DictType(vm, 5));
			resources.put(vm, "Emulator", new DictType(vm, 5));
			resources.put(vm, "IODevice", new DictType(vm, 5));
			resources.put(vm, "ColorRenderingType", new DictType(vm, 5));
			resources.put(vm, "FMapType", new DictType(vm, 5));
			resources.put(vm, "FontType", new DictType(vm, 5));
			resources.put(vm, "FormType", new DictType(vm, 5));
			resources.put(vm, "HalftonType", new DictType(vm, 5));
			resources.put(vm, "ImageType", new DictType(vm, 5));
			resources.put(vm, "PatternType", new DictType(vm, 5));
			DictType catdict = new DictType(vm, 22);
			catdict.put(vm, "Generic", new DictType(vm, 10));
			resources.put(vm, "Category", catdict);
		}

		/// <summary>
		/// Save the graphics context.
		/// </summary>
		internal virtual void gsave()
		{
			GStateType gstate = (GStateType) gstack.dup();
			int savelevel = vm.SaveLevel;
			gstate.save(savelevel);
		}

		/// <summary>
		/// Restore the graphics context.
		/// </summary>
		internal virtual void grestore()
		{
			if (gstack.count() < 1)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "grestore: no initial gstate");
			}
			if (gstack.count() >= 2)
			{
				GStateType gstate = (GStateType) gstack.pop();
				GStateType @base = (GStateType) gstack.top();
				int savelevel = gstate.SaveLevel;
				if (@base.SaveLevel < savelevel)
				{
					@base = (GStateType) gstack.dup();
					@base.SaveLevel = savelevel;
				}
				@base.restore(gstate);
			}
		}

		/// <summary>
		/// Restore the graphics context down to the initial state, which
		/// is the state of the last recent save.
		/// </summary>
		internal virtual void grestoreAll()
		{
			int count = gstack.count();
			if (count < 1)
			{
				throw new Stop(Stoppable_Fields.INTERNALERROR, "grestoreall: no initial gstate");
			}
			if (count >= 2)
			{
				int savelevel = vm.SaveLevel;
				GStateType currentstate = (GStateType) gstack.pop();
				GStateType gstate = null;
				do
				{
					gstate = (GStateType) gstack.pop();
				} while (--count > 0 && gstate.SaveLevel > savelevel);
				gstack.pushRef(gstate);
				if (savelevel > gstate.SaveLevel)
				{
					gstate = (GStateType) gstack.dup();
					gstate.SaveLevel = savelevel;
				}
				gstate.restore(currentstate);
			}
		}

		/// <returns> the graphics context </returns>
		public virtual GraphicsState GraphicsState
		{
			get
			{
				return (GraphicsState) gstack.top();
			}
		}

		/// <returns> the debug mode as defined in statusdict </returns>
		public virtual bool DebugMode
		{
			get
			{
				return debug;
			}
		}

		/// <returns> the server name </returns>
		public virtual string Server
		{
			get
			{
				return "server.ps";
			}
		}

		/// <returns> the code base </returns>
		public virtual URL CodeBase
		{
			get
			{
				Any any = StatusDict.get("codebase");
				if (any != null)
				{
					try
					{
						return new URL(any.ToString());
					}
					catch (MalformedURLException)
					{
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Install operators in systemdict
		/// </summary>
		private void installOp()
		{
			systemdict.put(vm, "null", new NullType());
			systemdict.put(vm, "true", BoolType.TRUE);
			systemdict.put(vm, "false", BoolType.FALSE);
			systemdict.put(vm, "systemdict", systemdict);
			systemdict.put(vm, "globaldict", new DictType(vm, 20, true));
			systemdict.put(vm, "GlobalFontDirectory", new DictType(vm, 35, true));
			systemdict.put(vm, "userdict", new DictType(vm, 200));
			systemdict.put(vm, "errordict", new DictType(vm, 20));
			systemdict.put(vm, "$error", new DictType(vm, 20));
			systemdict.put(vm, "FontDirectory", new DictType(vm, 35));
			systemdict.put(vm, "statusdict", new DictType(vm, 10));
			ArithOp.install(this);
			ArrayOp.install(this);
			BoolOp.install(this);
			ControlOp.install(this);
			DictOp.install(this);
			FileOp.install(this);
			FontOp.install(this);
			GStateOp.install(this);
			ImageOp.install(this);
			MatrixOp.install(this);
			MiscOp.install(this);
			PathOp.install(this);
			ResourceOp.install(this);
			StackOp.install(this);
			StringOp.install(this);
			TypeOp.install(this);
			VMOp.install(this);
		}

		/// <summary>
		/// Install operators in systemdict </summary>
		/// <param name="op"> the operator </param>
		public virtual void installOp(OperatorType op)
		{
			installOp(op, systemdict);
		}

		/// <summary>
		/// Install operator into some dictionary </summary>
		/// <param name="op"> the operator </param>
		/// <param name="dict"> the dictionary </param>
		public virtual void installOp(OperatorType op, DictType dict)
		{
			dict.put(vm, op.ToString(), op);
		}

		/// <summary>
		/// Install operators in systemdict </summary>
		/// <param name="names"> the names of the operators </param>
		/// <param name="clazz"> the implementing class </param>
		public virtual void installOp(string[] names, Type clazz)
		{
			installOp(names, clazz, systemdict);
		}

		/// <summary>
		/// Install operators into dict </summary>
		/// <param name="names"> the names of the operators </param>
		/// <param name="clazz"> the implementing class </param>
		/// <param name="dict"> the dictionary to install operators into </param>
		public virtual void installOp(string[] names, Type clazz, DictType dict)
		{
			for (int i = 0; i < names.Length; i++)
			{
				try
				{
					dict.put(vm, names[i], new ReflectionOperator(names[i], clazz));
				}
				catch (Exception ex)
				{
					System.Console.Error.WriteLine("Interpreter.installOp(" + names[i] + ") " + ex);
				}
			}
		}

		/// <returns> the status dictionary </returns>
		public virtual DictType StatusDict
		{
			get
			{
				return (DictType) systemdict.get("statusdict");
			}
		}

		/// <returns> the status dictionary </returns>
		public virtual DictType Resources
		{
			get
			{
				return resources;
			}
		}

		/// <summary>
		/// Interrupt the execution as soon as possible. </summary>
		/// <param name="state"> if true halt else clear pending interrupt </param>
		public virtual void interrupt(bool state)
		{
			estack.interrupt(state);
		}

		/// <returns> the time we are running im milliseconds </returns>
		public virtual int usertime()
		{
			return (int)(DateTimeHelper.CurrentUnixTimeMillis() - basetime);
		}

		/// <summary>
		/// Run until the execution stack is empty.
		/// </summary>
		public virtual void run()
		{
			estack.run(this);
		}

		/// <summary>
		/// Run a file. </summary>
		/// <param name="filename"> the file to run </param>
		public virtual void run(string filename)
		{
			ostack.pushRef(new StringType(vm, filename));
			estack.push(systemdict.get("run"));
			estack.run(this);
		}

		/// <summary>
		/// Load object in the context of the dict stack
		/// and return the object if found, null otherwise. </summary>
		/// <param name="name"> the name of the object to look up </param>
		/// <returns> the value bound to name </returns>
		public virtual Any load(string name)
		{
			return dstack.load(name);
		}

		/// <summary>
		/// Execute the string parameter. </summary>
		/// <param name="s"> some ps commands </param>
		public virtual void exec(string s)
		{
			estack.run(this, (new StringType(vm, s)).cvx());
		}

		/// <summary>
		/// Execute the image source procedure. </summary>
		/// <returns> the result of the image procedure </returns>
		public virtual CharStream getImageData(object proc)
		{
			estack.run(this, (ArrayType) proc);
			return (CharStream) ostack.pop(Types_Fields.STRING);
		}

		/// <summary>
		/// Parse input for next token. </summary>
		/// <param name="scanner"> the scanner object. </param>
		/// <param name="execute"> flag which indicates if we do "token" or "exec". </param>
		/// <returns> token. </returns>
		internal virtual int scan(CharSequenceType cs, Scanner scanner, bool execute)
		{
			int token;
			bool defered = false;
			do
			{
				this.lineno = cs.LineNo;
				token = scanner.token(cs);
				defered = scanner.defered();
				switch (token)
				{
				case Scanner.EOF:
					if (defered)
					{
						throw new Stop(Stoppable_Fields.SYNTAXERROR, "scan");
					}
					break;
				case Scanner.LITERAL:
					pushLiteral(scanner);
					break;
				case Scanner.IDENT:
					pushIdent(scanner, defered || !execute);
					break;
				case Scanner.IMMEDIATE:
					immediateToken(scanner);
					break;
				case Scanner.PROC_BEGIN:
					ostack.pushRef(new MarkType());
					break;
				case Scanner.PROC_END:
					pushProc(ostack);
					break;
				case Scanner.NUMBER:
					pushNumber(scanner.Number);
					break;
				case Scanner.STRING:
					pushString(scanner);
					break;
				default:
					throw new Stop(Stoppable_Fields.INTERNALERROR, "error in scanner -> " + token);
				}
			} while (defered);
			return token;
		}

		/// <summary>
		/// Push a literal onto the stack.
		/// </summary>
		private void pushLiteral(Scanner scanner)
		{
			NameType name = new NameType(scanner.String);
			name.LineNo = lineno;
			ostack.pushRef(name);
		}

		/// <summary>
		/// Push an identifier onto the stack. </summary>
		/// <param name="mode"> the execution mode </param>
		private void pushIdent(Scanner scanner, bool deferred)
		{
			Any name = (new NameType(scanner.String)).cvx();
			name.LineNo = lineno;
			if (deferred)
			{
				ostack.pushRef(name);
			}
			else
			{
				estack.pushRef(name);
			}
		}

		/// <summary>
		/// Make a procedure from objects on the stack. </summary>
		/// <param name="stack"> the stack which holds the elements for procedure. </param>
		private void pushProc(System.Collections.Stack stack)
		{
			int n = stack.counttomark();
			ArrayType array = new ArrayType(vm, n, stack);
			array.Packed = arraypacking;
			array.LineNo = lineno;
			stack.remove(n + 1);
			ostack.pushRef(array.cvx());
		}

		/// <summary>
		/// Immediatly load a token.
		/// </summary>
		private void immediateToken(Scanner scanner)
		{
			NameType key = new NameType(scanner.String);
			Any val = dstack.load(key);
			if (val == null)
			{
				throw new Stop(Stoppable_Fields.UNDEFINED, key.ToString());
			}
			val.LineNo = lineno;
			ostack.push(val);
		}

		/// <summary>
		/// Push a NumberType onto operand stack. </summary>
		/// <param name="val"> the number to push. </param>
		private void pushNumber(Number val)
		{
			NumberType num;
			if (val is int?)
			{
				num = new IntegerType(val.intValue());
			}
			else
			{
				num = new RealType(val.doubleValue());
			}
			num.LineNo = lineno;
			ostack.pushRef(num);
		}

		/// <summary>
		/// Push a string onto operand stack.
		/// </summary>
		private void pushString(Scanner scanner)
		{
			StringType s = new StringType(vm, scanner.String);
			s.LineNo = lineno;
			ostack.pushRef(s);
		}

	}

}