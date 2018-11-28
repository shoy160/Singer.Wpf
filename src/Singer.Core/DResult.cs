using System;
using System.Collections.Generic;

namespace Singer.Core
{
    /// <summary> 通用结果类 </summary>
    [Serializable]
    public class DResult
    {
        /// <summary> 状态 </summary>
        public bool Status { get; set; }
        /// <summary> 错误码 </summary>
        public int Code { get; set; } = -1;
        /// <summary> 错误消息 </summary>
        public string Message { get; set; }
        /// <summary> 时间戳 </summary>
        public DateTime TimeStamp { get; set; }

        public DResult() { }
        public DResult(string message, int code = -1)
        {
            Code = code;
            Message = message;
        }

        public static DResult Success => new DResult(string.Empty, 0) { Status = true };

        public static DResult<T> Succ<T>(T data) => new DResult<T>(data);
        public static DResults<T> Succ<T>(List<T> data, int total) => new DResults<T>(data, total);
    }

    public class DResult<T> : DResult
    {
        public T Data { get; set; }

        public DResult() { }

        public DResult(string message, int code = -1) : base(message, code)
        {
        }

        public DResult(T data)
        {
            Data = data;
            Code = 0;
            Status = true;
        }

    }

    public class DResults<T> : DResult
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
        public DResults() { }

        public DResults(string message, int code = -1) : base(message, code)
        {
        }
        public DResults(List<T> data, int total = -1)
        {
            Data = data;
            Total = total < 0 ? data.Count : total;
        }
    }
}
