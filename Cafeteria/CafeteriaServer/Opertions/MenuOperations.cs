// using MySql.Data.MySqlClient;
// using System;
// using System.Text;
// using System.Collections.Generic;
// using System.Data;

// namespace CafeteriaServer.Operations
// {
//     public static class MenuOperations
//     {
//         public static string FetchMenuItemsWithFeedback(MySqlConnection connection)
//         {
//             try
//             {
//                 if (connection.State == ConnectionState.Closed)
//                 {
//                     connection.Open();
//                 }

//                 string menuQuery = "SELECT item_id, name, price, available FROM MenuItem";
//                 MySqlCommand menuCmd = new MySqlCommand(menuQuery, connection);

//                 var menuItems = new Dictionary<int, (string Name, decimal Price, int Available)>();

//                 using (var reader = menuCmd.ExecuteReader())
//                 {
//                     while (reader.Read())
//                     {
//                         int itemId = reader.GetInt32("item_id");
//                         string name = reader.GetString("name");
//                         decimal price = reader.GetDecimal("price");
//                         int available = reader.GetInt32("available");

//                         menuItems[itemId] = (name, price, available);
//                     }
//                 }

//                 string feedbackQuery = "SELECT f.item_name, fd.rating, fd.comments, fd.created_at FROM Feedback f " +
//                                        "JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";
//                 MySqlCommand feedbackCmd = new MySqlCommand(feedbackQuery, connection);

//                 var feedbackDict = new Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>>();

//                 using (var reader = feedbackCmd.ExecuteReader())
//                 {
//                     while (reader.Read())
//                     {
//                         string itemName = reader.GetString("item_name").Trim();
//                         double rating = reader.GetDouble("rating");
//                         string comment = reader.GetString("comments");
//                         DateTime createdAt = reader.GetDateTime("created_at");

//                         if (!feedbackDict.ContainsKey(itemName))
//                         {
//                             feedbackDict[itemName] = new List<(double Rating, string Comment, DateTime CreatedAt)>();
//                         }

//                         feedbackDict[itemName].Add((rating, comment, createdAt));
//                     }
//                 }

//                 StringBuilder response = new StringBuilder();

//                 foreach (var item in menuItems)
//                 {
//                     int itemId = item.Key;
//                     string itemName = item.Value.Name;
//                     decimal price = item.Value.Price;
//                     int available = item.Value.Available;

//                     response.AppendLine($"Item ID: {itemId}, Name: {itemName}, Price: {price:F2}, Available: {available}");

//                     if (feedbackDict.ContainsKey(itemName))
//                     {
//                         var feedbackEntries = feedbackDict[itemName];
//                         if (feedbackEntries.Count > 0)
//                         {
//                             var (averageRating, overallSentiment, recommendation) = Utilities.AnalyzeSentimentsAndRatings(feedbackEntries);
//                             response.AppendLine($"Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}");
//                         }
//                     }
//                     else
//                     {
//                         response.AppendLine($"  Average Rating: N/A, Overall Sentiment: N/A");
//                     }
//                 }

//                 return response.ToString();
//             }
//             catch (Exception ex)
//             {
//                 return $"Error fetching menu items: {ex.Message}";
//             }
//             finally
//             {
//                 if (connection.State == ConnectionState.Open)
//                 {
//                     connection.Close();
//                 }
//             }
//         }

//         public static string AddMenuItem(MySqlConnection connection, string menuType, string itemName, decimal price, int available)
//         {
//             try
//             {
//                 int menuId = Utilities.GetMenuId(connection, menuType);
//                 if (menuId == -1)
//                     return "Invalid menu type.";

//                 string query = $"INSERT INTO MenuItem (menu_id, name, price, available) VALUES (@menuId, @itemName, @price, @available)";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@menuId", menuId);
//                 cmd.Parameters.AddWithValue("@itemName", itemName);
//                 cmd.Parameters.AddWithValue("@price", price);
//                 cmd.Parameters.AddWithValue("@available", available);

//                 int rowsAffected = cmd.ExecuteNonQuery();

//                 return rowsAffected > 0 ? "New item added successfully." : "Failed to add new item.";
//             }
//             catch (Exception ex)
//             {
//                 return "Error adding item: " + ex.Message;
//             }
//         }

//         public static string UpdateMenuItem(MySqlConnection connection, int itemId, string itemName, decimal price, int available)
//         {
//             try
//             {
//                 string query = "UPDATE MenuItem SET name = @itemName, price = @price, available = @available WHERE item_id = @itemId";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@itemName", itemName);
//                 cmd.Parameters.AddWithValue("@price", price);
//                 cmd.Parameters.AddWithValue("@available", available);
//                 cmd.Parameters.AddWithValue("@itemId", itemId);

//                 int rowsAffected = cmd.ExecuteNonQuery();

//                 return rowsAffected > 0 ? "Item updated successfully." : "Failed to update item.";
//             }
//             catch (Exception ex)
//             {
//                 return "Error updating item: " + ex.Message;
//             }
//         }

//         public static string DeleteMenuItem(MySqlConnection connection, int itemId)
//         {
//             try
//             {
//                 string query = "DELETE FROM MenuItem WHERE item_id = @itemId";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@itemId", itemId);

//                 int rowsAffected = cmd.ExecuteNonQuery();

//                 return rowsAffected > 0 ? "Item deleted successfully." : "Failed to delete item.";
//             }
//             catch (Exception ex)
//             {
//                 return "Error deleting item: " + ex.Message;
//             }
//         }
//     }
// }
using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace CafeteriaServer.Operations
{
    public static class MenuOperations
    {

   public static string FetchMenuItemsWithFeedback(MySqlConnection connection)
{
    try
    {
        if (connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }
        
        
        string menuQuery = "SELECT item_id, name, price, available FROM MenuItem";
        MySqlCommand menuCmd = new MySqlCommand(menuQuery, connection);

        var menuItems = new Dictionary<int, (string Name, decimal Price, int Available)>();

        using (var reader = menuCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                int itemId = reader.GetInt32("item_id");
                string name = reader.GetString("name");
                decimal price = reader.GetDecimal("price");
                int available = reader.GetInt32("available");

                menuItems[itemId] = (name, price, available);
            }
        }
        
        
        string feedbackQuery = "SELECT f.item_name, fd.rating, fd.comments, fd.created_at FROM Feedback f " +
                               "JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";
        MySqlCommand feedbackCmd = new MySqlCommand(feedbackQuery, connection);

        var feedbackDict = new Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>>();

        using (var reader = feedbackCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string itemName = reader.GetString("item_name").Trim();
                double rating = reader.GetDouble("rating");
                string comment = reader.GetString("comments");
                DateTime createdAt = reader.GetDateTime("created_at");

                if (!feedbackDict.ContainsKey(itemName))
                {
                    feedbackDict[itemName] = new List<(double Rating, string Comment, DateTime CreatedAt)>();
                }

                feedbackDict[itemName].Add((rating, comment, createdAt));
            }
        }

        var recommendedItems = new List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)>();

        foreach (var item in menuItems)
        {
            int itemId = item.Key;
            string itemName = item.Value.Name;
            decimal price = item.Value.Price;
            int available = item.Value.Available;

            if (feedbackDict.ContainsKey(itemName))
            {
                var feedbackEntries = feedbackDict[itemName];

                if (feedbackEntries.Count > 0)
                {
                    var (averageRating, overallSentiment, recommendation) = RolloutOperations.AnalyzeSentimentsAndRatings(feedbackEntries);

                    if (overallSentiment == "Positive")
                    {
                        recommendedItems.Add((itemId, itemName, price, available, averageRating, overallSentiment, recommendation));
                    }
                }
            }
        }

        
        recommendedItems = recommendedItems.OrderByDescending(i => i.AverageRating).ToList();

    
        StringBuilder response = new StringBuilder();
        response.AppendLine("Recommended Items with Positive Sentiment:");

        foreach (var item in recommendedItems)
        {
            response.AppendLine($"Item ID: {item.ItemId}, Name: {item.Name}, Price: {item.Price:F2}, Available: {item.Available}");
            response.AppendLine($"  Average Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
        }

        return response.ToString();
    }
    catch (Exception ex)
    {
        return $"Error fetching menu items: {ex.Message}";
    }
    finally
    {
        if (connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }
}

        public static string AddMenuItem(MySqlConnection connection, string menuType, string itemName, decimal price, int available)
        {
            try
            {
                int menuId = GetMenuId(connection, menuType);
                if (menuId == -1)
                    return "Invalid menu type.";

                string query = $"INSERT INTO MenuItem (menu_id, name, price, available) VALUES (@menuId, @itemName, @price, @available)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@menuId", menuId);
                cmd.Parameters.AddWithValue("@itemName", itemName);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@available", available);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return "New item added successfully.";
                else
                    return "Failed to add new item.";
            }
            catch (Exception ex)
            {
                return "Error adding item: " + ex.Message;
            }
        }
        static int GetMenuId(MySqlConnection connection, string menuType)
        {
            try
            {
                string query = $"SELECT menu_id FROM Menu WHERE menu_type = @menuType";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@menuType", menuType);

                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return -1; // Invalid menu type
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static string UpdateMenuItem(MySqlConnection connection, string itemName, decimal price, int available)
        {
            try
            {
                string query = "UPDATE MenuItem SET  price = @price, available = @available WHERE name = @itemName";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@itemName", itemName);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@available", available);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0 ? "Item updated successfully." : "Failed to update item.";
            }
            catch (Exception ex)
            {
                return "Error updating item: " + ex.Message;
            }
        }
        public static string SelectFoodItemForNextDay(MySqlConnection connection, int rolloutId)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                int userId = 3;

                string selectQuery = "SELECT * FROM RolloutItems WHERE rollout_id = @rolloutId AND available = 1";
                MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
                selectCmd.Parameters.AddWithValue("@rolloutId", rolloutId);

                using (MySqlDataReader reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reader.Close();

                        string insertQuery = "INSERT INTO EmployeeSelections (user_id, rollout_id, selected) VALUES (@userId, @rolloutId, true)";
                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
                        insertCmd.Parameters.AddWithValue("@userId", userId);
                        insertCmd.Parameters.AddWithValue("@rolloutId", rolloutId);

                        int rowsAffected = insertCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Employee {userId} selected rollout item {rolloutId}.");
                            return "Your selection has been recorded.";
                        }
                        else
                        {
                            return "Failed to record your selection.";
                        }
                    }
                    else
                    {
                        return "Rollout item not found or not available.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error selecting item: " + ex.Message;
            }
        }

        public static string DeleteMenuItem(MySqlConnection connection, int itemId)
        {
            try
            {
                string query = "DELETE FROM MenuItem WHERE item_id = @itemId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@itemId", itemId);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0 ? "Item deleted successfully." : "Failed to delete item.";
            }
            catch (Exception ex)
            {
                return "Error deleting item: " + ex.Message;
            }
        }
    }
}
