using System;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
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

            while (!Console.KeyAvailable)
            {
            }

            webSocketServer.Stop();
        }
    }

    public class Echo : WebSocketBehavior
    {
        public DateTime lastTimeRetrievedData;
        public int requestIntervalMinutes = 5; //TODO: Do smthing better...

        const string azureUri = "http://localhost:7071/api/DataCollector";

        Timer timer;

        protected override void OnMessage(MessageEventArgs e)
        {
            var _data = e.Data;
            Console.WriteLine(DateTime.Now + " | Received from client: " + _data);

            lastTimeRetrievedData = DateTime.Now;

            PostDataToAzure(_data);
        }
        private void PostDataToAzure(string postData)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(azureUri);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";

                using (Stream stream = webRequest.GetRequestStream())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(postData);
                    stream.Write(buffer, 0, buffer.Length);

                    Console.WriteLine(DateTime.Now + " | Send data to Azure.");
                }

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                webResponse.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + " | [ERROR] " + e.Message); //TODO: Extrude console log into function both in client and server project.
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine(DateTime.Now + " | Client connected!");

            timer = new Timer(1000 * 60 * requestIntervalMinutes);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " | [ERROR] " + e.Message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " | Client disconnected!");

            timer = null;
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Send("GetData");
            }
            catch (Exception ex)
            {
                //TODO: Handle ex
            }
        }


    }

}