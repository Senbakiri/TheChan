using System;
using Newtonsoft.Json;

namespace Core.Converters {
    class JsonConverter<T> : IConverter<string, T> {
        public T Convert(string source) {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
