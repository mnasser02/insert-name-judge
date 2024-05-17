using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Modules
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            List<Tuple<int, string>> list = new() { new(1, "one"), new(2, "two"), new(3, "three") };
            string json = Jsonifier.ToJson(list);
            Console.WriteLine("\\n\n" + json + "\n\n");
            List<Tuple<int, string>> list2 = (List<Tuple<int, string>>)Jsonifier.ToObject(json);
            foreach (var item in list2)
            {
                Console.WriteLine(item);
            }
        }
    }
}
