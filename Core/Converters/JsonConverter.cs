﻿using Newtonsoft.Json;

namespace Core.Converters {
    public class JsonConverter<T> : IConverter<string, T> {
        public T Convert(string source) {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
