using System;

namespace Singer.Core
{
    public class BusiException : Exception
    {
        public int Code { get; set; }

        public BusiException(string message, int code = -1)
            : base(message)
        {
            Code = code;
        }
    }
}
