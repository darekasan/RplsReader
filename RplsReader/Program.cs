using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace RplsReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0) return;
            if (!System.IO.File.Exists(args[0])) return;
            byte[] rawRpls = System.IO.File.ReadAllBytes(args[0]);

            var rpls = Rpls.Parse(rawRpls, 0);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            Console.WriteLine(JsonSerializer.Serialize(rpls, options));
        }
    }
}
