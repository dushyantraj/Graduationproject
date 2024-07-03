using System;
using System.Net.Sockets;
using System.Text;

namespace CafeteriaClient.Services
{
    public class ServerCommunicator
    {
        private readonly string serverAddress;
        private readonly int serverPort;

        public ServerCommunicator(string address = "127.0.0.1", int port = 13000)
        {
            serverAddress = address;
            serverPort = port;
        }

        public string SendCommandToServer(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient(serverAddress, serverPort))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] requestBytes = Encoding.UTF8.GetBytes(command);
                    stream.Write(requestBytes, 0, requestBytes.Length);
                    Console.WriteLine("Sent to server: {0}", command);

                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    return response.Trim();
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
