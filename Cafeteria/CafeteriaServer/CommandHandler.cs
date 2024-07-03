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
using CafeteriaServer.Models;

public class CommandHandler
{
    private readonly MenuOperations _menuOperations;

    public CommandHandler(MenuOperations menuOperations)
    {
        _menuOperations = menuOperations ?? throw new ArgumentNullException(nameof(menuOperations));
    }

    public string ProcessCommand(string data, MySqlConnection connection)
    {
        string[] parts = data.Split(' ');
        string command = parts[0];

        return command switch
        {
            "LOGIN" => HandleLoginCommand(parts, connection),
            "FETCH_WITH_RECOMMENDATION" => HandleFetchMenuItemsWithFeedback(connection),
            "MENU_FETCH" => HandleMenuFetch(connection),
            "ADD" => HandleAddMenuItem(parts, connection),
            "UPDATE" => HandleUpdateMenuItem(parts, connection),
            "DELETE" => HandleDeleteMenuItem(parts, connection),
            "ROLLOUT" => HandleRolloutCommand(parts, connection),
            "FETCH_FEEDBACK" => HandleFetchFeedback(connection),
            "FETCH_EMPLOYEE_SELECTIONS" => HandleFetchEmployeeSelections(connection),
            "FETCH_ROLLOUT" => HandleFetchRolloutItems(parts, connection),
            "SELECT_ITEM" => HandleSelectItem(parts, connection),
            "SEND_FEEDBACK_FORM" => HandleSendFeedbackForm(parts, connection),
            "SUBMIT_FEEDBACK" => HandleSubmitFeedback(parts, connection),
            "LOGOUT" => HandleLogout(parts, connection),
            "FETCH_NOTIFICATION_FOR_EMPLOYEE" => HandleFetchEmployeeNotification(connection),
            "FETCH_NOTIFICATION_FOR_CHEF" => HandleFetchChefNotification(connection),
            "FETCH_NOTIFICATIONS" => HandleFetchNotifications(connection),
            "FETCH_DISCARD_MENU_ITEMS" => HandleFetchDiscardMenuItems(connection),
            "REMOVE_DISCARD_MENU_ITEM" => HandleRemoveDiscardMenuItem(parts, connection),
            "GET_DETAILED_FEEDBACK" => HandleGetDetailedFeedback(parts, connection),
            "ROLL_OUT_FEEDBACK" => HandleRollOutFeedback(parts, connection),
            "SUBMIT_DETAILED_FEEDBACK" => HandleSubmitDetailedFeedback(parts, connection),
            "UPDATE_PROFILE" => HandleUpdateProfile(parts, connection),
            "FETCH_DETAILED_FEEDBACK_QUESTIONS" => HandleFetchDetailedFeedbackQuestions(parts, connection),
            "CHECK_FEEDBACK_ROLLOUT" => HandleFetchFeedback_DiscardMenuItems(connection),
            _ => "Invalid command.",
        };
    }

    private static string HandleLoginCommand(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 3)
        {
            string username = parts[1];
            string password = parts[2];

            var credentials = new UserCredentials
            {
                Username = username,
                Password = password
            };

            return LoginOperations.LoginUser(connection, credentials);
        }
        return "Invalid login command format.";
    }

    private string HandleFetchMenuItemsWithFeedback(MySqlConnection connection)
    {
        return _menuOperations.FetchMenuItemsWithFeedback(connection);
    }

    private string HandleMenuFetch(MySqlConnection connection)
    {
        return _menuOperations.FetchMenu(connection);
    }

    private string HandleAddMenuItem(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 6)
        {
            var dto = new AddMenuItemDTO
            {
                MenuType = parts[1],
                ItemName = string.Join(" ", parts, 2, parts.Length - 5).Trim('"').Trim(),
                FoodType = parts[parts.Length - 1]
            };

            if (decimal.TryParse(parts[parts.Length - 3], out decimal price))
            {
                if (int.TryParse(parts[parts.Length - 2], out int available) && (available == 0 || available == 1))
                {
                    dto.Price = price;
                    dto.Available = available;

                    return _menuOperations.AddMenuItemToMenu(connection, dto);
                }
                return "Invalid availability format. Availability should be 0 or 1.";
            }
            return "Invalid price format.";
        }
        return "Invalid command format.";
    }

    private string HandleUpdateMenuItem(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 4)
        {
            var dto = new UpdateMenuItemDTO
            {
                ItemName = string.Join(" ", parts.Skip(1).Take(parts.Length - 3)).Trim('"')
            };

            if (decimal.TryParse(parts[parts.Length - 2], out decimal price))
            {
                if (int.TryParse(parts[parts.Length - 1], out int available) && (available == 0 || available == 1))
                {
                    dto.Price = price;
                    dto.Available = available;

                    return _menuOperations.UpdateMenuItemInMenu(connection, dto);
                }
                return "Invalid availability format.";
            }
            return "Invalid price format.";
        }
        return "Invalid command format.";
    }

    private string HandleDeleteMenuItem(string[] parts, MySqlConnection connection)
    {
        if (parts.Length == 2 && int.TryParse(parts[1], out int deleteItemId))
        {
            return _menuOperations.DeleteMenuItemFromMenu(connection, deleteItemId);
        }
        return "Invalid delete command format.";
    }

    private static string HandleRolloutCommand(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 2)
        {
            string[] itemIdsStr = parts.Skip(1).ToArray();

            var rolloutRepository = new RolloutRepository(connection);
            var notificationService = new NotificationService(connection);

            var rolloutManager = new RolloutManager(rolloutRepository, notificationService);

            return rolloutManager.RolloutFoodItemsForNextDay(connection, itemIdsStr);
        }
        return "Invalid rollout command format.";
    }

    private static string HandleFetchFeedback(MySqlConnection connection)
    {
        FeedbackService feedbackServiceFetch = new FeedbackService();
        return feedbackServiceFetch.FetchFeedbackItems(connection);
    }

    private static string HandleFetchEmployeeSelections(MySqlConnection connection)
    {
        var employeeSelectionRepository = new EmployeeSelectionRepository(connection);
        var employeeSelectionService = new EmployeeSelectionService(employeeSelectionRepository);
        return employeeSelectionService.FetchEmployeeSelectionsForToday();
    }

    private static string HandleFetchRolloutItems(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 2)
        {
            string username = parts[1];
            var rolloutOperations = new RolloutOperations(connection);
            return rolloutOperations.FetchRolloutItemsWithFeedback(username);
        }
        return "Invalid command format for fetching rollout items.";
    }

    private static string HandleSelectItem(string[] parts, MySqlConnection connection)
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
                return $"User '{username}' not found.";
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
        return "No rollout items selected.";
    }

    private static string HandleSendFeedbackForm(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 2)
        {
            FeedbackService feedbackServiceForm = new FeedbackService();
            string foodItem = string.Join(" ", parts.Skip(1));
            return feedbackServiceForm.SubmitFeedback(connection, foodItem);
        }
        return "Invalid format for sending feedback form.";
    }

    private static string HandleSubmitFeedback(string[] parts, MySqlConnection connection)
    {
        string commandArgs = string.Join(" ", parts.Skip(1));
        Regex feedbackRegex = new Regex("\"(.*?)\"\\s(\\d)\\s\"(.*?)\"");

        Match match = feedbackRegex.Match(commandArgs);
        if (match.Success)
        {
            FeedbackService feedbackServiceSubmit = new FeedbackService();
            string itemName = match.Groups[1].Value;
            int rating = int.Parse(match.Groups[2].Value);
            string comments = match.Groups[3].Value;

            Console.WriteLine($"Parsed Feedback - Item: {itemName}, Rating: {rating}, Comments: {comments}");

            return feedbackServiceSubmit.FillFeedbackForm(connection, itemName, rating, comments);
        }
        return "Invalid feedback submission format.";
    }

    private static string HandleLogout(string[] parts, MySqlConnection connection)
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

    private static string HandleFetchEmployeeNotification(MySqlConnection connection)
    {
        var notificationService = new NotificationService(connection);
        return notificationService.GetEmployeeNotification((int)RoleEnum.Employee);
    }

    private static string HandleFetchChefNotification(MySqlConnection connection)
    {
        var notificationService = new NotificationService(connection);
        return notificationService.GetChefNotification((int)RoleEnum.Chef);
    }

    private static string HandleFetchNotifications(MySqlConnection connection)
    {
        int userTypeId = 3;
        return DiscardMenu.FetchTodayNotificationsForEmployees(connection, userTypeId);
    }

    private static string HandleFetchDiscardMenuItems(MySqlConnection connection)
    {
        return DiscardMenu.FetchMenuItemsWithNegativeFeedback(connection);
    }

    private static string HandleRemoveDiscardMenuItem(string[] parts, MySqlConnection connection)
    {
        if (parts.Length > 1)
        {
            string itemName = string.Join(" ", parts.Skip(1));
            itemName = itemName.Trim();
            DiscardMenu.RemoveMenuItem(connection, itemName);
            return $"Removed {itemName} from discard menu items.";
        }
        else
        {
            return "Invalid command format for removing discard menu item.";
        }

    }

    private static string HandleGetDetailedFeedback(string[] parts, MySqlConnection connection)
    {
        if (parts.Length > 1)
        {
            string itemName = parts[1];
            return DiscardMenu.RollOutFoodForDetailedFeedback(connection, itemName);
        }
        else
        {
            return "Invalid command format for getting detailed feedback.";
        }
    }

    private static string HandleRollOutFeedback(string[] parts, MySqlConnection connection)
    {
        if (parts.Length > 1)
        {
            string itemName = string.Join(" ", parts.Skip(1)).Trim();
            return DiscardMenu.RollOutFoodForDetailedFeedback(connection, itemName);
        }
        else
        {
            return "Invalid command format for rolling out food for feedback.";
        }
    }

    private static string HandleSubmitDetailedFeedback(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 5)
        {

            string feedbackCommand = string.Join(" ", parts.Skip(1));
            var feedbackParts = ParseFeedbackCommand(feedbackCommand);

            if (feedbackParts.Length == 4)
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

    private static string HandleUpdateProfile(string[] parts, MySqlConnection connection)
    {
        if (parts.Length >= 6)
        {
            string username = parts[1];
            string remainingInput = string.Join(" ", parts.Skip(2));
            string[] preferencesParts = Regex.Matches(remainingInput, @"[\""].+?[\""]|[^ ]+")
                                              .Cast<Match>()
                                              .Select(m => m.Value.Trim('"'))
                                              .ToArray();

            if (preferencesParts.Length >= 4)
            {
                try
                {
                    string preference = preferencesParts[0];
                    string spiceLevel = preferencesParts[1];
                    string cuisinePreference = preferencesParts[2];
                    bool sweetTooth = preferencesParts[3].Equals("true", StringComparison.OrdinalIgnoreCase);


                    var profileRepository = new EmployeeProfileRepository();
                    var profileOperations = new EmployeeProfileOperations();


                    int userId = profileRepository.GetUserIdByUsername(connection, username);
                    Console.WriteLine($"Username: {username}, UserID: {userId}");

                    if (userId != -1)
                    {

                        var profileData = new UserProfile
                        {
                            UserID = userId,
                            Preference = preference,
                            SpiceLevel = spiceLevel,
                            CuisinePreference = cuisinePreference,
                            SweetTooth = sweetTooth
                        };

                        return profileOperations.SaveProfile(connection, profileData);
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

    private static string HandleFetchDetailedFeedbackQuestions(string[] parts, MySqlConnection connection)
    {
        if (parts.Length > 1)
        {
            string itemName = string.Join(" ", parts.Skip(1)).Trim();
            return DiscardMenu.GetFeedbackQuestionsForItem(itemName);
        }
        else
        {
            return "Invalid command format for fetching detailed feedback questions.";
        }
    }

    private static string HandleFetchFeedback_DiscardMenuItems(MySqlConnection connection)
    {
        return DiscardMenu.FetchCurrentRolloutFeedbackItems(connection);
    }
    private static string[] ParseFeedbackCommand(string commandText)
    {
        var matches = System.Text.RegularExpressions.Regex.Matches(commandText, @"(?<=^| )\""(\\.|[^\""])*\""|[^ ]+");
        return matches.Select(m => m.Value.Trim('"')).ToArray();
    }
}
