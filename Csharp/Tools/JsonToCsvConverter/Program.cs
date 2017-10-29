using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonToCsvConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Usage: JsonToCsvConverter.exe InputFile JsonPath OutputFile");
            List<string> csv = Common.Storage.JsonToCsvConverter.ConvertJsonToCsv(args[0], args[1], true).ToList();
            File.WriteAllLines(args[2], csv);
        }
    }
}
