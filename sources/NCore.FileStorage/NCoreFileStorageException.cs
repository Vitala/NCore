using System;

namespace NCore.FileStorage
{
    public class NCoreFileStorageException : Exception
    {
        public NCoreFileStorageException(string message) : base(message)
        {
            
        }

        public NCoreFileStorageException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
