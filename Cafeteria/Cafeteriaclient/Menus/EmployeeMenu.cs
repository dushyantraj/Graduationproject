using System;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    public class EmployeeMenu : IMenu
    {
        public void Show()
        {
            while (true)
            {
                DisplayMenu();
                string choice = GetMenuChoice();

                if (!HandleMenuChoice(choice))
                {
                    break;
                }
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("Employee Menu:");
            Console.WriteLine("1. Select Food Item for Next Day");
            Console.WriteLine("2. Fill the Feedback Form");
            Console.WriteLine("3. View Notification");
            Console.WriteLine("4. Detailed Feedback");
            Console.WriteLine("5. Fetch Employee Notifications");
            Console.WriteLine("6. Logout");
        }

        private string GetMenuChoice()
        {
            Console.Write("Enter your choice: ");
            return Console.ReadLine();
        }

        private bool HandleMenuChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    EmployeeOperations.SelectFoodItemForNextDay();
                    break;
                case "2":
                    EmployeeOperations.FillFeedbackForm();
                    break;
                case "3":
                    EmployeeOperations.FetchNotificationForEmployee();
                    break;
                case "4":
                    EmployeeOperations.DetailedFeedback();
                    break;
                case "5":
                    EmployeeOperations.FetchEmployeeNotifications();
                    break;
                case "6":
                    EmployeeOperations.UpdateProfile();
                    return false;
                case "7":
                    Program.Logout();
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
            return true;
        }

    }
}
