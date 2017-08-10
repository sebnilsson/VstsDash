using System;
using Newtonsoft.Json;

namespace VstsDash.RestApi
{
    internal static class ApiResponseSerializationHelper
    {
        private static readonly JsonSerializerSettings Settings;

        static ApiResponseSerializationHelper()
        {
            Settings = new JsonSerializerSettings
                       {
                           NullValueHandling = NullValueHandling.Ignore
                       };
        }

        public static TDeserialized DeserializeResponse<TDeserialized>(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var deserialized = JsonConvert.DeserializeObject<TDeserialized>(value, Settings);
            return deserialized;
        }

        public static string SerializeRequest(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var serialized = JsonConvert.SerializeObject(value, Settings);
            return serialized;
        }
    }
}