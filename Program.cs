using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ConsoleService
{
    class Program
    {
        const string uri = "ws://localhost";
        const int port = 80;

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now + " | Started.");

            WebSocketServer webSocketServer = new WebSocketServer(uri + ":" + port);

            webSocketServer.AddWebSocketService<Echo>("/Echo");

            webSocketServer.Start();
            Console.WriteLine(DateTime.Now + " | Server started.");

            Console.ReadKey();
            webSocketServer.Stop();
                        
        }
    }

    public class Echo : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " | Received from client: " + e.Data);
        }

        protected override void OnOpen()
        {
            Console.WriteLine(DateTime.Now + " | Client connected!");
        }
    }

}