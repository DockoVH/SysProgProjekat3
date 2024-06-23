using System;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server;
public class Program
{
    public static void Main(string[] args)
    {
        var server = new WebServer();
        server.Start();

        Console.WriteLine("Server pokrenut. Pritisnite Enter za zaustavljanje.");
        Console.ReadLine();
    }
}