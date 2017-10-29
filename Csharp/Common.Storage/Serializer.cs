using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Common.Storage
{
    public static class Serializer
    {
        static Serializer()
        {
            JsonConvert.DefaultSettings = () =>
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
        }

        public static string Serialize<T>(T @object)
            => JsonConvert.SerializeObject(@object);

        public static T Deserialize<T>(string @object)
            => JsonConvert.DeserializeObject<T>(@object);
    }
}
