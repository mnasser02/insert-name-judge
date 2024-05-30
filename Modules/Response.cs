using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Modules
{
    public class Response
    {

        public object Body { get; set; }

        public Type BodyType { get; }

        [Newtonsoft.Json.JsonConstructor]
        public Response(object body, Type bodyType)
        {
            Body = body;
            BodyType = bodyType;
        }

        public Response(string jsonString)
        {
            var res = JsonConvert.DeserializeObject<Response>(jsonString, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            })!;
            BodyType = res.BodyType;
            if (BodyType == typeof(string))
            {
                Body = res.Body.ToString()!;
            }
            else
            {
                Body = JsonConvert.DeserializeObject(res.Body.ToString()!, BodyType)!;
            }
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
