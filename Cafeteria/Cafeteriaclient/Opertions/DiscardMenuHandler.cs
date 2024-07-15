using System;
using CafeteriaClient.Services;
using CafeteriaClient.Utilities;

namespace CafeteriaClient.Operations
{
    public static class DiscardMenuHandler
    {
        private static ServerCommunicator serverCommunicator = new ServerCommunicator();

        public static void HandleDiscardMenu()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("1) Remove a Food Item from Menu");
            Console.WriteLine("2) Get Detailed Feedback on a Food Item");
            Console.WriteLine("3) Exit");

            string subChoice = Console.ReadLine();

            switch (subChoice)
            {
                case "1":
                    RemoveFoodItemFromMenu();
                    break;
                case "2":
                    GetDetailedFeedbackOnFoodItem();
                    break;
                case "3":
                    Console.WriteLine("Exiting Discard Food Item menu.");
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }

        private static void RemoveFoodItemFromMenu()
        {
            Console.WriteLine("Enter the name of the food item to remove:");
            string removeItemName = Console.ReadLine();
            string removeRequest = $"{ServerCommands.RemoveDiscardMenuItem} {removeItemName}";
            string removeResponse = serverCommunicator.SendCommandToServer(removeRequest);
            Console.WriteLine(removeResponse);
        }

        private static void GetDetailedFeedbackOnFoodItem()
        {
            Console.WriteLine("Enter the name of the food item for feedback:");
            string feedbackItemName = Console.ReadLine();
            string feedbackRequest = $"{ServerCommands.RollOutFeedback} {feedbackItemName}";
            string feedbackResponse = serverCommunicator.SendCommandToServer(feedbackRequest);
            Console.WriteLine(feedbackResponse);
        }
    }
}
