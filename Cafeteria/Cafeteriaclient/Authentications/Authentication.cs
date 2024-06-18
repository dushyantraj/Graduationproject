
using System;
using MenuClient.Communication;

namespace MenuClient.Authentications
{
    public static class Authentication
    {
        public static string Login(string username, string password)
        {
            string command = $"LOGIN {username} {password}";
            return ServerCommunicator.SendCommandToServer(command);
        }
    }
}
