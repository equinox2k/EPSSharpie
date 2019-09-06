using System;

namespace EPSSharpie.Tests.ImageCompare.Exceptions
{ 
    public class ImagesSimilarityException : Exception
    {
        public ImagesSimilarityException(string message) : base(message)
        {
        }
    }
}