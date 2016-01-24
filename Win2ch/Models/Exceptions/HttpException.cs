using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Win2ch.Models.Exceptions {
    public class HttpException : Exception {
        public HttpStatusCode Code { get; }

        public HttpException(HttpStatusCode code) : base(code.ToString()) {
            Code = code;
        }
    }
}
