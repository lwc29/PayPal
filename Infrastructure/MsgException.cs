using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class MsgException : Exception
    {
        public int StatusCode { get; private set; }

        public MsgException(string msg, HttpMsgType httpMsgType = HttpMsgType.OK)
            : base(msg)
        {
            StatusCode = (int)httpMsgType;
        }
    }

    public enum HttpMsgType
    {
        Exception = 100,
        LoginExpired = 110,
        OK = 200,
        Error = 500
    }
}
