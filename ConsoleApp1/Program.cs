using System;
using System.IO.Pipes;

namespace ConsoleApp1
{
    class Program
    {
        static NamedPipeServerStream pipeServer;


        static void Main(string[] args)
        {
            pipeServer = new NamedPipeServerStream("TestPipe", PipeDirection.InOut);

            Console.WriteLine("Waiting..");

            pipeServer.WaitForConnection();

            Console.WriteLine("Someone connected");

            Console.WriteLine("Hello World!");
        }
    }
}
