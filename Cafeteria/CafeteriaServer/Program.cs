using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MySql.Data.MySqlClient;
using CafeteriaServer.Services;
using CafeteriaServer.Operations;

namespace CafeteriaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost;Database=CafeteriaDB;Uid=root;Pwd=Admin@123;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                var menuService = new MenuService();
                var feedbackService = new FeedbackService();
                var recommendationService = new RecommendationService();
                var selectionService = new SelectionService();

                var menuOperations = new MenuOperations(menuService, feedbackService, recommendationService, selectionService);
                var commandHandler = new CommandHandler(menuOperations); // Instantiate CommandHandler with menuOperations

                TcpListener server = new TcpListener(IPAddress.Any, 13000);
                connection.Open();
                Console.WriteLine("Connected to MySQL database.");

                server.Start();
                Console.WriteLine("Server started. Listening for clients...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine("Received from client: {0}", data);

                    string response = commandHandler.ProcessCommand(data, connection);

                    SendResponseToClient(stream, response);
                    Console.WriteLine("Sent to client: {0}", response);

                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Disconnected from MySQL database.");
                }
            }
        }

        static void SendResponseToClient(NetworkStream stream, string response)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}
