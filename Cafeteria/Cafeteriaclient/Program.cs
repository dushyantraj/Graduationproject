using System;
using MenuClient.Communication;
using MenuClient.Authentications;
using MenuClient.Menus;
using MenuClient.Utilities;

namespace MenuClient
{
    class Program
    {
        public static string currentUsername { get; set; }
        public static string currentRole { get; set; }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Welcome to Cafeteria Management System");
                Console.Write("Enter username: ");
                string username = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                string loginResponse = Authentication.Login(username, password);

                if (loginResponse.StartsWith("LOGIN_SUCCESS"))
                {
                    currentUsername = username;
                    currentRole = loginResponse.Split(' ')[1];
                    Console.WriteLine("Login successful. Role: {0}", currentRole);
                    ShowMenu();
                }
                else
                {
                    Console.WriteLine("Login failed. Server response: {0}", loginResponse);
                }
            }
        }

        static void ShowMenu()
        {
            switch (currentRole)
            {
                case "Admin":
                    AdminMenu.Show();
                    break;
                case "Chef":
                    ChefMenu.Show();
                    break;
                case "Employee":
                    EmployeeMenu.Show();
                    break;
                default:
                    Console.WriteLine("Invalid role. Logging out...");
                    Logout();
                    break;
            }
        }

        public static void Logout()
        {
            currentUsername = null;
            currentRole = null;
        }
    }
}
