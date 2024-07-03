using CafeteriaServer.Models.DTO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Operations;
using CafeteriaServer.Utilities;
namespace CafeteriaServer.Services
{
    public class MenuService
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
            int menuId = DatabaseUtilities.GetMenuIdByType(connection, dto.MenuType);
            if (menuId == -1)
                return "Invalid menu type.";

            const string insertQuery = "INSERT INTO MenuItem (menu_id, name, price, available, food_type) VALUES (@menuId, @itemName, @price, @available, @foodType)";
            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@menuId", menuId);
                cmd.Parameters.AddWithValue("@itemName", dto.ItemName);
                cmd.Parameters.AddWithValue("@price", dto.Price);
                cmd.Parameters.AddWithValue("@available", dto.Available);
                cmd.Parameters.AddWithValue("@foodType", dto.FoodType);

                return cmd.ExecuteNonQuery() > 0 ? "New item added successfully." : "Failed to add new item.";
            }
        }

        public string UpdateMenuItem(MySqlConnection connection, UpdateMenuItemDTO dto)
        {
            const string query = "UPDATE MenuItem SET price = @price, available = @available WHERE name = @itemName";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@itemName", dto.ItemName);
                cmd.Parameters.AddWithValue("@price", dto.Price);
                cmd.Parameters.AddWithValue("@available", dto.Available);

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
