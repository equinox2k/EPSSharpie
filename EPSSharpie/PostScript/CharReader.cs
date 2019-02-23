using System;
using System.Collections.Generic;
using System.Text;

namespace EPSSharpie.PostScript
{
    class CharReader
    {

        private readonly string _input;

        public int Position { get; set; }

        public int Length => _input.Length;

        public CharReader(string input)
        {
            _input = input;
            Position = 0;
        }

        public char PeekChar()
        {
            if (Position < Length)
            {
                return _input[Position];
            }
            return '\0';
        }

        public char ReadChar()
        {
            if (Position < Length)
            {
                return _input[Position++];
            }
            return '\0';
        }
    }
}
