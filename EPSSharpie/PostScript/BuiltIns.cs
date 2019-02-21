using EPSSharpie.PostScript.Objects;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript
{
    class BuiltIns
    {
        public enum BinaryOperation
        {
            Add,
            IDivide,
            Divide,
            Modulus,
            Multiply,           
            Subtract                                   
        }

        public enum UnaryOperation
        {
            Absolute,
            Negate
        }

        private Interpreter _interpreter;
        private Dictionary<string, Action> _dictionary;

        public BuiltIns()
        {
            _dictionary = new Dictionary<string, Action>();
        }

        private void CreateOperand(string name, Action function)
        {
            _dictionary.Add(name, function);
        }

        private ObjectBase Top()
        {
            return _interpreter.OperandStack.Peek();
        }

        private ObjectBase Pop()
        {
            return _interpreter.OperandStack.Pop();
        }

        private T Pop<T>() where T : ObjectBase
        {
            return _interpreter.OperandStack.Pop() as T;
        }

        private ObjectBase[] Pop(int count)
        {
            var list = new List<ObjectBase>();
            for (var i = 0; i < count; i++)
            {
                list.Add(Pop());
            }
            return list.ToArray();
        }

        private void Push(ObjectBase objectBase)
        {
            _interpreter.OperandStack.Push(objectBase);
        }

        private void Push(ObjectBase[] objects)
        {
            foreach (var item in objects)
            {
                Push(item);
            }
        }

        private void BinaryOp(BinaryOperation operation)
        {
            double result = 0;
            var first = Pop<NumericalObject>();
            var second = Pop<NumericalObject>();

            if (operation == BinaryOperation.Add)
            {
                result = second.Double + first.Double;
            }
            else if (operation == BinaryOperation.IDivide)
            {
                result = second.Integer / first.Integer;
            }
            else if (operation == BinaryOperation.Divide)
            {
                result = second.Double / first.Double;
            }
            else if (operation == BinaryOperation.Modulus)
            {
                result = second.Double % first.Double;
            }
            else if (operation == BinaryOperation.Multiply)
            {
                result = second.Double * first.Double;
            }
            else if (operation == BinaryOperation.Subtract)
            {
                result = second.Double - first.Double;
            }

            if (operation == BinaryOperation.IDivide || (first.NumericalType == NumericalType.Integer && second.NumericalType == NumericalType.Integer))
            {
                Push(new NumericalObject((int)result)); 
            }
            else
            {
                Push(new NumericalObject(result));
            }
        }

        private void UnaryOp(UnaryOperation operation)
        {
            double result = 0;
            var first = Pop<NumericalObject>();

            if (operation == UnaryOperation.Absolute)
            {
                result = Math.Abs(first.Double);
            }
            else if (operation == UnaryOperation.Negate)
            {
                result = - first.Double;
            }

            if (first.NumericalType == NumericalType.Integer)
            {
                Push(new NumericalObject((int)result));
            }
            else
            {
                Push(new NumericalObject(result));
            }
        }

        public Dictionary<string, Action> CreateDictionary(Interpreter interpreter)
        {
            _interpreter = interpreter;

            CreateOperand("pop", () =>
            {
                Pop();
            });

            CreateOperand("exch", () =>
            {
                var first = Pop();
                var second = Pop();
                Push(first);
                Push(second);
            });

            CreateOperand("dup", () =>
            {
                Push(Top());
            });

            CreateOperand("copy", () =>
            {
                var numericalObject = Pop<NumericalObject>();
                var objects = Pop(numericalObject.Integer);
                objects = objects.Reverse().ToArray();
                Push(objects);
                Push(objects);
            });

            CreateOperand("clear", () =>
            {
                _interpreter.OperandStack.Clear();
            });

            CreateOperand("count", () =>
            {
                var item = new NumericalObject(_interpreter.OperandStack.Count);
                _interpreter.OperandStack.Push(item);
            });

            CreateOperand("add", () =>
            {
                BinaryOp(BinaryOperation.Add);
            });

            CreateOperand("div", () =>
            {
                BinaryOp(BinaryOperation.Divide);
            });

            CreateOperand("idiv", () =>
            {
                BinaryOp(BinaryOperation.IDivide);
            });

            CreateOperand("mod", () =>
            {
                BinaryOp(BinaryOperation.Modulus);
            });

            CreateOperand("mul", () =>
            {
                BinaryOp(BinaryOperation.Multiply);
            });

            CreateOperand("sub", () =>
            {
                BinaryOp(BinaryOperation.Subtract);
            });

            CreateOperand("abs", () =>
            {
                UnaryOp(UnaryOperation.Absolute);
            });

            CreateOperand("neg", () =>
            {
                UnaryOp(UnaryOperation.Negate);
            });

            return _dictionary;
        }
    }
}
