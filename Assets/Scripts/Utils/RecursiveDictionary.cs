using System;
using System.Collections.Generic;

namespace Utils
{

    public class RecursiveDict : Dictionary<string, RecursiveDict>
    {

        public object value { get; set; }

        public override string ToString()
        {
            return (value ?? string.Empty).ToString();
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            //Usage example:
            RecursiveDict dict = new RecursiveDict();
            dict["string"] = new RecursiveDict();
            dict["string"]["stringagain"] = new RecursiveDict();
            dict["string"]["stringagain"].value = "Bob";

            //Console.WriteLine(dict["string"]["stringagain"]);

        }
    }

}