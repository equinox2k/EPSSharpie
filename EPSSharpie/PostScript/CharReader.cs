using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript
{
    class CharReader
    {

        private readonly byte[] _input;

        public int Position { get; set; }

        public int Length => _input.Length;

        public CharReader(byte[] input)
        {
            _input = input;
            Position = 0;
        }
        
        private string PeekString(int position, int count)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                if (position < Length)
                {
                    stringBuilder.Append((char)_input[position]);
                    position++;
                }
                else
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        public string PeekString(int count)
        {
            var tempPosition = Position;
            return PeekString(tempPosition, count);
        }

        public string PeekLine()
        {
            var tempPosition = Position;
            var stringBuilder = new StringBuilder();
            while (true)
            {
                if (tempPosition < Length)
                {
                    if (PeekString(tempPosition, 2) == "\r\n")
                    {
                        break;
                    }
                    else if (PeekString(tempPosition, 1) == "\n")
                    {
                        break;
                    }
                    stringBuilder.Append((char)_input[tempPosition++]);
                }
                else
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }            

        public string ReadLine()
        {
            var stringBuilder = new StringBuilder();
            while (true)
            { 
                if (Position < Length)
                {
                    if (PeekString(2) == "\r\n")
                    {
                        ReadString(2);
                        break;
                    }
                    else if (PeekString(1) == "\r")
                    {
                        ReadString(1);
                        break;
                    }
                    else if (PeekString(1) == "\n")
                    {
                        ReadString(1);
                        break;
                    }
                    stringBuilder.Append((char)_input[Position]);
                    Position++;
                }
                else
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        public string ReadString(int count)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                if (Position < Length)
                {
                    stringBuilder.Append((char)_input[Position++]);
                }
                else
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        public char PeekChar()
        {
            if (Position < Length)
            {
                return (char)_input[Position];
            }
            return '\0';
        }

        public char ReadChar()
        {
            if (Position < Length)
            {
                return (char)_input[Position++];
            }
            return '\0';
        }
    }
}
