using System;

namespace CafeteriaClient.Utilities
{
    public static class ConsoleHelper
    {
        public static decimal ReadDecimalFromConsole(string prompt)
        {
            Console.Write(prompt);
            decimal value;
            while (!decimal.TryParse(Console.ReadLine().Trim(), out value))
            {
                Console.Write("Invalid input. Enter the value again: ");
            }
            return value;
        }

        public static int ReadIntFromConsole(string prompt = "Enter a number: ")
        {
            Console.Write(prompt);
            int value;
            while (!int.TryParse(Console.ReadLine().Trim(), out value))
            {
                Console.Write("Invalid input. Enter a number: ");
            }
            return value;
        }

        public static int ReadAvailability()
        {
            int availability = ReadIntFromConsole("Enter availability (1 for available, 0 for not available): ");
            while (availability != 0 && availability != 1)
            {
                availability = ReadIntFromConsole("Invalid availability. Enter 1 for available or 0 for not available: ");
            }
            return availability;
        }
    }
}
