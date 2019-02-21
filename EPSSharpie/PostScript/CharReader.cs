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

        public bool ReadChar(out char value)
        {
            value = _input[Position];
            Position += 1;
            return Position < Length;
        }
    }
}
