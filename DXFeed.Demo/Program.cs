using DXFeed.Net;
using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable S125 // Sections of code should not be commented out

namespace DXFeed.Demo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateConfiguration(args);

            if (args.Any(s => s == "--help") || string.IsNullOrEmpty(Configuration?["dxfeed:url"]))
            {
                Help();
                return;
            }

            var transport = TransportFactory.CreateWebsocketTransport();
            transport.Connect(Configuration["dxfeed:url"]);

            using var communicator = new Communicator(transport, true);



            //communicator.MessageSent += (sender, args) => Console.WriteLine("Raw Sent: {0}", args.Message);
            //communicator.MessageReceived += (sender, args) => Console.WriteLine("Raw Received: {0}", args.Message);

            using var connection = new DXFeedConnection(GetToken(), communicator, false);
            connection.SubscribeListener(new Listener());

            communicator.Start();

            while (Console.ReadKey().Key != ConsoleKey.Escape) Thread.Yield();
        }

        private static string? GetToken()
        {
            if (!string.IsNullOrEmpty(Configuration?["dxfeed:token"]))
                return Configuration["dxfeed:token"];
            if (!string.IsNullOrEmpty(Configuration?["dxfeed:tokenServlet"]))
            {
                var tokenFactory = new GehtsoftTokenFactory();
                return tokenFactory.GetToken(Configuration["dxfeed:tokenServlet"], "testusert");
            }
            return null;
        }


        public static IConfiguration? Configuration { get; private set; }

        private static void CreateConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", true)
                                .AddJsonFile("appsettings.local.json", true)
                                .AddCommandLine(args);

            Configuration = builder.Build();
        }

        private static void Help()
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("   --help                        show this help");
            Console.WriteLine("   --dxfeed:url=url              URL to dxfeed access point");
            Console.WriteLine("   --dxfeed:token=token          Token (if you use token to authenticate)");
            Console.WriteLine("   --dxfeed:tokenService=url     Url of the web service that generates the token");
            Console.WriteLine("");
            Console.WriteLine("Alternatively, create appsettings.local.json using the appsettings.json as template and put all settings there");
        }
    }
}
