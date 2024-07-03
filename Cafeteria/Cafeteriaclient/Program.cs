using System;
using CafeteriaClient.Authentications;
using CafeteriaClient.Menus;
using CafeteriaClient.Utilities;

namespace CafeteriaClient
{
    class Program
    {
        public static string CurrentUsername { get; private set; }
        public static string CurrentRole { get; private set; }

        static void Main(string[] args)
        {
            while (true)
            {
                DisplayWelcomeMessage();
                if (AuthenticateUser())
                {
                    ShowMenu();
                }
                else
                {
                    break; // Exit the loop if authentication fails
                }
            }
        }

        private static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to Cafeteria Management System");
        }

        private static bool AuthenticateUser()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            string loginResponse = Authentication.Login(username, password);

            if (loginResponse.StartsWith("LOGIN_SUCCESS"))
            {
                CurrentUsername = username;
                CurrentRole = loginResponse.Split(' ')[1];
                Console.WriteLine("Login successful. Role: {0}", CurrentRole);
                return true;
            }
            else
            {
                Console.WriteLine("Login failed. Server response: {0}", loginResponse);
                return false;
            }
        }

        private static void ShowMenu()
        {
            IMenu menu = MenuFactory.GetMenu(CurrentRole);
            if (menu != null)
            {
                menu.Show();
            }
            else
            {
                Console.WriteLine("Invalid role. Logging out...");
                Logout();
            }
        }

        public static void Logout()
        {
            Utils.Logout(CurrentUsername);
            ClearCurrentUser();
            Environment.Exit(0);
        }

        public static void ClearCurrentUser()
        {
            CurrentUsername = null;
            CurrentRole = null;
            Console.WriteLine("You have been logged out.");
        }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Chef = "Chef";
        public const string Employee = "Employee";
    }

    public interface IMenu
    {
        void Show();
    }
}
