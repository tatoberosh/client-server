using System;
using System.Net.Sockets;
using System.Text;
 
namespace ClientApp
{
    public class Program
    {
        private static readonly int port = 8999;
        static readonly string serverIp = "127.0.0.1";
 
        public static void Main()
        {
            try
            {
                var client = new TcpClient(serverIp, port);
                var buffer = new byte[256];
                NetworkStream stream = client.GetStream();
 
                // Приветствие
                ReceiveMessage(stream, buffer);
                SendMessage(stream);
 
                // Вопрос
                ReceiveMessage(stream, buffer);
                SendMessage(stream);
 
                // Результат
                ReceiveMessage(stream, buffer);
 
                stream.Close();
                client.Close();
            }
            catch (SocketException exception)
            {
                Console.WriteLine($"SocketException: {exception}");
            }
        }
 
        private static void SendMessage(NetworkStream stream)
        {
            Console.WriteLine("What should I send?");
            var bytes = Encoding.UTF8.GetBytes(Console.ReadLine());
            stream.Write(bytes, 0, bytes.Length);
        }
 
        private static string ReceiveMessage(NetworkStream stream, byte[] buffer)
        {
            var response = new StringBuilder();
            var isReading = true;
            while (isReading)
            {
                var byteCount = stream.Read(buffer, 0, buffer.Length);
                response.Append(Encoding.UTF8.GetString(buffer, 0, byteCount));
                isReading = stream.DataAvailable;
            }
 
            Console.WriteLine($"Received from Server: {response}");
            return response.ToString();
        }
    }
}
