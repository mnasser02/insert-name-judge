using System.Text;
using System.Text.Json;
using Modules;

namespace Modules
{
    public class Jsonifier
    {
        public static string ToJson(object obj)
        {
            string objectType = obj.GetType().ToString();
            StringBuilder jsonObj = new(JsonSerializer.Serialize(obj));
            jsonObj.Remove(jsonObj.Length - 1, 1);
            jsonObj.Append($",\"type\":\"{objectType}\"}}");
            return jsonObj.ToString();
        }

        public static object ToObject(string json)
        {
            // convert string to dictionary
            var jsonDict =
                JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                ?? throw new InvalidOperationException("Invalid JSON string.");
            // get the type of the object
            string type = jsonDict["type"].ToString()!;
            return type switch
            {
                "Solution"
                    => new Solution(
                        int.Parse(jsonDict["problemID"].ToString()!),
                        jsonDict["Lang"].ToString()!,
                        jsonDict["Code"].ToString()!
                    ),
                "Problem"
                    => new Problem(
                        int.Parse(jsonDict["Id"].ToString()!),
                        jsonDict["Name"].ToString()!,
                        jsonDict["Statement"].ToString()!,
                        int.Parse(jsonDict["Rating"].ToString()!),
                        jsonDict["Input_Format"].ToString()!,
                        jsonDict["Output_Format"].ToString()!
                    ),
                "Testcase"
                    => new Testcase(
                        int.Parse(jsonDict["Id"].ToString()!),
                        jsonDict["Input"].ToString()!,
                        jsonDict["Output"].ToString()!,
                        int.Parse(jsonDict["ProblemID"].ToString()!)
                    ),
                _ => throw new InvalidOperationException("Invalid object type."),
            };
        }
    }
}
