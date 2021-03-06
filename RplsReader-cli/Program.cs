using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RplsReader;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace RplsReader_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0) return;
            if (!System.IO.File.Exists(args[0])) return;

            var rpls = Rpls.Parse(args[0]);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            Console.WriteLine(JsonSerializer.Serialize(rpls, options));
        }
    }
}
