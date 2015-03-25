using System;

namespace NCore
{
    public class NCoreException : Exception
    {
        public NCoreException(string message)
            : base(message)
        {

        }
        public NCoreException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
