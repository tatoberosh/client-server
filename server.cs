using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
 
namespace ServerApp
{
    public class Program
    {
        static readonly int port = 8999;
        static readonly IPAddress serverIp = IPAddress.Parse("127.0.0.1");
 
        public static void Main()
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(serverIp, port);
                server.Start();
                Console.WriteLine("Server is loaded...");
                while (true)
                {
                    ThreadPool.QueueUserWorkItem(HandleClient, server.AcceptTcpClient(), false);
 
                }
            }
            catch (SocketException exception)
            {
                Console.WriteLine($"SocketException: {exception}");
            }
            finally
            {
                server.Stop();
            }
        }
 
        private static void HandleClient(TcpClient client)
        {
            try
            {
                Console.WriteLine($"Client connected with IP: {client.Client.RemoteEndPoint}");
                var buffer = new byte[256];
                NetworkStream stream = client.GetStream();
 
                SendMessage(stream, $"Hi {client.Client.RemoteEndPoint}");
                ReceiveMessage(stream, client, buffer);
                SendMessage(stream, "what is the answer to 2 + 2?");
                var response = ReceiveMessage(stream, client, buffer);
                SendMessage(stream, response == "4" ? "Correct" : "Nope");
            }
            catch
            {
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} stopped the connection");
            }
 
            client.Close();
        }
 
        private static void SendMessage(NetworkStream stream, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }
 
        private static string ReceiveMessage(NetworkStream stream, TcpClient client, byte[] buffer)
        {
            var response = new StringBuilder();
            var isReading = true;
            while (isReading)
            {
                var byteCount = stream.Read(buffer, 0, buffer.Length);
                response.Append(Encoding.UTF8.GetString(buffer, 0, byteCount));
                isReading = stream.DataAvailable;
            }
 
            Console.WriteLine($"Received from {client.Client.RemoteEndPoint}: {response}");
            return response.ToString();
        }
    }
}
