using CafeteriaServer.Models.DTO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Utilities;
using CafeteriaServer.Models;
namespace CafeteriaServer.Repositories
{
    public class MenuRepository
    {
        public Dictionary<int, ItemDTO> FetchMenuItems(MySqlConnection connection)
        {
            string query = "SELECT item_id, name, price, available FROM MenuItem";
            var menuItems = new Dictionary<int, ItemDTO>();

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int itemId = reader.GetInt32("item_id");
                        string name = reader.GetString("name");
                        decimal price = reader.GetDecimal("price");
                        int available = reader.GetInt32("available");

                        var itemDTO = new ItemDTO
                        {
                            Name = name,
                            Price = price,
                            Available = available
                        };

                        menuItems[itemId] = itemDTO;
                    }
                }
            }

            return menuItems;
        }

        public string AddMenuItem(MySqlConnection connection, AddMenuItemDTO dto)
        {
            try
            {
                int menuId = DatabaseUtilities.GetMenuIdByType(connection, dto.MenuType);
                if (menuId == -1)
                    return "Invalid menu type.";

                const string insertQuery = @"
            INSERT INTO MenuItem (menu_id, name, price, available, food_type, cuisine_preference, spice_level) 
            VALUES (@menuId, @itemName, @price, @available, @foodType, @cuisinePreference, @spiceLevel)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@menuId", menuId);
                    cmd.Parameters.AddWithValue("@itemName", dto.ItemName);
                    cmd.Parameters.AddWithValue("@price", dto.Price);
                    cmd.Parameters.AddWithValue("@available", dto.Available);
                    cmd.Parameters.AddWithValue("@foodType", dto.FoodType);
                    cmd.Parameters.AddWithValue("@cuisinePreference", dto.CuisinePreference);
                    cmd.Parameters.AddWithValue("@spiceLevel", dto.SpiceLevel);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        NotifyEmployeesAndChef(dto.ItemName, connection);
                        return "New item added successfully.";
                    }
                    else
                    {
                        return "Failed to add new item.";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding menu item: " + ex.Message);
                return "An error occurred while adding the item.";
            }
        }

        private void NotifyEmployeesAndChef(string itemName, MySqlConnection connection)
        {
            try
            {
                NotifyEmployees(itemName, connection);
                NotifyChef(itemName, connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error notifying employees and chef: " + ex.Message);
            }
        }

        private void NotifyEmployees(string itemName, MySqlConnection connection)
        {
            Notification notifyEmployees = new Notification()
            {
                Message = $"New item added: {itemName}",
                Date = DateTime.Now,
                Role = 2
            };
            AddNotification(notifyEmployees, connection);
        }

        private void NotifyChef(string itemName, MySqlConnection connection)
        {
            Notification notifyChef = new Notification()
            {
                Message = $"New item added: {itemName}",
                Date = DateTime.Now,
                Role = 3
            };
            AddNotification(notifyChef, connection);
        }

        public static void AddNotification(Notification notification, MySqlConnection connection)
        {
            try
            {
                string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) VALUES (@message, @userType_id, @notificationDateTime)";
                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@message", notification.Message);
                command.Parameters.AddWithValue("@userType_id", notification.Role);
                command.Parameters.AddWithValue("@notificationDateTime", notification.Date);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding notification: " + ex.Message);
            }
        }


        public string UpdateMenuItem(MySqlConnection connection, UpdateMenuItemDTO dto)
        {
            const string query = "UPDATE MenuItem SET price = @price, available = @available, spice_level = @spiceLevel WHERE name = @itemName";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@itemName", dto.ItemName);
                cmd.Parameters.AddWithValue("@price", dto.Price);
                cmd.Parameters.AddWithValue("@available", dto.Available);
                cmd.Parameters.AddWithValue("@spiceLevel", dto.SpiceLevel);

                return cmd.ExecuteNonQuery() > 0 ? "Item updated successfully." : "Failed to update item.";
            }
        }

        public string DeleteMenuItem(MySqlConnection connection, int itemId)
        {
            const string query = "DELETE FROM MenuItem WHERE item_id = @itemId";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@itemId", itemId);

                return cmd.ExecuteNonQuery() > 0 ? "Item deleted successfully." : "Failed to delete item.";
            }
        }
    }
}
