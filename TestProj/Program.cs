using System;
using System.Threading.Tasks;

public class Example
{
    public static void Main()
    {
        var tester = new AsyncTester();

        tester.Catcher();
        Console.ReadLine();
    }
}

public class AsyncTester
{
    public async Task Catcher()
    {
        Task thrower = Thrower();

        try
        {
            await thrower;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task Thrower()
    {
        await Task.Delay(100);
        throw new Exception("Ex");
    }
}
