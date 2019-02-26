using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EPSSharpie.PostScript.Objects;

namespace EPSSharpie.PostScript
{

    class Parser
    {

        public delegate void TokenEvent(Mode mode, string token);
        public event TokenEvent OnToken;

        public enum Mode
        {
            None,
            Token,
            BeginMark,
            EndMark,
            BeginArray,
            EndArray,
            BeginProcedure,
            EndProcedure,
            XPacket,
            Comment,

            Integer,
            String,
            HexString,
            Real,
            Name
           
        }

        private Mode _processMode;
        private StringBuilder _stringBuilder;

        private bool IsWhitespace(char c)
        {
            return c == '\0' || c == '\t' || c == '\r' || c == '\n' || c == '\f' || c == ' ';
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

        private readonly byte[] _input;

        public Parser(byte[] input)
        {
            _input = input;            
        }

        public void Process()
        {
            _processMode = Mode.None;
            _stringBuilder = new StringBuilder();
            ProcessBuffer(new CharReader(_input));
        }

     

        private void FlushTokenBuffer()
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
            FlushTokenBuffer();
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
                        textBuilder.Append('\t');
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
                textBuilder.Append(character);
            }
        }

        private bool ProcessHexString(CharReader charReader)
        {
            if (charReader.PeekChar() != '<')
            {
                return false;
            }
            FlushTokenBuffer();
            charReader.ReadChar();
            var textBuilder = new StringBuilder();
            while (true)
            {
                var character = charReader.ReadChar();
                if (character == '>')
                {
                    OnToken?.Invoke(Mode.HexString, textBuilder.ToString());
                    return true;
                }
                else if (character == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                else if (IsHexDigit(character))
                {
                    textBuilder.Append(character);
                }
                else if (IsWhitespace(character))
                {
                    // Skip new line
                }
                else
                {
                    throw new Exception("Invalid hex charcter.");
                }
            }
        }

        private bool ProcessXPacket(CharReader charReader)
        {
            if (!charReader.PeekLine().Equals("<?xpacket begin=\"ï»¿\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>"))
            {
                return false;
            }
            FlushTokenBuffer();
            charReader.ReadLine();
            var textBuilder = new StringBuilder();
            while (true)
            {
                var line = charReader.ReadLine();
                if (line.Equals("<?xpacket end=\"w\"?>"))
                {
                    OnToken?.Invoke(Mode.XPacket, textBuilder.ToString());
                    return true;
                }
                else if (!string.IsNullOrEmpty(line))
                {
                    textBuilder.AppendLine(line);
                }           
            }        
        }

        private bool ProcessMark(CharReader charReader)
        {
            if (charReader.PeekString(2) == "<<")
            {
                FlushTokenBuffer();
                charReader.ReadString(2);
                OnToken?.Invoke(Mode.BeginArray, "");
                return true;
            }
            if (charReader.PeekString(2) == ">>")
            {
                FlushTokenBuffer();
                charReader.ReadString(2);
                OnToken?.Invoke(Mode.EndArray, "");
                return true;
            }
            return false;
        }

        private bool ProcessArray(CharReader charReader)
        {
            if (charReader.PeekChar() == '[')
            {
                FlushTokenBuffer();
                charReader.ReadChar();
                OnToken?.Invoke(Mode.BeginArray, "");
                return true;
            }
            if (charReader.PeekChar() == ']')
            {
                FlushTokenBuffer();
                charReader.ReadChar();
                OnToken?.Invoke(Mode.EndArray, "");
                return true;
            }
            return false;
        }

        private bool ProcessProcedure(CharReader charReader)
        {
            if (charReader.PeekChar() == '{')
            {
                FlushTokenBuffer();
                charReader.ReadChar();
                OnToken?.Invoke(Mode.BeginProcedure, "");
                return true;
            }
            if (charReader.PeekChar() == '}')
            {
                FlushTokenBuffer();
                charReader.ReadChar();
                OnToken?.Invoke(Mode.EndProcedure, "");
                return true;
            }
            return false;
        }

        private bool ProcessComment(CharReader charReader)
        {
            if (!charReader.PeekLine().StartsWith("%"))
            {
                return false;
            }
            FlushTokenBuffer();
            OnToken?.Invoke(Mode.Comment, charReader.ReadLine().Substring(1));
            return true;
        }

        private void ProcessBuffer(CharReader charReader)
        {
            var test = string.Empty;
            while (charReader.PeekChar() != '\0')
            {

                if (ProcessComment(charReader))
                {
                    continue;
                }
                else if (ProcessXPacket(charReader))
                {
                    continue;
                }
                else if (ProcessMark(charReader))
                {
                    continue;
                }
                else if (ProcessArray(charReader))
                {
                    continue;
                }
                else if (ProcessProcedure(charReader))
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
                else
                {
                    var character = charReader.ReadChar();
                    if (!IsWhitespace(character))
                    {
                        _stringBuilder.Append(character);
                    }
                    else
                    {
                        FlushTokenBuffer();
                    }
                }
            }
            FlushTokenBuffer();
        }


             
    }
}
