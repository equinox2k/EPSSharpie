using EPSSharpie.PostScript.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using static EPSSharpie.PostScript.Objects.ObjectBase;

namespace EPSSharpie.PostScript
{
    public enum ScriptMode
    {
        Standalone,
        Embedded,
    }

    public class Interpreter
    {

        public ScriptMode ScriptMode { get; private set; }

        internal Stack<ObjectBase> OperandStack { get; private set; }

        internal Dictionary<string, Action> _systemDictionary;

        private Action DictLookup(NameObject nameObject)
        {
            var name = nameObject.Value;
            if (_systemDictionary.ContainsKey(name))
            {
                return _systemDictionary[name];
            }
            System.Diagnostics.Debug.WriteLine($"Unrecognised command {name}");
            return null;
        }

        public Interpreter()
        {
            OperandStack = new Stack<ObjectBase>();
            var builtIns = new BuiltIns();
            _systemDictionary = builtIns.CreateDictionary(this);
        }

        public Interpreter(ScriptMode scriptMode)
        {
            ScriptMode = scriptMode;
            OperandStack = new Stack<ObjectBase>();
            var builtIns = new BuiltIns();
            _systemDictionary = builtIns.CreateDictionary(this);
        }

        public void Load(string input)
        {
            Parser parser = new Parser(input);
            parser.OnComment += Parser_OnComment;
            parser.OnToken += Parser_OnToken;
            parser.OnXMP += Parser_OnXMP;
            parser.Process();
            var a = 1;
            //while (parser.GetObject(out var objectBase))
            //{
            //    if (!objectBase.IsExecutable())
            //    {
            //        OperandStack.Push(objectBase);
            //    }
            //    else if (objectBase is NameObject)
            //    {
            //        var value = DictLookup(objectBase as NameObject);
            //        value?.Invoke();
            //    }                
            //}
        }

        private void Parser_OnXMP(string data)
        {
            //Console.WriteLine($"XMP - {data}");
        }

        private void Parser_OnToken(Parser.Mode mode, string token)
        {
            Console.WriteLine($"{mode} - {token}");
        }

        private void Parser_OnComment(CommentType commentType, string comment)
        {
            //Console.WriteLine($"{commentType} - {comment}");
        }
    }
}
