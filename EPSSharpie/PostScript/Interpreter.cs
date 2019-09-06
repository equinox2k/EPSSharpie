using EPSSharpie.PostScript.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EPSSharpie.PostScript
{
    public class Interpreter
    {


        private StringBuilder _log;

        public List<ObjectBase> OperandStack { get; }
        public Dictionary<string, ObjectBase> DictionaryStack { get; }
        public List<ObjectBase> ExecutionStack { get; }

        private int _procedureCount = 0;

        public Dictionary<string, OperandObject> CreateSystemDictionary()
        {
            var systemDictionary = new Dictionary<string, OperandObject>();

            systemDictionary.Add("gt", new OperandObject(() =>
            {
                var second = OperandStack.Pop() as NumericalObject;
                var first = OperandStack.Pop() as NumericalObject;
                OperandStack.Push(new BooleanObject(first.Value > second.Value));
            }));

            systemDictionary.Add("ifelse", new OperandObject(() =>
            {
                var third = OperandStack.Pop() as ProcedureObject;
                var second = OperandStack.Pop() as ProcedureObject;
                var first = OperandStack.Pop() as BooleanObject;                
                ExecutionStack.Push(first.Value ? second : third);                
            }));

            systemDictionary.Add("pop", new OperandObject(() =>
            {
                OperandStack.Pop();
            }));

            systemDictionary.Add("exch", new OperandObject(() =>
            {
                var second = OperandStack.Pop();
                var first = OperandStack.Pop();
                OperandStack.Push(second);
                OperandStack.Push(first);
            }));

            systemDictionary.Add("dup", new OperandObject(() =>
            {
                OperandStack.Push(OperandStack.Top());
            }));

            systemDictionary.Add("copy", new OperandObject(() =>
            {
                var count = OperandStack.Pop() as NumericalObject;
                var objects = OperandStack.Pop((int)count.Value);
                objects = objects.Reverse().ToArray();
                OperandStack.Push(objects);
                OperandStack.Push(objects);
                //TODO not accurate to spec
            }));

            systemDictionary.Add("clear", new OperandObject(() =>
            {
                OperandStack.Clear();
            }));

            systemDictionary.Add("count", new OperandObject(() =>
            {
                var item = new NumericalObject(OperandStack.Count);
                OperandStack.Push(item);
            }));

            systemDictionary.Add("def", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as ProcedureObject;
                var second = OperandStack.Pop() as LiteralName;

                if (DictionaryStack.ContainsKey(second.Value))
                {
                    DictionaryStack[second.Value] = first;
                }
                else
                {
                    DictionaryStack.Add(second.Value, first);
                }
            }));

            systemDictionary.Add("add", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var second = OperandStack.Pop() as NumericalObject;
                var result = second.Value + first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("div", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var second = OperandStack.Pop() as NumericalObject;
                var result = second.Value / first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("idiv", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var second = OperandStack.Pop() as NumericalObject;
                var result = (long)second.Value / (long)first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("mod", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var second = OperandStack.Pop() as NumericalObject;
                var result = second.Value % first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("mul", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var second = OperandStack.Pop() as NumericalObject;
                var result = second.Value * first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("sub", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var second = OperandStack.Pop() as NumericalObject;
                var result = second.Value + first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("abs", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var result = Math.Abs(first.Value);
                OperandStack.Push(new NumericalObject(result));
            }));

            systemDictionary.Add("neg", new OperandObject(() =>
            {
                var first = OperandStack.Pop() as NumericalObject;
                var result = -first.Value;
                OperandStack.Push(new NumericalObject(result));
            }));

            return systemDictionary;
        }



        //var Os = [];  // Operand Stack
        //var Sd = { }; // System Dictionary
        //var Ds = [Sd]; // Dictionary Stack
        //var Es = []; // Execution Stack


        public Interpreter()
        {
            OperandStack = new List<ObjectBase>();
            DictionaryStack = new Dictionary<string, ObjectBase>();
            ExecutionStack = new List<ObjectBase>();
            
            var systemDictionary = CreateSystemDictionary();
            foreach (var systemDictionaryItem in systemDictionary)
            {
                DictionaryStack.Add(systemDictionaryItem.Key, systemDictionaryItem.Value);
            }
        }

        public void Load(byte[] input)
        {
            _log = new StringBuilder();

            Parser parser = new Parser(input);
            parser.OnToken += Parser_OnToken;

            try
            {
                parser.Process();
            }
            catch
            {
                // error occured;
            }

            File.WriteAllText("C:\\Users\\ptribe.PHOTO\\Desktop\\Log.txt", _log.ToString());            
        }
        
        private void Parser_OnToken(TokenType tokenType, string value)
        {
            _log.AppendLine($"{tokenType} - {value}");

            try
            {
                if (tokenType == TokenType.Real)
                {
                    OperandStack.Push(new NumericalObject(double.Parse(value)));
                }
                else if (tokenType == TokenType.Integer)
                {
                    OperandStack.Push(new NumericalObject(double.Parse(value)));
                }
                else if (tokenType == TokenType.Name)
                {
                    if (value.StartsWith("//"))
                    {
                        var name = value.Substring(2);
                        if (!DictionaryStack.ContainsKey(name))
                        {
                            throw new Exception($"Key '{name}' not found in DictionaryStack.");
                        }
                        OperandStack.Push(DictionaryStack[name]);
                    }
                    else if (value.StartsWith("/"))
                    {
                        var name = value.Substring(1);
                        OperandStack.Push(new LiteralName(name));
                    }
                    else
                    {
                        OperandStack.Push(new ExecutableName(value));
                    }
                }
                else if (tokenType == TokenType.BeginProcedure)
                {
                    _procedureCount++;
                    OperandStack.Push(new SymbolObject(SymbolEnum.Procedure));
                }
                else if (tokenType == TokenType.EndProcedure)
                {
                    var items = new List<ObjectBase>();
                    while (true)
                    {
                        var currentOperand = OperandStack.Pop();
                        if (currentOperand is SymbolObject symbolObject)
                        {
                            if (symbolObject.Value != SymbolEnum.Procedure)
                            {
                                throw new Exception("Unexpected symbol type");
                            }
                            break;
                        }
                        items.Add(currentOperand);
                    }
                    OperandStack.Push(new ProcedureObject(items.ToArray()));
                    _procedureCount--;
                }
                else if (tokenType == TokenType.BeginArray)
                {
                    OperandStack.Push(new SymbolObject(SymbolEnum.Array));
                }
                else if (tokenType == TokenType.EndArray)
                {
                    var items = new List<ObjectBase>();
                    while (true)
                    {
                        var currentOperand = OperandStack.Pop();
                        if (currentOperand is SymbolObject symbolObject)
                        {
                            if (symbolObject.Value != SymbolEnum.Array)
                            {
                                throw new Exception("Unexpected symbol type");
                            }
                            break;
                        }
                        items.Add(currentOperand);
                    }
                    OperandStack.Push(new ArrayObject(items.ToArray()));
                }
                else if (tokenType == TokenType.BeginDictionary)
                {
                    OperandStack.Push(new SymbolObject(SymbolEnum.Dictionary));
                }
                else if (tokenType == TokenType.EndDictionary)
                {
                    var items = new Dictionary<LiteralName, ObjectBase>();
                    while (true)
                    {
                        var currentOperand = OperandStack.Pop();
                        if (currentOperand is SymbolObject symbolObject)
                        {
                            if (symbolObject.Value != SymbolEnum.Array)
                            {
                                throw new Exception("Unexpected symbol type");
                            }
                            break;
                        }
                        var literal = OperandStack.Pop() as LiteralName;
                        items.Add(literal, currentOperand);
                    }
                    OperandStack.Push(new DictionaryObject(items));
                }

                if (ShouldRun(OperandStack.Top()))
                {
                    ExecutionStack.Push(OperandStack.Pop());
                    Run();
                }
            }
            catch (Exception ex)
            {
                _log.AppendLine($"Exception: {ex}");
                throw;
            }
        }

        private bool ShouldRun(ObjectBase objectBase)
        {
            return _procedureCount <= 0 && objectBase is ExecutableName;            
        }

        private void Run()
        {
            while (ExecutionStack.Count > 0)
            {
                var objectBase = ExecutionStack.Pop();
                if (objectBase is ExecutableName executableName)
                {
                    if (!DictionaryStack.ContainsKey(executableName.Value))
                    {
                        throw new Exception("Command not found");
                    }
                    ExecutionStack.Push(DictionaryStack[executableName.Value]);
                }
                else if (objectBase is OperandObject operandObject)
                {
                    operandObject.Value();
                }
                else if (objectBase is ProcedureObject procedureObject)
                {
                    foreach (var item in procedureObject.Value)
                    {
                        ExecutionStack.Push(item);
                    }
                }
                else
                {
                    OperandStack.Push(objectBase);
                }
            }
        }

    }
}
