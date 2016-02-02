using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace Win2ch.Models.Exceptions {
    public class HttpException : Exception {
        public int Code { get; set; }
        public bool IsConnectionError { get; set; }
        
        public HttpException() { }

        public HttpException(HttpStatusCode code) : base(code.ToString()) {
            Code = (int)code;
        }

        public HttpException(Exception exception) {
            Code = exception.HResult;
            IsConnectionError = true;
        }
    }
}
