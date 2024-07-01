using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using System;
using System.Collections.Generic;

namespace CafeteriaServer.Services
{
    public class RolloutService
    {
        public Dictionary<int, RolloutItem> FetchRolloutItems(MySqlConnection connection, string todayString, string foodTypePreference)
        {
            string query = @"
                SELECT rollout_id, item_name, price, available 
                FROM RolloutItems 
                WHERE available = 1 
                AND DATE(date_rolled_out) = @today 
                AND food_type = @foodType";

            var rolloutItems = new Dictionary<int, RolloutItem>();

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@today", todayString);
                cmd.Parameters.AddWithValue("@foodType", foodTypePreference);

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
    }
}
