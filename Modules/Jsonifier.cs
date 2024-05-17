using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;
using Modules;

namespace Modules
{
    public class Jsonifier
    {
        public static string ToJson(object obj)
        {
            if (obj is IList list)
            {
                string[] items = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    items[i] = $"\"Item{i}\":" + ToJson(list[i]!);
                }
                return "{\"Count\":"
                    + list.Count
                    + ","
                    + string.Join(",", items)
                    + ",\"type\":\""
                    + "List"
                    + "\"}";
            }
            string objectType = obj.GetType().ToString();
            StringBuilder jsonObj = new(JsonSerializer.Serialize(obj));
            if (jsonObj.Length < 3)
            {
                jsonObj = new("{\"value\":" + obj + "}");
            }
            jsonObj.Remove(jsonObj.Length - 1, 1);
            jsonObj.Append($",\"type\":\"{objectType}\"}}");
            return jsonObj.ToString();
        }

        public static object ToObject(string json)
        {
            Console.WriteLine("Converting JSON to object...\n\n");
            Console.WriteLine(json + "\n\n");
            // convert string to dictionary
            var jsonDict =
                JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                ?? throw new InvalidOperationException("Invalid JSON string.");
            // get the type of the object
            string type = jsonDict["type"].ToString()!;

            static List<Tuple<int, string>> lambda(Dictionary<string, object> dict)
            {
                int count = int.Parse(dict["Count"].ToString()!);
                List<Tuple<int, string>> list = new();
                for (int i = 0; i < count; i++)
                {
                    list.Add((Tuple<int, string>)ToObject(dict[$"Item{i}"].ToString()!));
                }
                return list;
            }
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
                        jsonDict["Input"].ToString()!,
                        jsonDict["Output"].ToString()!
                    ),
                "Testcase"
                    => new Testcase(
                        int.Parse(jsonDict["Id"].ToString()!),
                        jsonDict["Input"].ToString()!,
                        jsonDict["Output"].ToString()!,
                        int.Parse(jsonDict["ProblemID"].ToString()!)
                    ),
                "System.Tuple`2[System.Int32,System.String]"
                    => new Tuple<int, string>(
                        int.Parse(jsonDict["Item1"].ToString()!),
                        jsonDict["Item2"].ToString()!
                    ),
                "System.Int32" => int.Parse(jsonDict["value"].ToString()!),
                "List" => lambda(jsonDict),
                _ => throw new InvalidOperationException("Invalid object type."),
            };
        }
    }
}
