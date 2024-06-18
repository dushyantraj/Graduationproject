using System;
using System.Net.Sockets;
using System.Text;

namespace MenuClient.Communication
{
    class ServerCommunicator
    {
        public static string SendCommandToServer(string command)
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 13000))
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
                return "Error: " + ex.Message;
            }
        }
    }
}
