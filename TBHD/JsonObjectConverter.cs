using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SimpleYahooWeatherForecast
{
    public class JsonObjectConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type> { typeof(JsonObject) }; }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            JsonObject obj = new JsonObject();
            obj.Type = (string)dictionary["Type"];
            obj.Value = serializer.Serialize(dictionary["Value"]);
            return obj;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public class JsonObject
        {
            public string Value { get; set; }
            public string Type { get; set; }
        }
    }
}