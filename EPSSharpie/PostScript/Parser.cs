using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EPSSharpie.PostScript.Objects;

namespace EPSSharpie.PostScript
{
    public enum CommentType
    {
        Header,
        DSCComment,
        Comment
    }

    class Parser
    {
        public delegate void CommentEvent(CommentType commentType, string comment);
        public event CommentEvent OnComment;

        public delegate void TokenEvent(Mode mode, string token);
        public event TokenEvent OnToken;

        public delegate void XMPEvent(string data);
        public event XMPEvent OnXMP;

        public enum Mode
        {
            None,
            Token,
            BeginArray,
            EndArray,
            BeginProcedure,
            EndProcedure,

            Integer,
            String,
            EscapeString,
            HexString,
            Real,
            Name
           
        }

        private Mode _processMode;
        private StringBuilder _stringBuilder;

        private bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t';
        }

        private bool IsBeginArray(char c)
        {
            return c == '[';
        }

        private bool IsEndArray(char c)
        {
            return c == ']';
        }

        private bool IsBeginProcedure(char c)
        {
            return c == '{';
        }

        private bool IsEndProcedure(char c)
        {
            return c == '}';
        }

        private bool IsBeginString(char c)
        {
            return c == '(';
        }

        private bool IsEndString(char c)
        {
            return c == ')';
        }



        private bool IsDigit(char c)
        {
            const string digits = "0123456789";
            return digits.Contains(c.ToString());
        }

        private bool IsOctalDigit(char c)
        {
            const string digits = "01234567";
            return digits.Contains(c.ToString());
        }

        private bool IsHexDigit(char c)
        {
            const string digits = "0123456789ABCDEF";
            return digits.Contains(c.ToString().ToUpper());
        }


        private bool IsSign(char c)
        {
            return c == '+' || c == '-';
        }

        private readonly string _input;

        public Parser(string input)
        {
            _input = input;            
        }

        public void Process()
        {
            _processMode = Mode.None;
            ProcessBuffer(new CharReader(_input));
        }

        public void Processx()
        {
            bool xpacket = false;
            var xpacketBuffer = new StringBuilder();

            _processMode = Mode.None;
            _stringBuilder = new StringBuilder();
            using (var stringReader = new StringReader(_input))
            {
                var continueProcessing = true;
                var line = stringReader.ReadLine();
                while (line != null && continueProcessing == true)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        if (line.StartsWith("%"))
                        {
                            ProcessComment(line.Substring(1));
                            continue;
                        }

                        if (line.StartsWith("<?xpacket begin="))
                        {
                            xpacket = true;
                            continue;
                        }
                        else if (line.StartsWith("<?xpacket end="))
                        {
                            xpacket = false;
                            OnXMP?.Invoke(xpacketBuffer.ToString());
                            xpacketBuffer.Clear();
                            continue;
                        }

                        if (xpacket)
                        {
                            xpacketBuffer.AppendLine(line);
                        }
                        else
                        {
            
                        }
                    }
                    finally
                    {
                        line = stringReader.ReadLine();
                    }
                }
            }            
        }

        private void ProcessBuffer()
        {
            if (_stringBuilder.Length > 0)
            {
                OnToken?.Invoke(Mode.Token, _stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }

        private string GetOctalString(CharReader charReader, int numDigits)
        {
            var textBuilder = new StringBuilder();
            for (var i = 0; i < numDigits; i++)
            {
                var character = charReader.ReadChar();
                if (IsOctalDigit(character))
                {
                    textBuilder.Append(character);
                }
            }
            return textBuilder.ToString();
        }
        
        private bool ProcessString(CharReader charReader)
        {
            if (charReader.PeekChar() != '(')
            {
                return false;
            }
            charReader.ReadChar();
            var textBuilder = new StringBuilder();
            while (true)
            {
                var character = charReader.ReadChar();
                if (character == '\\')
                {
                    var peekedChar = charReader.PeekChar();
                    if (IsOctalDigit(peekedChar))
                    {
                        var octal = GetOctalString(charReader, 3);
                        var value = Convert.ToInt32(octal, 8);
                        if (value > 255)
                        {
                            throw new Exception("Invalid octal charcter.");
                        }
                        textBuilder.Append(Convert.ToChar((byte)value));
                        continue;
                    }
                    if (peekedChar == '\\' || peekedChar == '(' || peekedChar == ')')
                    {
                        textBuilder.Append(charReader.ReadChar());
                        continue;
                    }
                    if (peekedChar == 'n')
                    {
                        charReader.ReadChar();
                        textBuilder.Append('\n');
                        continue;
                    }
                    if (peekedChar == 'r')
                    {
                        charReader.ReadChar();
                        textBuilder.Append('\r');
                        continue;
                    }
                    if (peekedChar == 't')
                    {
                        charReader.ReadChar();
                        textBuilder.Append('\r');
                        continue;
                    }
                    if (peekedChar == 'b')
                    {
                        charReader.ReadChar();
                        textBuilder.Append('\b');
                        continue;
                    }
                    if (peekedChar == 'f')
                    {
                        charReader.ReadChar();
                        textBuilder.Append('\f');
                        continue;
                    }
                    throw new Exception("Unexpected escape code.");
                }
                else if(character == ')')
                {
                    OnToken?.Invoke(Mode.String, textBuilder.ToString());
                    return true;
                }
                else if (character == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                textBuilder.Append(charReader.ReadChar());
            }
        }

        private bool ProcessHexString(CharReader charReader)
        {
            if (charReader.PeekChar() != '<')
            {
                return false;
            }
            charReader.ReadChar();
            var textBuilder = new StringBuilder();
            while (true)
            {
                var character = charReader.ReadChar();
                if (character == '>')
                {
                    OnToken?.Invoke(Mode.String, textBuilder.ToString());
                    return true;
                }
                else if (character == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                else if (IsHexDigit(character))
                {
                    textBuilder.Append(charReader.ReadChar());
                }
                else
                {
                    throw new Exception("Invalid hex charcter.");
                }
            }
        }

        private bool ProcessArray(CharReader charReader)
        {
            if (charReader.PeekChar() != '[')
            {
                return false;
            }
            while (true)
            {
                OnToken?.Invoke(Mode.BeginArray, "");
                var peekedChar = charReader.PeekChar();
                if (peekedChar == ']')
                {
                    charReader.ReadChar();
                    OnToken?.Invoke(Mode.EndArray, "");
                    return true;
                }
                else if (peekedChar == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                else
                {
                    ProcessBuffer(charReader);
                }
            }
        }

        private bool ProcessProcedure(CharReader charReader)
        {
            if (charReader.PeekChar() != '{')
            {
                return false;
            }
            while (true)
            {
                OnToken?.Invoke(Mode.BeginProcedure, "");
                var peekedChar = charReader.PeekChar();
                if (peekedChar == '}')
                {
                    charReader.ReadChar();
                    OnToken?.Invoke(Mode.EndProcedure, "");
                    return true;
                }
                else if (peekedChar == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                else
                {
                    ProcessBuffer(charReader);
                }
            }
        }

        private bool ProcessComment(CharReader charReader)
        {
            if (charReader.PeekChar() != '%')
            {
                return false;
            }
            var textBuilder = new StringBuilder();
            while (true)
            {
                var peekedChar = charReader.PeekChar();
                if (peekedChar == '\r')
                {
                    charReader.ReadChar();
                    OnToken?.Invoke(Mode.None, textBuilder.ToString());
                    return true;
                }
                else if (peekedChar == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                else
                {
                    textBuilder.Append(charReader.ReadChar());
                }
            }
        }

        private void ProcessBuffer(CharReader charReader)
        {
            while (charReader.PeekChar() != '\0')
            {

                if (ProcessComment(charReader))
                {
                    continue;
                }
                else if (ProcessString(charReader))
                {
                    continue;
                }
                else if (ProcessHexString(charReader))
                {
                    continue;
                }
                else if (ProcessProcedure(charReader))
                {
                    continue;
                }
                else if (ProcessArray(charReader))
                {
                    continue;
                }
                else
                {
                    charReader.ReadChar();
                }



                //if (IsWhitespace(character))
                //{
                //    ProcessBuffer(ref continueProcessing);
                //}
                //else if (IsBeginArray(character))
                //{
                //    ProcessBuffer(ref continueProcessing);
                //    if (continueProcessing)
                //    {
                //        OnToken?.Invoke(Mode.BeginArray, _stringBuilder.ToString(), ref continueProcessing);
                //    }
                //}
                //else if (IsEndArray(character))
                //{
                //    ProcessBuffer(ref continueProcessing);
                //    if (continueProcessing)
                //    {
                //        OnToken?.Invoke(Mode.EndArray, _stringBuilder.ToString(), ref continueProcessing);
                //    }
                //}
                //else if (IsBeginProcedure(character))
                //{
                //    ProcessBuffer(ref continueProcessing);
                //    if (continueProcessing)
                //    {
                //        OnToken?.Invoke(Mode.BeginProcedure, _stringBuilder.ToString(), ref continueProcessing);
                //    }
                //}
                //else if (IsEndProcedure(character))
                //{
                //    ProcessBuffer(ref continueProcessing);
                //    if (continueProcessing)
                //    {
                //        OnToken?.Invoke(Mode.EndProcedure, _stringBuilder.ToString(), ref continueProcessing);
                //    }
                //}
                //else
                //{
                //    //_stringBuilder.Append(character);
                //}

            }
        }

        private void ProcessComment(string comment)
        {
            if (comment.StartsWith("!"))
            {
                OnComment?.Invoke(CommentType.Header, comment.Substring(1));
            }
            else if (comment.StartsWith("%"))
            {
                OnComment?.Invoke(CommentType.DSCComment, comment.Substring(1));
            }
            else
            {
                OnComment?.Invoke(CommentType.Comment, comment);
            }
        }

        public bool GetObject(out ObjectBase objectBase)
        {
            //Mode mode = Mode.None;
            //ObjectBase result = null;

            //var buffer = string.Empty;
            //bool finished = false;
            //while (!finished && _input.ReadChar(out var c))
            //{
            //    if (c == '\r')
            //    {
            //        continue;
            //    }
            //    if (mode == Mode.Comment)
            //    {
            //        if (IsNewline(c))
            //        {
            //            mode = Mode.None;
            //        }
            //        continue;
            //    }
            //    if (IsWhitespace(c) || IsNewline(c))
            //    {
            //        if (mode != Mode.None)
            //        {
            //            finished = true;
            //        }
            //        continue;
            //    }
            //    if (mode != Mode.String && IsComment(c))
            //    {
            //        mode = Mode.Comment;
            //        continue;
            //    }
            //    buffer += c;

            //    switch (mode)
            //    {
            //        case Mode.None:
            //            if (IsDigit(c) || IsSign(c))
            //            {
            //                mode = Mode.Integer;
            //            }
            //            else
            //            {
            //                mode = Mode.Name;
            //            }
            //            break;
            //        case Mode.Integer:
            //            if (c == 'E' || c == '.')
            //            {
            //                mode = Mode.Real;
            //            }
            //            else if (!IsDigit(c))
            //            {
            //                mode = Mode.Name;
            //            }
            //            break;
            //        case Mode.Real:
            //            if (c == 'E' || c == '.')
            //            {
            //                mode = Mode.Name;
            //            }
            //            break;
            //        case Mode.String:
            //            break;
            //        case Mode.Array:
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //switch (mode)
            //{
            //    case Mode.Name:
            //        result = new NameObject(buffer);
            //        break;
            //    case Mode.Integer:
            //        result = new NumericalObject(int.Parse(buffer));
            //        break;
            //    case Mode.Real:
            //        result = new NumericalObject(double.Parse(buffer));
            //        break;
            //}

            //objectBase = result;

            //return _input.Position < _input.Length;
            objectBase = null;
            return true;
        }               
    }
}
