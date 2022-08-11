using DXFeed.Net.Platform;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            bool received = false;
            
            communicator.MessageReceived += (sender, args) => 
            {
                Console.WriteLine("received: {0}", args.Message);
                received = true;
            };

            communicator.ExceptionRaised += (sender, args) =>
            {
                Console.WriteLine("exception: {0}", args.Exception.Message);
            };

            var token = GetToken();
            var message = "[ {\"channel\" : \"/meta/handshake\"";
            if (token != null)
            {
                message += ", \"ext\" : { ";
                message += "\"com.devexperts.auth.AuthToken\" : \"";
                message += token;
                message += "\" }";
            }
            message += " } ]";


            communicator.Start();
            communicator.Send(message);

            while (!received)
                Thread.Sleep(10);

            communicator.Close();
        }

        private static string? GetToken()
        {
            if (!string.IsNullOrEmpty(Configuration["dxfeed:token"]))
                return Configuration["dxfeed:token"];
            if (!string.IsNullOrEmpty(Configuration["dxfeed:tokenServlet"]))
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
