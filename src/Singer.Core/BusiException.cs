using System;

namespace Singer.Core
{
    /// <summary> 业务异常 </summary>
    public class BusiException : Exception
    {
        /// <summary> 异常编码 </summary>
        public int Code { get; set; }

        public BusiException(string message, int code = -1)
            : base(message)
        {
            Code = code;
        }
    }
}
