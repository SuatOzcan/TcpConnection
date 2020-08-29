using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SampleTcpServer
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            int port = 54321;
            IPAddress address = IPAddress.Any;
            TcpListener server = new TcpListener(address, port);
            server.Start();

            var loggedNoRequest = false;
            bool done = false;
            string DELIMITER = "|";
            string TERMINATE = "TERMINATE";

            while (!done)
            {
                if (!loggedNoRequest)
                {
                    Console.WriteLine("Server listening...");
                    //loggedNoRequest = false;
                    byte[] bytes = new byte[256];

                    using (var client = await server.AcceptTcpClientAsync())
                    {
                        using (var tcpStream = client.GetStream())
                        {
                            await tcpStream.ReadAsync(bytes, 0, bytes.Length);
                            var requestMessage = Encoding.UTF8.GetString(bytes).Replace("\0",string.Empty);

                            if (requestMessage.Equals(TERMINATE))
                            {
                                done = true;
                                server.Stop();
                                Thread.Sleep(10000);
                            }
                            else
                            {
                                Console.WriteLine(requestMessage);
                                var payload = requestMessage.Split(DELIMITER).Last();
                                var responseMessage = $"Greetings from the server! | {payload}";
                                var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                                await tcpStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                            }
                        }
                    }
                } 
            }
        }
    }
}
