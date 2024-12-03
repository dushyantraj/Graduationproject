using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Models;

namespace CafeteriaServer.Repositories
{
    public class RolloutRepository
    {
        private readonly MySqlConnection _connection;

        public RolloutRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public Dictionary<int, RolloutItem> FetchPreferredRolloutItems(string todayString, string foodTypePreference, string cuisinePreference, string spiceLevel)
        {
            const string query = @"
                SELECT rollout_id, item_name, price, available 
                FROM RolloutItems 
                WHERE available = 1 
                AND DATE(date_rolled_out) = @today 
                AND food_type = @foodType 
                AND EXISTS (
                    SELECT 1 FROM MenuItem 
                    WHERE MenuItem.name = RolloutItems.item_name 
                    AND MenuItem.cuisine_preference = @cuisinePreference 
                    AND MenuItem.spice_level = @spiceLevel
                )";

            var rolloutItems = new Dictionary<int, RolloutItem>();

            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@today", todayString);
                cmd.Parameters.AddWithValue("@foodType", foodTypePreference);
                cmd.Parameters.AddWithValue("@cuisinePreference", cuisinePreference);
                cmd.Parameters.AddWithValue("@spiceLevel", spiceLevel);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int rolloutId = reader.GetInt32("rollout_id");
                        string itemName = reader.IsDBNull(reader.GetOrdinal("item_name")) ? "Unnamed Item" : reader.GetString("item_name").Trim();
                        decimal price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0m : reader.GetDecimal("price");
                        int available = reader.IsDBNull(reader.GetOrdinal("available")) ? 0 : reader.GetInt32("available");

                        rolloutItems[rolloutId] = new RolloutItem { ItemName = itemName, Price = price, Available = available };
                    }
                }
            }

            return rolloutItems;
        }

        public Dictionary<int, RolloutItem> FetchAllRolloutItems(string todayString, string foodTypePreference, string cuisinePreference, string spiceLevel)
        {
            const string query = @"
                SELECT rollout_id, item_name, price, available 
                FROM RolloutItems 
                WHERE available = 1 
                AND DATE(date_rolled_out) = @today";

            var rolloutItems = new Dictionary<int, RolloutItem>();

            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@today", todayString);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int rolloutId = reader.GetInt32("rollout_id");
                        string itemName = reader.IsDBNull(reader.GetOrdinal("item_name")) ? "Unnamed Item" : reader.GetString("item_name").Trim();
                        decimal price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0m : reader.GetDecimal("price");
                        int available = reader.IsDBNull(reader.GetOrdinal("available")) ? 0 : reader.GetInt32("available");

                        rolloutItems[rolloutId] = new RolloutItem { ItemName = itemName, Price = price, Available = available };
                    }
                }
            }

            return rolloutItems;
        }
        public bool IsItemRolledOutToday(int itemId, DateTime today, MySqlTransaction transaction)
        {
            string query = "SELECT COUNT(*) FROM RolloutItems WHERE item_id = @itemId AND date_rolled_out = @today";
            MySqlCommand command = new MySqlCommand(query, _connection, transaction);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@today", today);

            long count = (long)command.ExecuteScalar();
            return count > 0;
        }

        public MenuItem GetMenuItem(int itemId, MySqlTransaction transaction)
        {
            string query = "SELECT name, price, available, food_type FROM MenuItem WHERE item_id = @itemId";
            MySqlCommand command = new MySqlCommand(query, _connection, transaction);
            command.Parameters.AddWithValue("@itemId", itemId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new MenuItem
                    {
                        Name = reader.GetString("name"),
                        Price = reader.GetDecimal("price"),
                        Available = reader.GetInt32("available"),
                        FoodType = reader.IsDBNull(reader.GetOrdinal("food_type")) ? null : reader.GetString("food_type")
                    };
                }
                return null;
            }
        }

        public void InsertRolledOutItem(int itemId, MenuItem menuItem, DateTime today, MySqlTransaction transaction)
        {
            string query = "INSERT INTO RolloutItems (item_id, item_name, price, available, selected_for_next_day, date_rolled_out, food_type) " +
                           "VALUES (@itemId, @itemName, @price, @available, true, @today, @foodType)";
            MySqlCommand command = new MySqlCommand(query, _connection, transaction);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.Parameters.AddWithValue("@itemName", menuItem.Name);
            command.Parameters.AddWithValue("@price", menuItem.Price);
            command.Parameters.AddWithValue("@available", menuItem.Available);
            command.Parameters.AddWithValue("@today", today);
            command.Parameters.AddWithValue("@foodType", menuItem.FoodType);

            command.ExecuteNonQuery();
        }
    }
}
