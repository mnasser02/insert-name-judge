using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Modules
{
    public class Request
    {

        public string Type { get; }
        public object Body { get; }

        public Type BodyType { get; }

        [Newtonsoft.Json.JsonConstructor]
        public Request(string type, object body, Type bodyType)
        {
            Type = type;
            Body = body;
            BodyType = bodyType;
        }

        public Request(string jsonString)
        {
            var req = JsonConvert.DeserializeObject<Request>(jsonString, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            })!;
            Type = req.Type;
            BodyType = req.BodyType;
            if (BodyType == typeof(string))
            {
                Body = req.Body.ToString()!;
            }
            else
            {
                Body = JsonConvert.DeserializeObject(req.Body.ToString()!, BodyType)!;
            }
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
