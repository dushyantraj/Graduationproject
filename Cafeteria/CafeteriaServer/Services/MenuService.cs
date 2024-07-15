// using CafeteriaServer.Models.DTO;
// using MySql.Data.MySqlClient;
// using System;
// using System.Collections.Generic;
// using CafeteriaServer.Operations;
// using CafeteriaServer.Utilities;
// namespace CafeteriaServer.Services
// {
//     public class MenuService
//     {
//         public Dictionary<int, ItemDTO> FetchMenuItems(MySqlConnection connection)
//         {
//             string query = "SELECT item_id, name, price, available FROM MenuItem";

//             var menuItems = new Dictionary<int, ItemDTO>();

//             using (MySqlCommand cmd = new MySqlCommand(query, connection))
//             {
//                 using (var reader = cmd.ExecuteReader())
//                 {
//                     while (reader.Read())
//                     {
//                         int itemId = reader.GetInt32("item_id");
//                         string name = reader.GetString("name");
//                         decimal price = reader.GetDecimal("price");
//                         int available = reader.GetInt32("available");

//                         var itemDTO = new ItemDTO
//                         {
//                             Name = name,
//                             Price = price,
//                             Available = available
//                         };

//                         menuItems[itemId] = itemDTO;
//                     }
//                 }
//             }

//             return menuItems;
//         }

//         public string AddMenuItem(MySqlConnection connection, AddMenuItemDTO dto)
// {
//     int menuId = DatabaseUtilities.GetMenuIdByType(connection, dto.MenuType);
//     if (menuId == -1)
//         return "Invalid menu type.";

//     const string insertQuery = "INSERT INTO MenuItem (menu_id, name, price, available, food_type, cuisine_preference, spice_level) " +
//                                "VALUES (@menuId, @itemName, @price, @available, @foodType, @cuisinePreference, @spiceLevel)";
//     using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
//     {
//         cmd.Parameters.AddWithValue("@menuId", menuId);
//         cmd.Parameters.AddWithValue("@itemName", dto.ItemName);
//         cmd.Parameters.AddWithValue("@price", dto.Price);
//         cmd.Parameters.AddWithValue("@available", dto.Available);
//         cmd.Parameters.AddWithValue("@foodType", dto.FoodType);
//         cmd.Parameters.AddWithValue("@cuisinePreference", dto.CuisinePreference);
//         cmd.Parameters.AddWithValue("@spiceLevel", dto.SpiceLevel);

//         return cmd.ExecuteNonQuery() > 0 ? "New item added successfully." : "Failed to add new item.";
//     }
// }


//         public string UpdateMenuItem(MySqlConnection connection, UpdateMenuItemDTO dto)
//         {
//             const string query = "UPDATE MenuItem SET price = @price, available = @available, spice_level = @spiceLevel WHERE name = @itemName";
//             using (MySqlCommand cmd = new MySqlCommand(query, connection))
//             {
//                 cmd.Parameters.AddWithValue("@itemName", dto.ItemName);
//                 cmd.Parameters.AddWithValue("@price", dto.Price);
//                 cmd.Parameters.AddWithValue("@available", dto.Available);
//                 cmd.Parameters.AddWithValue("@spiceLevel", dto.SpiceLevel); // Include spice level as string

//                 return cmd.ExecuteNonQuery() > 0 ? "Item updated successfully." : "Failed to update item.";
//             }
//         }


//         public string DeleteMenuItem(MySqlConnection connection, int itemId)
//         {
//             const string query = "DELETE FROM MenuItem WHERE item_id = @itemId";
//             using (MySqlCommand cmd = new MySqlCommand(query, connection))
//             {
//                 cmd.Parameters.AddWithValue("@itemId", itemId);

//                 return cmd.ExecuteNonQuery() > 0 ? "Item deleted successfully." : "Failed to delete item.";
//             }
//         }
//     }
// }
using CafeteriaServer.Models.DTO;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using CafeteriaServer.Repositories;

namespace CafeteriaServer.Services
{
    public class MenuService
    {
        private readonly MenuRepository _menuRepository;

        public MenuService()
        {
            _menuRepository = new MenuRepository();
        }

        public Dictionary<int, ItemDTO> FetchMenuItems(MySqlConnection connection)
        {
            return _menuRepository.FetchMenuItems(connection);
        }

        public string AddMenuItem(MySqlConnection connection, AddMenuItemDTO dto)
        {
            return _menuRepository.AddMenuItem(connection, dto);
        }

        public string UpdateMenuItem(MySqlConnection connection, UpdateMenuItemDTO dto)
        {
            return _menuRepository.UpdateMenuItem(connection, dto);
        }

        public string DeleteMenuItem(MySqlConnection connection, int itemId)
        {
            return _menuRepository.DeleteMenuItem(connection, itemId);
        }
    }
}
