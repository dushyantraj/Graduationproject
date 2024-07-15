using System;
using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using CafeteriaServer.Repositories;
using CafeteriaServer.Services;

namespace CafeteriaServer.Operations
{
    public class RolloutManager
    {
        private readonly RolloutRepository _rolloutRepository;
        private readonly NotificationService _notificationService;

        public RolloutManager(RolloutRepository rolloutRepository, NotificationService notificationService)
        {
            _rolloutRepository = rolloutRepository ?? throw new ArgumentNullException(nameof(rolloutRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public string RolloutFoodItemsForNextDay(MySqlConnection connection, string[] itemIds)
        {
            try
            {
                int successfulCount = 0;
                DateTime today = DateTime.Today;

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    foreach (var itemIdStr in itemIds)
                    {
                        if (int.TryParse(itemIdStr, out int itemId))
                        {
                            if (!_rolloutRepository.IsItemRolledOutToday(itemId, today, transaction))
                            {
                                MenuItem menuItem = _rolloutRepository.GetMenuItem(itemId, transaction);

                                if (menuItem != null)
                                {
                                    _rolloutRepository.InsertRolledOutItem(itemId, menuItem, today, transaction);
                                    successfulCount++;

                                    var message = $"Item '{menuItem.Name}' has been rolled out for the next day.";
                                    var notification = new Notification
                                    {
                                        Message = message,
                                        Role = (int)RoleEnum.Employee, // Assuming UserRole enum exists
                                        Date = DateTime.Now
                                    };
                                    _notificationService.SendNotification(notification);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Item with ID {itemId} has already been rolled out today.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid itemId: {itemIdStr}. Skipping.");
                        }
                    }
                    transaction.Commit();
                }

                return GenerateRolloutMessage(successfulCount, itemIds.Length);
            }
            catch (MySqlException ex)
            {
                LogException("Error rolling out items", ex);
                return "Error rolling out items: Database error.";
            }
            catch (Exception ex)
            {
                LogException("Error rolling out items", ex);
                return "Error rolling out items: " + ex.Message;
            }
        }

        private string GenerateRolloutMessage(int successfulCount, int totalItems)
        {
            if (successfulCount == totalItems)
                return "Items rolled out for next day successfully.";
            else if (successfulCount > 0)
                return "Some items were already rolled out today. Only new items were added.";
            else
                return "All selected items were already rolled out today.";
        }

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
