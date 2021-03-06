﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EPSSharpie.PostScript.Objects;

namespace EPSSharpie.PostScript
{
    public enum TokenType
    {
        None,
        BeginDictionary,
        EndDictionary,
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

    class Parser
    {

        public delegate void TokenEvent(TokenType tokenType, string token);
        public event TokenEvent OnToken;      
        
        private StringBuilder _stringBuilder;

        static public long BaseDecode(string value, int numBase)
        {
            const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            var result = 0L;
            foreach (char character in value)
            {
                result = result * numBase + CharList.IndexOf(character.ToString(), StringComparison.CurrentCultureIgnoreCase);
            }
            return result;
        }

        private bool IsWhitespace(char character)
        {
            return character == '\0' || character == '\t' || character == '\r' || character == '\n' || character == '\f' || character == ' ';
        }

        private bool IsBaseX(char character, int numBase)
        {
            const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            var index = CharList.IndexOf(character.ToString(), StringComparison.CurrentCultureIgnoreCase);
            return index >= 0 && index < numBase;
        }



        private readonly byte[] _input;

        public Parser(byte[] input)
        {
            _input = input;            
        }

        public void Process()
        {
            _stringBuilder = new StringBuilder();
            ProcessBuffer(new CharReader(_input));
        }

     

        private void FlushTokenBuffer()
        {
            if (_stringBuilder.Length > 0)
            {
                var value = _stringBuilder.ToString();
                if (long.TryParse(value, out _))
                {
                    OnToken?.Invoke(TokenType.Integer, value);
                }
                else if (float.TryParse(value, out _))
                {
                    OnToken?.Invoke(TokenType.Real, value);
                }
                else
                {
                    OnToken?.Invoke(TokenType.Name, value);
                }
                _stringBuilder.Clear();
            }
        }

        private string GetOctalString(CharReader charReader)
        {
            var bytes = new List<byte>();
            var textBuilder = new StringBuilder();
            while (true)
            {
                var peekedChar = charReader.PeekChar();
                if (IsBaseX(peekedChar, 8))
                {
                    textBuilder.Append(charReader.ReadChar());
                    if (textBuilder.Length == 3)
                    {
                        bytes.Add(Convert.ToByte(textBuilder.ToString(), 8));
                        textBuilder.Clear();
                    }
                }
                else
                {
                    break;
                }
            }
            if (textBuilder.Length > 0)
            {
                bytes.Add(Convert.ToByte(textBuilder.ToString(), 8));
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        
        private bool ProcessString(CharReader charReader)
        {
            if (charReader.PeekChar() != '(')
            {
                return false;
            }
            int bracketCount = 1;
            FlushTokenBuffer();
            charReader.ReadChar();
            var textBuilder = new StringBuilder();
            while (true)
            {
                var character = charReader.ReadChar();
                if (character == '\\')
                {
                    var peekedChar = charReader.PeekChar();
                    if (IsBaseX(peekedChar, 8))
                    {
                        var octalString = GetOctalString(charReader);
                        textBuilder.Append(octalString);
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
                else if (character == '(')
                {
                    bracketCount++;
                }
                else if (character == ')' && bracketCount > 1)
                {
                    bracketCount--;
                }
                else if (character == ')' && bracketCount == 1)
                {
                    OnToken?.Invoke(TokenType.String, textBuilder.ToString());
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
                    OnToken?.Invoke(TokenType.HexString, textBuilder.ToString());
                    return true;
                }
                else if (character == '\0')
                {
                    throw new Exception("Unexpected end of data.");
                }
                else if (IsBaseX(character, 16))
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
                    OnToken?.Invoke(TokenType.XPacket, textBuilder.ToString());
                    return true;
                }
                else if (!string.IsNullOrEmpty(line))
                {
                    textBuilder.AppendLine(line);
                }           
            }        
        }

        private bool ProcessDictionary(CharReader charReader)
        {
            if (charReader.PeekString(2) == "<<")
            {
                FlushTokenBuffer();
                charReader.ReadString(2);
                OnToken?.Invoke(TokenType.BeginDictionary, "");
                return true;
            }
            if (charReader.PeekString(2) == ">>")
            {
                FlushTokenBuffer();
                charReader.ReadString(2);
                OnToken?.Invoke(TokenType.EndDictionary, "");
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
                OnToken?.Invoke(TokenType.BeginArray, "");
                return true;
            }
            if (charReader.PeekChar() == ']')
            {
                FlushTokenBuffer();
                charReader.ReadChar();
                OnToken?.Invoke(TokenType.EndArray, "");
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
                OnToken?.Invoke(TokenType.BeginProcedure, "");
                return true;
            }
            if (charReader.PeekChar() == '}')
            {
                FlushTokenBuffer();
                charReader.ReadChar();
                OnToken?.Invoke(TokenType.EndProcedure, "");
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
            OnToken?.Invoke(TokenType.Comment, charReader.ReadLine().Substring(1));
            return true;
        }

        private void ProcessBuffer(CharReader charReader)
        {
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
                else if (ProcessDictionary(charReader))
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
