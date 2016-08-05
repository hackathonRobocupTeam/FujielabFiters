using KinokoLib;

class TestCode
{
    static void Main(string[] args) 
    {
		string test = "test";
		string status = "status";
		string module = "module";
		AccessClass.push(test, status);
		AccessClass.pull(test);
		AccessClass.update(module, test, -1);
    }
}
