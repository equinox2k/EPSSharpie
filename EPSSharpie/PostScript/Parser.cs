using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EPSSharpie.PostScript.Objects;

namespace EPSSharpie.PostScript
{
    class Parser
    {
        private enum Mode
        {
            None,
            Integer,
            String,
            Real,
            Name,
            Array,
            Comment,
            Error
        }

        private bool IsComment(char c)
        {
            return c == '%';
        }

        private bool IsNewline(char c)
        {
            return c == '\n';
        }

        private bool IsDigit(char c)
        {
            const string digits = "0123456789";
            return digits.Contains(c.ToString());
        }

        private bool IsWhitespace(char c)
        {
            return c == ' ';
        }

        private bool IsSign(char c)
        {
            return c == '+' || c == '-';
        }

        private CharReader _input;

        public Parser(CharReader input)
        {
            _input = input;            
        }

        public bool GetObject(out ObjectBase objectBase)
        {
            Mode mode = Mode.None;
            ObjectBase result = null;

            var buffer = string.Empty;
            bool finished = false;
            while (!finished && _input.ReadChar(out var c))
            {
                if (c == '\r')
                {
                    continue;
                }
                if (mode == Mode.Comment)
                {
                    if (IsNewline(c))
                    {
                        mode = Mode.None;
                    }
                    continue;
                }
                if (IsWhitespace(c) || IsNewline(c))
                {
                    if (mode != Mode.None)
                    {
                        finished = true;
                    }
                    continue;
                }
                if (mode != Mode.String && IsComment(c))
                {
                    mode = Mode.Comment;
                    continue;
                }
                buffer += c;

                switch (mode)
                {
                    case Mode.None:
                        if (IsDigit(c) || IsSign(c))
                        {
                            mode = Mode.Integer;
                        }
                        else
                        {
                            mode = Mode.Name;
                        }
                        break;
                    case Mode.Integer:
                        if (c == 'E' || c == '.')
                        {
                            mode = Mode.Real;
                        }
                        else if (!IsDigit(c))
                        {
                            mode = Mode.Name;
                        }
                        break;
                    case Mode.Real:
                        if (c == 'E' || c == '.')
                        {
                            mode = Mode.Name;
                        }
                        break;
                    case Mode.String:
                        break;
                    case Mode.Array:
                        break;
                    default:
                        break;
                }
            }

            switch (mode)
            {
                case Mode.Name:
                    result = new NameObject(buffer);
                    break;
                case Mode.Integer:
                    result = new NumericalObject(int.Parse(buffer));
                    break;
                case Mode.Real:
                    result = new NumericalObject(double.Parse(buffer));
                    break;
            }
            
            objectBase = result;

            return _input.Position < _input.Length;
        }               
    }
}
