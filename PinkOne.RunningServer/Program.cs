using System;

namespace PinkOne.RunningServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoRunProgram.Instance.StartUp();
            while (true)
            {
                GC.Collect();
                System.Threading.Thread.Sleep(1000*60);
            }
        }
    }
}
