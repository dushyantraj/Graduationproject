using System;
using MenuClient.Operations;

namespace MenuClient.Menus
{
    class EmployeeMenu
    {
        public static void Show()
        {
            while (true)
            {
                Console.WriteLine("Employee Menu:");
                Console.WriteLine("1. Select Food Item for Next Day");
                Console.WriteLine("2. Fill the Feedback Form");
                Console.WriteLine("3. Logout");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EmployeeOperations.SelectFoodItemForNextDay();
                        break;
                    case "2":
                        EmployeeOperations.FillFeedbackForm();
                        break;
                    case "3":
                        Program.Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}
