using KinokoLib;
using System;

class TestCode
{
    static void Main(string[] args) 
    {
		string test = "test";
		string status = "status";
		string module = "module";

        Console.WriteLine(String.Format("{0},{1},{2}",
            AccessClass.push(test, status),
            AccessClass.pull(test),
            AccessClass.update(module, test, -1)));

        Console.ReadKey();
        
    }
}
