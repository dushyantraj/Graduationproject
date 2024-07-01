using System;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using CafeteriaServer.Operations;
using CafeteriaServer.Services;
using CafeteriaServer.Repositories;
using CafeteriaServer.Utilities;
using CafeteriaServer.Models; // Ensure this namespace is correct for RoleEnum

public static class CommandHandler
{
         public  static string ProcessCommand(string data, MySqlConnection connection)
        {
            string[] parts = data.Split(' ');
            string command = parts[0];
            string response = "";

            switch (command)
            {
                case "LOGIN":
                    response = HandleLoginCommand(parts, connection);
                    break;
                case "FETCH_WITH_RECOMMENDATION":
                    response = HandleFetchMenuItemsWithFeedback(connection);
                    break;
                case "MENU_FETCH":
                    response = HandleMenuFetch(connection);
                    break;
                case "ADD":
                    response = HandleAddMenuItem(parts, connection);
                    break;
                case "UPDATE":
                    response = HandleUpdateMenuItem(parts, connection);
                    break;
                case "DELETE":
                    response = HandleDeleteMenuItem(parts, connection);
                    break;
                case "ROLLOUT":
                    response = HandleRolloutCommand(parts, connection);
                    break;
                case "FETCH_FEEDBACK":
                    response = HandleFetchFeedback(connection);
                    break;
                case "FETCH_EMPLOYEE_SELECTIONS":
                    response = HandleFetchEmployeeSelections(connection);
                    break;
                case "FETCH_ROLLOUT":
                    response = HandleFetchRolloutItems(parts, connection);
                    break;
                case "SELECT_ITEM":
                    response = HandleSelectItem(parts, connection);
                    break;
                case "SEND_FEEDBACK_FORM":
                    response = HandleSendFeedbackForm(parts, connection);
                    break;
                case "SUBMIT_FEEDBACK":
                    response = HandleSubmitFeedback(parts, connection);
                    break;
                case "LOGOUT":
                    response = HandleLogout(parts, connection);
                    break;
                case "FETCH_NOTIFICATION_FOR_EMPLOYEE":
                    response = HandleFetchEmployeeNotification(connection);
                    break;
                case "FETCH_NOTIFICATION_FOR_CHEF":
                    response = HandleFetchChefNotification(connection);
                    break;
                case "FETCH_NOTIFICATIONS":
                    response = HandleFetchNotifications(connection);
                    break;
                case "FETCH_DISCARD_MENU_ITEMS":
                    response = HandleFetchDiscardMenuItems(connection);
                    break;
                case "REMOVE_DISCARD_MENU_ITEM":
                    response = HandleRemoveDiscardMenuItem(parts, connection);
                    break;
                case "GET_DETAILED_FEEDBACK":
                    response = HandleGetDetailedFeedback(parts, connection);
                    break;
                case "ROLL_OUT_FEEDBACK":
                    response = HandleRollOutFeedback(parts, connection);
                    break;
                case "SUBMIT_DETAILED_FEEDBACK":
                    response = HandleSubmitDetailedFeedback(parts, connection);
                    break;
                case "UPDATE_PROFILE":
                    response = HandleUpdateProfile(parts, connection);
                    break;
                case "FETCH_DETAILED_FEEDBACK_QUESTIONS":
                    response = HandleFetchDetailedFeedbackQuestions(parts, connection);
                    break;
                default:
                    response = "Invalid command.";
                    break;
            }

            return response;
        }

        

        // Command Handlers (Move each to respective service files)

        static string HandleLoginCommand(string[] parts, MySqlConnection connection)
        {
            if (parts.Length >= 3)
            {
                string username = parts[1];
                string password = parts[2];
                return LoginOperations.LoginUser(connection, username, password);
            }
            else
            {
                return "Invalid login command format.";
            }
        }

        static string HandleFetchMenuItemsWithFeedback(MySqlConnection connection)
        {
            var menuOperations = new MenuOperations();
            return menuOperations.FetchMenuItemsWithFeedback(connection);
        }

        static string HandleMenuFetch(MySqlConnection connection)
        {
            var menuOperations = new MenuOperations();
            return menuOperations.FetchMenu(connection);
        }

        static string HandleAddMenuItem(string[] parts, MySqlConnection connection)
        {
            if (parts.Length >= 6)
            {
                string menuType = parts[1];
                string itemName = string.Join(" ", parts, 2, parts.Length - 5).Trim('"').Trim();
                string foodType = parts[parts.Length - 1];

                if (decimal.TryParse(parts[parts.Length - 3], out decimal price))
                {
                    if (int.TryParse(parts[parts.Length - 2], out int available) && (available == 0 || available == 1))
                    {
                        var addMenuOperation = new MenuOperations();
                        return addMenuOperation.AddMenuItemToMenu(connection, menuType, itemName, price, available, foodType);
                    }
                    else
                    {
                        return "Invalid availability format.";
                    }
                }
                else
                {
                    return "Invalid price format.";
                }
            }
            else
            {
                return "Invalid command format.";
            }
        }

        static string HandleUpdateMenuItem(string[] parts, MySqlConnection connection)
        {
            if (parts.Length >= 4)
            {
                string itemName = string.Join(" ", parts.Skip(1).Take(parts.Length - 3)).Trim('"');

                if (decimal.TryParse(parts[parts.Length - 2], out decimal price))
                {
                    if (int.TryParse(parts[parts.Length - 1], out int available) && (available == 0 || available == 1))
                    {
                        var updateMenuOperation = new MenuOperations();
                        return updateMenuOperation.UpdateMenuItemInMenu(connection, itemName, price, available);
                    }
                    else
                    {
                        return "Invalid availability format.";
                    }
                }
                else
                {
                    return "Invalid price format.";
                }
            }
            else
            {
                return "Invalid command format.";
            }
        }

        static string HandleDeleteMenuItem(string[] parts, MySqlConnection connection)
        {
            if (parts.Length == 2 && int.TryParse(parts[1], out int deleteItemId))
            {
                var deleteMenuOperation = new MenuOperations();
                return deleteMenuOperation.DeleteMenuItemFromMenu(connection, deleteItemId);
            }
            else
            {
                return "Invalid delete command format.";
            }
        }

        static string HandleRolloutCommand(string[] parts, MySqlConnection connection)
        {
            if (parts.Length >= 2)
    {
        string[] itemIdsStr = new string[parts.Length - 1];
        Array.Copy(parts, 1, itemIdsStr, 0, parts.Length - 1);

        // Assuming you have an instance of `MySqlConnection` named `connection`
        // And the necessary dependencies for `RolloutManager`
        var rolloutRepository = new RolloutRepository(connection); // Example, replace with actual instantiation
        var notificationService = new NotificationService(connection); // Example, replace with actual instantiation

        // Create an instance of `RolloutManager` with the necessary dependencies
        var rolloutManager = new RolloutManager(rolloutRepository, notificationService);

        // Use the instance to call the method
        return rolloutManager.RolloutFoodItemsForNextDay(connection, itemIdsStr);
    }
    else
    {
        return "Invalid rollout command format.";
    }
        }

        static string HandleFetchFeedback(MySqlConnection connection)
        {
              FeedbackService feedbackServiceFetch = new FeedbackService();
             return feedbackServiceFetch.FetchFeedbackItems(connection);
        }

        static string HandleFetchEmployeeSelections(MySqlConnection connection)
        {
            var employeeSelectionService = new EmployeeSelectionService(connection);
            return employeeSelectionService.FetchEmployeeSelections();
        }

        static string HandleFetchRolloutItems(string[] parts, MySqlConnection connection)
        {
            if (parts.Length >= 2)
            {
                string username = parts[1];
                var rolloutOperations = new RolloutOperations();
                return rolloutOperations.FetchRolloutItemsWithFeedback(connection, username);
            }
            else
            {
                return "Invalid command format for fetching rollout items.";
            }
        }

        static string HandleSelectItem(string[] parts, MySqlConnection connection)
        {
            if (parts.Length > 1)
            {
                try
                {
                    var profileRepository = new EmployeeProfileRepository();
                    string username = parts[1];
                    int[] rolloutIds = parts.Skip(2).Select(int.Parse).ToArray();

                    int userId = profileRepository.GetUserIdByUsername(connection, username);
                    if (userId != -1)
                    {
                        var selectionService = new SelectionService();
                        return selectionService.SelectFoodItemsForNextDay(connection, userId, rolloutIds);
                    }
                    else
                    {
                        return $"User '{username}' not found.";
                    }
                }
                catch (FormatException)
                {
                    return "Invalid selection command format. Please provide valid rollout IDs.";
                }
                catch (Exception ex)
                {
                    return $"Error selecting items: {ex.Message}";
                }
            }
            else
            {
                return "No rollout items selected.";
            }
        }

        static string HandleSendFeedbackForm(string[] parts, MySqlConnection connection)
        {
               if (parts.Length >= 2)
                        {
                            // Create instance of FeedbackService with a unique variable name for this case
                            FeedbackService feedbackServiceForm = new FeedbackService();
                            string foodItem = string.Join(" ", parts, 1, parts.Length - 1);
                            return feedbackServiceForm.SubmitFeedback(connection, foodItem);
                        }
                        else
                        {
                            return "Invalid format for sending feedback form.";
                        }
        }

        static string HandleSubmitFeedback(string[] parts, MySqlConnection connection)
        {
              string commandArgs = string.Join(" ", parts.Skip(1));
                        Regex feedbackRegex = new Regex("\"(.*?)\"\\s(\\d)\\s\"(.*?)\"");

                        Match match = feedbackRegex.Match(commandArgs);
                        if (match.Success)
                        {
                            // Create instance of FeedbackService with a unique variable name for this case
                            FeedbackService feedbackServiceSubmit = new FeedbackService();
                            string itemName = match.Groups[1].Value; // Group 1: itemName
                            int rating = int.Parse(match.Groups[2].Value); // Group 2: rating
                            string comments = match.Groups[3].Value; // Group 3: comments

                            Console.WriteLine($"Parsed Feedback - Item: {itemName}, Rating: {rating}, Comments: {comments}");
                            return feedbackServiceSubmit.FillFeedbackForm(connection, itemName, rating, comments);
                        }
                        else
                        {
                            return "Invalid feedback format. Please provide a valid format.";
                        }
        }

        static string HandleLogout(string[] parts, MySqlConnection connection)
        {
              if (parts.Length >= 2)
                    {
                        return LoginOperations.LogoutUser(connection, parts[1]);
                    }
                    else
                    {
                        return "Invalid logout command format.";
                    }
        }

        static string HandleFetchEmployeeNotification(MySqlConnection connection)
        {
             return NotificationService.GetEmployeeNotification((int)RoleEnum.Employee, connection);
        }

        static string HandleFetchChefNotification(MySqlConnection connection)
        {
           return NotificationService.GetChefNotification((int)RoleEnum.Chef, connection);
         }

        static string HandleFetchNotifications(MySqlConnection connection)
        {
            int userTypeId = 3;
                return  DiscardMenu.FetchNotificationsForEmployees(userTypeId, connection);
        }

        static string HandleFetchDiscardMenuItems(MySqlConnection connection)
        {
              return DiscardMenu.FetchMenuItemsWithNegativeFeedback(connection);

        }

        static string HandleRemoveDiscardMenuItem(string[] parts, MySqlConnection connection)
        {
              if (parts.Length > 1)
                    {
                        string itemName = string.Join(" ", parts.Skip(1));
                        itemName = itemName.Trim();
                        DiscardMenu.RemoveMenuItem(connection, itemName);
                        return  $"Removed {itemName} from discard menu items.";
                    }
                    else
                    {
                        return "Invalid command format for removing discard menu item.";
                    }

                   
        }

        static string HandleGetDetailedFeedback(string[] parts, MySqlConnection connection)
        {
              if (parts.Length > 1)
                    {
                        string itemName = parts[1];
                        return DiscardMenu.RollOutFoodForDetailsFeedback(connection, itemName);
                    }
                    else
                    {
                        return "Invalid command format for getting detailed feedback.";
                    }
        }

        static string HandleRollOutFeedback(string[] parts, MySqlConnection connection)
        {
             if (parts.Length > 1)
                    {
                        string itemName = string.Join(" ", parts.Skip(1)).Trim();
                        return DiscardMenu.RollOutFoodForDetailsFeedback(connection, itemName);
                    }
                    else
                    {
                        return "Invalid command format for rolling out food for feedback.";
                 }
        }

        static string HandleSubmitDetailedFeedback(string[] parts, MySqlConnection connection)
        {
            if (parts.Length >= 5)
                    {
                        // Use a different variable name instead of 'command' to avoid conflict
                        string feedbackCommand = string.Join(" ", parts.Skip(1));
                        var feedbackParts = ParseFeedbackCommand(feedbackCommand);

                        if (feedbackParts.Length == 4) // itemName, question1Response, question2Response, question3Response
                        {
                            string itemName = feedbackParts[0];
                            string question1Response = feedbackParts[1];
                            string question2Response = feedbackParts[2];
                            string question3Response = feedbackParts[3];

                            return DiscardMenu.SubmitDetailedFeedback(connection, itemName, question1Response, question2Response, question3Response);
                        }
                        else
                        {
                            return "Invalid command format for submitting detailed feedback.";
                        }
                    }
                    else
                    {
                        return "Invalid command format for submitting detailed feedback.";
                    }
        }

        static string HandleUpdateProfile(string[] parts, MySqlConnection connection)
        {
                if (parts.Length >= 6) // Ensure there are enough parts
    {
        string username = parts[1]; // This is the username

        // Reconstruct the remaining parts into a single string for easier handling
        string remainingInput = string.Join(" ", parts.Skip(2));

        // Use a regex to split the string respecting quoted sections
        string[] preferencesParts = Regex.Matches(remainingInput, @"[\""].+?[\""]|[^ ]+")
                                          .Cast<Match>()
                                          .Select(m => m.Value.Trim('"'))
                                          .ToArray();

        if (preferencesParts.Length >= 4)
        {
            try
            {
                // Assign values to variables correctly
                string preference = preferencesParts[0];
                string spiceLevel = preferencesParts[1];
                string cuisinePreference = preferencesParts[2];
                bool sweetTooth = preferencesParts[3].Equals("true", StringComparison.OrdinalIgnoreCase);

                // Create instances of the required classes
                var profileRepository = new EmployeeProfileRepository();
                var profileOperations = new EmployeeProfileOperations();

                // Fetch UserID based on the provided username
                int userId = profileRepository.GetUserIdByUsername(connection, username);
                Console.WriteLine($"Username: {username}, UserID: {userId}");

                if (userId != -1)
                {
                    // Call the instance method using the created instance
                    return profileOperations.UpdateOrCreateProfile(connection, userId, preference, spiceLevel, cuisinePreference, sweetTooth);
                }
                else
                {
                    return "Error: Unable to retrieve UserID.";
                }
            }
            catch (Exception ex)
            {
                return $"Error processing the UPDATE_PROFILE command: {ex.Message}";
            }
        }
        else
        {
            return "Invalid command format for updating employee profile.";
        }
    }
    else
    {
        return "Invalid command format for updating employee profile.";
    }

        }

        static string HandleFetchDetailedFeedbackQuestions(string[] parts, MySqlConnection connection)
        {
             if (parts.Length > 1)
                    {
                        string itemName = string.Join(" ", parts.Skip(1)).Trim();
                        return DiscardMenu.GetDetailedFeedbackQuestions(itemName);
                    }
                    else
                    {
                        return "Invalid command format for fetching detailed feedback questions.";
                    }
        }
         private static string[] ParseFeedbackCommand(string commandText)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(commandText, @"(?<=^| )\""(\\.|[^\""])*\""|[^ ]+");
            return matches.Select(m => m.Value.Trim('"')).ToArray();
        }
   }
