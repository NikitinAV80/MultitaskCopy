using System;

namespace MultitaskCopyLib
{
    public class PathException : Exception
    {
        public PathException(string message) : base(message) {}
    }
}