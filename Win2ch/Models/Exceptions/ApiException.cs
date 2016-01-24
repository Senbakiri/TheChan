using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Win2ch.Models.Api;

namespace Win2ch.Models.Exceptions {
    public class ApiException : Exception {
        public ApiException() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception inner) : base(message, inner) { }

    }

    public static class ApiResultChecker {
        public static void CheckForApiError(this string json) {
            var parsed = new ApiResult();

            try {
                parsed = JsonConvert.DeserializeObject<ApiResult>(json);
            } catch (JsonException) { }

            if (parsed.Error != null)
                throw new ApiException(parsed.Reason ?? parsed.Error);
        }
    }
}
