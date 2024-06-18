// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using MySql.Data.MySqlClient;
// using System.Data;
// using System.Text.RegularExpressions;  
// namespace MenuServer
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             // MySQL database connection settings
//             string connectionString = "Server=localhost;Database=CafeteriaDB;Uid=root;Pwd=Admin@123;";
//             MySqlConnection connection = new MySqlConnection(connectionString);

//             try
//             {
//                 // Open the database connection
//                 connection.Open();
//                 Console.WriteLine("Connected to MySQL database.");

//                 // Start server
//                 TcpListener server = new TcpListener(IPAddress.Any, 13000);
//                 server.Start();
//                 Console.WriteLine("Server started. Listening for clients...");

//                 while (true)
//                 {
//                     TcpClient client = server.AcceptTcpClient();
//                     Console.WriteLine("Client connected.");

//                     NetworkStream stream = client.GetStream();
//                     byte[] buffer = new byte[1024];
//                     int bytesRead = stream.Read(buffer, 0, buffer.Length);
//                     string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
//                     Console.WriteLine("Received from client: {0}", data);

//                     string[] parts = data.Split(' ');
//                     string command = parts[0];

//                     string response = "";
//                     switch (command)
//                     {
//                         case "LOGIN":
//                             if (parts.Length >= 3)
//                             {
//                                 string username = parts[1];
//                                 string password = parts[2];
//                                 response = LoginUser(connection, username, password);
//                             }
//                             else
//                             {
//                                 response = "Invalid login command format.";
//                             }
//                             break;
//                         case "FETCH":
//                             response = FetchMenuItemsWithFeedback(connection);
//                             break;
//                        case "FETCH_ROLLOUT_Feedback":
//                             response = FetchRolloutItemsWithFeedback(connection);
//                             break;
//                         case "ADD":
//                             if (parts.Length >= 5)
//                             {
//                                 string menuType = parts[1]; // Breakfast, Lunch, Dinner
//                                 string itemName = parts[2];
//                                 if (decimal.TryParse(parts[3], out decimal price))
//                                 {
//                                     int available = int.Parse(parts[4]);
//                                     response = AddMenuItem(connection, menuType, itemName, price, available);
//                                 }
//                                 else
//                                 {
//                                     response = "Invalid price format.";
//                                 }
//                             }
//                             else
//                             {
//                                 response = "Invalid command format.";
//                             }
//                             break;
//                         case "UPDATE":
//                             if (parts.Length >= 5)
//                             {
//                                 int itemId = int.Parse(parts[1]);
//                                 string itemName = parts[2];
//                                 if (decimal.TryParse(parts[3], out decimal price))
//                                 {
//                                     int available = int.Parse(parts[4]);
//                                     response = UpdateMenuItem(connection, itemId, itemName, price, available);
//                                 }
//                                 else
//                                 {
//                                     response = "Invalid price format.";
//                                 }
//                             }
//                             else
//                             {
//                                 response = "Invalid command format.";
//                             }
//                             break;
                            
//                         case "DELETE":
//                             if (parts.Length == 2 && int.TryParse(parts[1], out int deleteItemId))
//                             {
//                                 response = DeleteMenuItem(connection, deleteItemId);
//                             }
//                             else
//                             {
//                                 response = "Invalid delete command format.";
//                             }
//                             break;
//                         case "ROLLOUT":
//                             if (parts.Length >= 2)
//                             {
//                                 string[] itemIdsStr = new string[parts.Length - 1];
//                                 Array.Copy(parts, 1, itemIdsStr, 0, parts.Length - 1);
//                                 response = RolloutFoodItemsForNextDay(connection, itemIdsStr);
//                             }
//                             else
//                             {
//                                 response = "Invalid rollout command format.";
//                             }
//                             break;
//                             case "FETCH_FEEDBACK":
//                             response = FetchFeedbackItems(connection);
//                             break;
//                         case "FETCH_EMPLOYEE_SELECTIONS":
//                             response = FetchEmployeeSelections(connection);
//                             break;
//                             case "FETCH_ROLLOUT":
//                             response = FetchRolloutItemsWithFeedback(connection);
//                             break;
//                             case "SELECT_ITEM":
//                             if (parts.Length == 2 && int.TryParse(parts[1], out int selectedRolloutId))
//                             {
//                                 response = SelectFoodItemForNextDay(connection, selectedRolloutId);
//                             }
//         else
//         {
//             response = "Invalid selection command format.";
//         }
//         break;
//       case "SEND_FEEDBACK_FORM":
//     if (parts.Length >= 2)
//     {
//         string foodItem = string.Join(" ", parts, 1, parts.Length - 1);
//         response = SubmitFeedback(connection, foodItem);
//     }
//     else
//     {
//         response = "Invalid format for sending feedback form.";
//     }
//     break;
//    case "SUBMIT_FEEDBACK":
//     // Combine the command arguments to handle quotes correctly
//     string commandArgs = string.Join(" ", parts.Skip(1));

//     // Use regular expressions to match the quoted strings
//     // Pattern breakdown:
//     // \"(.*?)\" - Match quoted strings for itemName and comments
//     // \\s(\\d) - Match the single digit for rating after a space
//     Regex feedbackRegex = new Regex("\"(.*?)\"\\s(\\d)\\s\"(.*?)\"");

//     Match match = feedbackRegex.Match(commandArgs);
//     if (match.Success)
//     {
//         string itemName = match.Groups[1].Value; // Group 1: itemName
//         int rating = int.Parse(match.Groups[2].Value); // Group 2: rating
//         string comments = match.Groups[3].Value; // Group 3: comments

//         // Log for debugging
//         Console.WriteLine($"Parsed Feedback - Item: {itemName}, Rating: {rating}, Comments: {comments}");

//         // Call the method to handle feedback
//         response = FillFeedbackForm(connection, itemName, rating, comments);
//     }
//     else
//     {
//         response = "Invalid feedback format. Please provide a valid format.";
//     }
//     break;


//                       default:
//                             response = "Invalid command.";
//                             break;
//                     }

//                     // Send response back to client
//                     SendResponseToClient(stream, response);
//                     Console.WriteLine("Sent to client: {0}", response);

//                     // Clean up
//                     stream.Close();
//                     client.Close();
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Error: {0}", ex.Message);
//             }
//             finally
//             {
//                 // Close database connection
//                 if (connection != null && connection.State == System.Data.ConnectionState.Open)
//                 {
//                     connection.Close();
//                     Console.WriteLine("Disconnected from MySQL database.");
//                 }
//             }
//         }

//         static void SendResponseToClient(NetworkStream stream, string response)
//         {
//             byte[] responseBytes = Encoding.UTF8.GetBytes(response);
//             stream.Write(responseBytes, 0, responseBytes.Length);
//         }

// // New FillFeedbackForm method
// private static string FillFeedbackForm(MySqlConnection connection, string itemName, int rating, string comments)
// {
//     try
//     {
//         FetchFeedbackItems(connection);
//         // Check if the item exists in the Feedback table
//         string selectQuery = "SELECT feedback_id FROM Feedback WHERE item_name = @itemName";
//         MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
//         selectCmd.Parameters.AddWithValue("@itemName", itemName);

//         // Only open the connection if it's not already open
//         if (connection.State == System.Data.ConnectionState.Closed)
//         {
//             connection.Open();
//         }

//         object feedbackIdObj = selectCmd.ExecuteScalar();

//         if (feedbackIdObj != null)
//         {
//             int feedbackId = Convert.ToInt32(feedbackIdObj);

//             // Insert new feedback into FeedbackDetails table
//             string insertQuery = "INSERT INTO FeedbackDetails (feedback_id, rating, comments) VALUES (@feedbackId, @rating, @comments)";
//             MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
//             insertCmd.Parameters.AddWithValue("@feedbackId", feedbackId);
//             insertCmd.Parameters.AddWithValue("@rating", rating);
//             insertCmd.Parameters.AddWithValue("@comments", comments);

//             int rowsAffected = insertCmd.ExecuteNonQuery();

//             return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
//         }
//         else
//         {
//             return "Item not found. Please provide valid food item name.";
//         }
//     }
//     catch (Exception ex)
//     {
//         return $"Error filling feedback form: {ex.Message}";
//     }
//     finally
//     {
//         if (connection.State == System.Data.ConnectionState.Open)
//         {
//             connection.Close();
//         }
//     }
// }
// // Function to fetch the list of food items selected by chef for feedback
//  static string FetchFeedbackItems(MySqlConnection connection)
//     {
//         try
//         {
//             StringBuilder sb = new StringBuilder();
//             string query = "SELECT item_name FROM Feedback";
//             MySqlCommand cmd = new MySqlCommand(query, connection);
//             MySqlDataReader reader = cmd.ExecuteReader();

//             while (reader.Read())
//             {
//                 string itemName = reader.GetString("item_name");
//                 sb.AppendLine(itemName);
//             }

//             reader.Close();

//             return sb.ToString();
//         }
//         catch (Exception ex)
//         {
//             return "Error fetching feedback items: " + ex.Message;
//         }
//     }

//        static string LoginUser(MySqlConnection connection, string username, string password)
// {
//     try
//     {
//         string query = "SELECT RoleID FROM Users WHERE Username = @username AND Password = @password";
//         MySqlCommand cmd = new MySqlCommand(query, connection);
//         cmd.Parameters.AddWithValue("@username", username);
//         cmd.Parameters.AddWithValue("@password", password);

//         object result = cmd.ExecuteScalar();

//         if (result != null)
//         {
//             int roleId = Convert.ToInt32(result);
//             string roleName = GetRoleName(connection, roleId);

//             // Ensure the role is valid for the application
//             if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
//                 roleName.Equals("Chef", StringComparison.OrdinalIgnoreCase) ||
//                 roleName.Equals("Employee", StringComparison.OrdinalIgnoreCase))
//             {
//                 return $"LOGIN_SUCCESS {roleName}";
//             }
//             else
//             {
//                 return "LOGIN_FAILURE Invalid role.";
//             }
//         }
//         else
//         {
//             return "LOGIN_FAILURE Invalid credentials.";
//         }
//     }
//     catch (Exception ex)
//     {
//         return "Error during login: " + ex.Message;
//     }
// }

//         static string GetRoleName(MySqlConnection connection, int roleId)
//         {
//             try
//             {
//                 string query = "SELECT RoleName FROM Roles WHERE RoleID = @roleId";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@roleId", roleId);

//                 object result = cmd.ExecuteScalar();

//                 if (result != null)
//                 {
//                     return result.ToString();
//                 }
//                 else
//                 {
//                     return "Unknown";
//                 }
//             }
//             catch (Exception)
//             {
//                 return "Unknown";
//             }
//         }
// public static string FetchMenuItemsWithFeedback(MySqlConnection connection)
// {
//     try
//     {
//         if (connection.State == ConnectionState.Closed)
//         {
//             connection.Open();
//         }

//         // Fetch menu items
//         string menuQuery = "SELECT item_id, name, price, available FROM MenuItem";
//         MySqlCommand menuCmd = new MySqlCommand(menuQuery, connection);

//         var menuItems = new Dictionary<int, (string Name, decimal Price, int Available)>();

//         using (var reader = menuCmd.ExecuteReader())
//         {
//             while (reader.Read())
//             {
//                 int itemId = reader.GetInt32("item_id");
//                 string name = reader.GetString("name");
//                 decimal price = reader.GetDecimal("price");
//                 int available = reader.GetInt32("available");

//                 menuItems[itemId] = (name, price, available);
//             }
//         }
//    // Fetch feedback details
//         string feedbackQuery = "SELECT f.item_name, fd.rating, fd.comments, fd.created_at FROM Feedback f " +
//                                "JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";
//         MySqlCommand feedbackCmd = new MySqlCommand(feedbackQuery, connection);

//         var feedbackDict = new Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>>();

//         using (var reader = feedbackCmd.ExecuteReader())
//         {
//             while (reader.Read())
//             {
//                 string itemName = reader.GetString("item_name").Trim();
//                 double rating = reader.GetDouble("rating");
//                 string comment = reader.GetString("comments");
//                 DateTime createdAt = reader.GetDateTime("created_at");

//                 if (!feedbackDict.ContainsKey(itemName))
//                 {
//                     feedbackDict[itemName] = new List<(double Rating, string Comment, DateTime CreatedAt)>();
//                 }

//                 feedbackDict[itemName].Add((rating, comment, createdAt));
//             }
//         }
//     StringBuilder response = new StringBuilder();

//         foreach (var item in menuItems)
//         {
//             int itemId = item.Key;
//             string itemName = item.Value.Name;
//             decimal price = item.Value.Price;
//             int available = item.Value.Available;

//             response.AppendLine($"Item ID: {itemId}, Name: {itemName}, Price: {price:F2}, Available: {available}");

//             if (feedbackDict.ContainsKey(itemName))
//             {
//                 var feedbackEntries = feedbackDict[itemName];

//                 if (feedbackEntries.Count > 0)
//                 {
//                     var (averageRating, overallSentiment, recommendation) = AnalyzeSentimentsAndRatings(feedbackEntries);

//                     response.AppendLine($"Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}");
        
//                 }
//             }
//             else
//             {
//                 response.AppendLine($"  Average Rating: N/A, Overall Sentiment: N/A");
//             }
//         }

//         return response.ToString();
//     }
//     catch (Exception ex)
//     {
//         return $"Error fetching menu items: {ex.Message}";
//     }
//     finally
//     {
//         if (connection.State == ConnectionState.Open)
//         {
//             connection.Close();
//         }
//     }
// }
// private static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
// {
//     // Call CalculateOverallSentimentsAndRatings to get overall metrics
//     var overallMetrics = CalculateOverallSentimentsAndRatings(entries);

//     // Return the overall metrics
//     return overallMetrics;
// }

// public static (double AverageRating, string OverallSentiment, string Recommendation) CalculateOverallSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
// {
//     // Define sentiment features for analysis
//     Dictionary<string, List<string>> sentimentFeatures = new Dictionary<string, List<string>>
//     {
//         { "Positive", new List<string> { "delicious", "amazing", "great", "fantastic", "excellent", "good", "tasty", "wonderful", "superb", "awesome", "very good" } },
//         { "Negative", new List<string> { "bad", "terrible", "disappointing", "awful", "poor", "tasteless", "horrible", "gross", "unpleasant", "mediocre", "not good", "bad taste" } }
//     };

//     int positiveCount = 0;
//     int negativeCount = 0;
//     int neutralCount = 0;

//     double totalRating = 0;

//     foreach (var entry in entries)
//     {
//         string comment = entry.Comment;
//         int score = 0;

//         // Calculate sentiment score
//         foreach (var kvp in sentimentFeatures)
//         {
//             foreach (var feature in kvp.Value)
//             {
//                 if (comment.Contains(feature, StringComparison.OrdinalIgnoreCase))
//                 {
//                     score += kvp.Key == "Positive" ? 1 : -1;
//                 }
//             }
//         }

//         // Count positive, negative, and neutral sentiments
//         if (score > 0)
//         {
//             positiveCount++;
//         }
//         else if (score < 0)
//         {
//             negativeCount++;
//         }
//         else
//         {
//             neutralCount++;
//         }

//         // Accumulate ratings to calculate average
//         totalRating += entry.Rating;
//     }

//     // Calculate average rating
//     double averageRating = entries.Count > 0 ? totalRating / entries.Count : 0.0;

//     // Determine overall sentiment
//     string overallSentiment = "Neutral";
//     if (positiveCount > negativeCount && positiveCount > neutralCount)
//     {
//         overallSentiment = "Positive";
//     }
//     else if (negativeCount > positiveCount && negativeCount > neutralCount)
//     {
//         overallSentiment = "Negative";
//     }
//     else if (positiveCount > 0 && negativeCount > 0)
//     {
//         overallSentiment = "Mixed";
//     }

//     // Determine recommendation based on average rating
//     string recommendation = averageRating >= 4.0 ? "Highly recommended" :
//                             averageRating >= 3.0 ? "Recommended" :
//                             averageRating >= 2.0 ? "Average" :
//                             "Not recommended";

//     return (averageRating, overallSentiment, recommendation);
// }

//         static string AddMenuItem(MySqlConnection connection, string menuType, string itemName, decimal price, int available)
//         {
//             try
//             {
//                 // Get menu_id for the given menuType
//                 int menuId = GetMenuId(connection, menuType);
//                 if (menuId == -1)
//                     return "Invalid menu type.";

//                 string query = $"INSERT INTO MenuItem (menu_id, name, price, available) VALUES (@menuId, @itemName, @price, @available)";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@menuId", menuId);
//                 cmd.Parameters.AddWithValue("@itemName", itemName);
//                 cmd.Parameters.AddWithValue("@price", price);
//                 cmd.Parameters.AddWithValue("@available", available);

//                 int rowsAffected = cmd.ExecuteNonQuery();

//                 if (rowsAffected > 0)
//                     return "New item added successfully.";
//                 else
//                     return "Failed to add new item.";
//             }
//             catch (Exception ex)
//             {
//                 return "Error adding item: " + ex.Message;
//             }
//         }

//         static int GetMenuId(MySqlConnection connection, string menuType)
//         {
//             try
//             {
//                 string query = $"SELECT menu_id FROM Menu WHERE menu_type = @menuType";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@menuType", menuType);

//                 object result = cmd.ExecuteScalar();
//                 if (result != null)
//                     return Convert.ToInt32(result);
//                 else
//                     return -1; // Invalid menu type
//             }
//             catch (Exception)
//             {
//                 return -1;
//             }
//         }
//     static string UpdateMenuItem(MySqlConnection connection, int itemId, string itemName, decimal price, int available)
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

//                 if (rowsAffected > 0)
//                     return "Item updated successfully.";
//                 else
//                     return "Failed to update item.";
//             }
//             catch (Exception ex)
//             {
//                 return "Error updating item: " + ex.Message;
//             }
//         }

//         static string DeleteMenuItem(MySqlConnection connection, int itemId)
//         {
//             try
//             {
//                 string query = "DELETE FROM MenuItem WHERE item_id = @itemId";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 cmd.Parameters.AddWithValue("@itemId", itemId);

//                 int rowsAffected = cmd.ExecuteNonQuery();

//                 if (rowsAffected > 0)
//                       return "Item deleted successfully.";
//                 else
//                     return "Failed to delete item.";
//             }
//             catch (Exception ex)
//             {
//                 return "Error deleting item: " + ex.Message;
//             }
//         }

//  static string RolloutFoodItemsForNextDay(MySqlConnection connection, string[] itemIds)
// {
//     try
//     {
//         FetchMenuItemsWithFeedback(connection);
//         int successfulCount = 0;
//         DateTime today = DateTime.Today; // Get today's date without the time component

//         using (MySqlTransaction transaction = connection.BeginTransaction())
//         {
//             foreach (var itemIdStr in itemIds)
//             {
//                 if (int.TryParse(itemIdStr, out int itemId))
//                 {
//                     // Check if the item has already been rolled out today
//                     string checkQuery = "SELECT COUNT(*) FROM RolloutItems WHERE item_id = @itemId AND date_rolled_out = @today";
//                     MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection, transaction);
//                     checkCmd.Parameters.AddWithValue("@itemId", itemId);
//                     checkCmd.Parameters.AddWithValue("@today", today);

//                     long count = (long)checkCmd.ExecuteScalar();

//                     if (count == 0)
//                     {
//                         // Get item details
//                         string getItemQuery = "SELECT * FROM MenuItem WHERE item_id = @itemId";
//                         MySqlCommand getItemCmd = new MySqlCommand(getItemQuery, connection, transaction);
//                         getItemCmd.Parameters.AddWithValue("@itemId", itemId);

//                         // Use a `using` block to ensure the reader is closed after use
//                         using (MySqlDataReader reader = getItemCmd.ExecuteReader())
//                         {
//                             if (reader.Read())
//                             {
//                                 string itemName = reader.GetString("name");
//                                 decimal price = reader.GetDecimal("price");
//                                 int available = reader.GetInt32("available");

//                                 // Close the reader before the next command
//                                 reader.Close();

//                                 // Insert into RolloutItems table
//                                 string insertQuery = "INSERT INTO RolloutItems (item_id, item_name, price, available, selected_for_next_day, date_rolled_out) " +
//                                                      "VALUES (@itemId, @itemName, @price, @available, true, @today)";
//                                 MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction);
//                                 insertCmd.Parameters.AddWithValue("@itemId", itemId);
//                                 insertCmd.Parameters.AddWithValue("@itemName", itemName);
//                                 insertCmd.Parameters.AddWithValue("@price", price);
//                                 insertCmd.Parameters.AddWithValue("@available", available);
//                                 insertCmd.Parameters.AddWithValue("@today", today);

//                                 int rowsAffected = insertCmd.ExecuteNonQuery();

//                                 if (rowsAffected > 0)
//                                     successfulCount++;
//                             }
//                             else
//                             {
//                                 // Ensure to close the reader if no data is read
//                                 reader.Close();
//                             }
//                         }
//                     }
//                     else
//                     {
//                         Console.WriteLine($"Item with ID {itemId} has already been rolled out today.");
//                     }
//                 }
//             }

//             // Commit the transaction if no exceptions
//             transaction.Commit();
//         }

//         if (successfulCount == itemIds.Length)
//             return "Items rolled out for next day successfully.";
//         else if (successfulCount > 0)
//             return "Some items were already rolled out today. Only new items were added.";
//         else
//             return "All selected items were already rolled out today.";
//     }
//     catch (Exception ex)
//     {
//         return "Error rolling out item: " + ex.Message;
//     }
// }

// static string SubmitFeedback(MySqlConnection connection, string itemName)
// {
//     try
//     {
//         // Trim the itemName to remove leading and trailing spaces
//         itemName = itemName.Trim();

//         if (string.IsNullOrEmpty(itemName))
//         {
//             return "Item name cannot be empty.";
//         }

//         string query = "INSERT INTO Feedback (item_name) VALUES (@itemName)";
//         MySqlCommand cmd = new MySqlCommand(query, connection);
//         cmd.Parameters.AddWithValue("@itemName", itemName);

//         int rowsAffected = cmd.ExecuteNonQuery();

//         return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
//     }
//     catch (Exception ex)
//     {
//         return "Error submitting feedback: " + ex.Message;
//     }
// }
//    static string FetchRolloutItems(MySqlConnection connection)
//         {
//             try
//             {
//                 StringBuilder sb = new StringBuilder();
//                 string query = "SELECT * FROM RolloutItems WHERE selected_for_next_day = true";
//                 MySqlCommand cmd = new MySqlCommand(query, connection);
//                 MySqlDataReader reader = cmd.ExecuteReader();

//                 while (reader.Read())
//                 {
//                     int rolloutId = reader.GetInt32("rollout_id");
//                     string itemName = reader.GetString("item_name");
//                     decimal price = reader.GetDecimal("price");

//                     sb.AppendLine($"Rollout ID: {rolloutId}, Item Name: {itemName}, Price: {price}");
//                 }

//                 reader.Close();
//                 return sb.ToString();
//             }
//             catch (Exception ex)
//             {
//                 return "Error fetching rollout items: " + ex.Message;
//             }
//         }

// static string FetchEmployeeSelections(MySqlConnection connection)
// {
//     try
//     {
//         StringBuilder sb = new StringBuilder();
//         string query = "SELECT es.*, ri.item_name FROM EmployeeSelections es JOIN RolloutItems ri ON es.rollout_id = ri.rollout_id";
//         MySqlCommand cmd = new MySqlCommand(query, connection);
//         MySqlDataReader reader = cmd.ExecuteReader();

//         while (reader.Read())
//         {
//             int userId = reader.GetInt32("user_id");
//             int rolloutId = reader.GetInt32("rollout_id");
//             string itemName = reader.GetString("item_name");

//             sb.AppendLine($"Employee ID: {userId}, Rollout ID: {rolloutId}, Item Name: {itemName}");
//         }

//         reader.Close();

//         return sb.ToString();
//     }
//     catch (Exception ex)
//     {
//         return "Error fetching employee selections: " + ex.Message;
//     }
// }
// static string SelectFoodItemForNextDay(MySqlConnection connection, int rolloutId)
// {
//     try
//     {
//         // Ensure the connection is open
//         if (connection.State == ConnectionState.Closed)
//         {
//             connection.Open();
//         }

//         // Example employee user ID; replace with actual logged-in user's ID
//         int userId = 3;

//         // Check if the rollout item exists and is available
//         string selectQuery = "SELECT * FROM RolloutItems WHERE rollout_id = @rolloutId AND available = 1";
//         MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
//         selectCmd.Parameters.AddWithValue("@rolloutId", rolloutId);

//         using (MySqlDataReader reader = selectCmd.ExecuteReader())
//         {
//             if (reader.Read())
//             {
//                 reader.Close(); // Close the reader before proceeding to the next operation

//                 // Insert the employee's selection into the EmployeeSelections table
//                 string insertQuery = "INSERT INTO EmployeeSelections (user_id, rollout_id, selected) VALUES (@userId, @rolloutId, true)";
//                 MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
//                 insertCmd.Parameters.AddWithValue("@userId", userId);
//                 insertCmd.Parameters.AddWithValue("@rolloutId", rolloutId);

//                 int rowsAffected = insertCmd.ExecuteNonQuery();

//                 if (rowsAffected > 0)
//                 {
//                     Console.WriteLine($"Employee {userId} selected rollout item {rolloutId}.");
//                     return "Your selection has been recorded.";
//                 }
//                 else
//                 {
//                     return "Failed to record your selection.";
//                 }
//             }
//             else
//             {
//                 return "Rollout item not found or not available.";
//             }
//         }
//     }
//     catch (Exception ex)
//     {
//         return "Error selecting item: " + ex.Message;
//     }
// }
// public static string FetchRolloutItemsWithFeedback(MySqlConnection connection)
// {
//     try
//     {
       
//         // Fetch rollout items
//         string rolloutQuery = "SELECT rollout_id, item_name, price, available FROM RolloutItems WHERE available = 1";
//         MySqlCommand rolloutCmd = new MySqlCommand(rolloutQuery, connection);

//         var rolloutItems = new Dictionary<int, (string ItemName, decimal Price, int Available)>();

//         using (var reader = rolloutCmd.ExecuteReader())
//         {
//             while (reader.Read())
//             {
//                 int rolloutId = reader.GetInt32("rollout_id");
//                 string itemName = reader.IsDBNull(reader.GetOrdinal("item_name")) ? "Unnamed Item" : reader.GetString("item_name");
//                 decimal price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0m : reader.GetDecimal("price");
//                 int available = reader.IsDBNull(reader.GetOrdinal("available")) ? 0 : reader.GetInt32("available");

//                 rolloutItems[rolloutId] = (itemName, price, available);
//             }
//         }
//         // Fetch feedback details
//         string feedbackQuery = @"
//             SELECT f.item_name, AVG(fd.rating) AS averageRating, COUNT(fd.feedback_id) AS totalFeedback,
//                    SUM(CASE WHEN fd.rating >= 4 THEN 1 ELSE 0 END) AS positiveCount,
//                    SUM(CASE WHEN fd.rating < 2 THEN 1 ELSE 0 END) AS negativeCount
//             FROM Feedback f
//             JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id
//             WHERE f.item_name IN (SELECT item_name FROM RolloutItems WHERE available = 1)
//             GROUP BY f.item_name";

//         MySqlCommand feedbackCmd = new MySqlCommand(feedbackQuery, connection);

//         var feedbackDict = new Dictionary<string, (double AverageRating, string OverallSentiment)>();

//         using (var reader = feedbackCmd.ExecuteReader())
//         {
//             while (reader.Read())
//             {
//                 string itemName = reader.GetString("item_name").Trim();
//                 double averageRating = reader.IsDBNull(reader.GetOrdinal("averageRating")) ? 0.0 : reader.GetDouble("averageRating");
//                 int positiveCount = reader.IsDBNull(reader.GetOrdinal("positiveCount")) ? 0 : reader.GetInt32("positiveCount");
//                 int negativeCount = reader.IsDBNull(reader.GetOrdinal("negativeCount")) ? 0 : reader.GetInt32("negativeCount");
//                 int totalFeedback = reader.IsDBNull(reader.GetOrdinal("totalFeedback")) ? 0 : reader.GetInt32("totalFeedback");

//                 string overallSentiment = CalculateOverallSentiment(positiveCount, negativeCount, totalFeedback);

//                 feedbackDict[itemName] = (averageRating, overallSentiment);
//             }
//         }
//         // Prepare the response with rollout items and their feedback
//         StringBuilder response = new StringBuilder();

//         foreach (var item in rolloutItems)
//         {
//             int rolloutId = item.Key;
//             string itemName = item.Value.ItemName;
//             decimal price = item.Value.Price;
//             int available = item.Value.Available;

//             response.AppendLine($"Rollout ID: {rolloutId}, Item Name: {itemName}, Price: {price:F2}, Available: {available}");

//             if (feedbackDict.ContainsKey(itemName))
//             {
//                 var (averageRating, overallSentiment) = feedbackDict[itemName];
//                 response.AppendLine($"  Average Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}");
//             }
//             else
//             {
//                 response.AppendLine($"  Average Rating: N/A, Overall Sentiment: N/A");
//             }
//         }

//         return response.ToString();
//     }
//     catch (Exception ex)
//     {
//         return $"Error fetching rollout items: {ex.Message}";
//     }
//     finally
//     {
//         Console.WriteLine("Closing connection...");
//     }
// }
// private static string CalculateOverallSentiment(int positiveCount, int negativeCount, int totalFeedback)
// {
//     if (totalFeedback == 0) return "N/A";

//     if (positiveCount > negativeCount)
//     {
//         return "Positive";
//     }
//     else if (negativeCount > positiveCount)
//     {
//         return "Negative";
//     }
//     else
//     {
//         return "Neutral";
//     }
// }
//            }
// }
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using CafeteriaServer.Operations;
using System.Collections.Generic;

namespace CafeteriaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // MySQL database connection settings
            string connectionString = "Server=localhost;Database=CafeteriaDB;Uid=root;Pwd=Admin@123;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                // Open the database connection
                connection.Open();
                Console.WriteLine("Connected to MySQL database.");

                // Start server
                TcpListener server = new TcpListener(IPAddress.Any, 13000);
                server.Start();
                Console.WriteLine("Server started. Listening for clients...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine("Received from client: {0}", data);

                    string[] parts = data.Split(' ');
                    string command = parts[0];

                    string response = "";
                    switch (command)
                    {
                        case "LOGIN":
                            if (parts.Length >= 3)
                            {
                                string username = parts[1];
                                string password = parts[2];
                                response = MenuOperations.LoginUser(connection, username, password);
                            }
                            else
                            {
                                response = "Invalid login command format.";
                            }
                            break;
                        case "FETCH":
                            response = MenuOperations.FetchMenuItemsWithFeedback(connection);
                            break;
                        case "FETCH_ROLLOUT_Feedback":
                            response = MenuOperations.FetchRolloutItemsWithFeedback(connection);
                            break;
                        case "ADD":
                            if (parts.Length >= 5)
                            {
                                string menuType = parts[1]; // Breakfast, Lunch, Dinner
                                string itemName = parts[2];
                                if (decimal.TryParse(parts[3], out decimal price))
                                {
                                    int available = int.Parse(parts[4]);
                                    response = MenuOperations.AddMenuItem(connection, menuType, itemName, price, available);
                                }
                                else
                                {
                                    response = "Invalid price format.";
                                }
                            }
                            else
                            {
                                response = "Invalid command format.";
                            }
                            break;
                        case "UPDATE":
                            if (parts.Length >= 5)
                            {
                                int itemId = int.Parse(parts[1]);
                                string itemName = parts[2];
                                if (decimal.TryParse(parts[3], out decimal price))
                                {
                                    int available = int.Parse(parts[4]);
                                    response = MenuOperations.UpdateMenuItem(connection, itemId, itemName, price, available);
                                }
                                else
                                {
                                    response = "Invalid price format.";
                                }
                            }
                            else
                            {
                                response = "Invalid command format.";
                            }
                            break;
                        case "DELETE":
                            if (parts.Length == 2 && int.TryParse(parts[1], out int deleteItemId))
                            {
                                response = MenuOperations.DeleteMenuItem(connection, deleteItemId);
                            }
                            else
                            {
                                response = "Invalid delete command format.";
                            }
                            break;
                        case "ROLLOUT":
                            if (parts.Length >= 2)
                            {
                                string[] itemIdsStr = new string[parts.Length - 1];
                                Array.Copy(parts, 1, itemIdsStr, 0, parts.Length - 1);
                                response = MenuOperations.RolloutFoodItemsForNextDay(connection, itemIdsStr);
                            }
                            else
                            {
                                response = "Invalid rollout command format.";
                            }
                            break;
                        case "FETCH_FEEDBACK":
                            response = MenuOperations.FetchFeedbackItems(connection);
                            break;
                        case "FETCH_EMPLOYEE_SELECTIONS":
                            response = MenuOperations.FetchEmployeeSelections(connection);
                            break;
                        case "FETCH_ROLLOUT":
                            response = MenuOperations.FetchRolloutItemsWithFeedback(connection);
                            break;
                        case "SELECT_ITEM":
                            if (parts.Length == 2 && int.TryParse(parts[1], out int selectedRolloutId))
                            {
                                response = MenuOperations.SelectFoodItemForNextDay(connection, selectedRolloutId);
                            }
                            else
                            {
                                response = "Invalid selection command format.";
                            }
                            break;
                        case "SEND_FEEDBACK_FORM":
                            if (parts.Length >= 2)
                            {
                                string foodItem = string.Join(" ", parts, 1, parts.Length - 1);
                                response = MenuOperations.SubmitFeedback(connection, foodItem);
                            }
                            else
                            {
                                response = "Invalid format for sending feedback form.";
                            }
                            break;
                        case "SUBMIT_FEEDBACK":
                            // Combine the command arguments to handle quotes correctly
                            string commandArgs = string.Join(" ", parts.Skip(1));

                            // Use regular expressions to match the quoted strings
                            // Pattern breakdown:
                            // \"(.*?)\" - Match quoted strings for itemName and comments
                            // \\s(\\d) - Match the single digit for rating after a space
                            Regex feedbackRegex = new Regex("\"(.*?)\"\\s(\\d)\\s\"(.*?)\"");

                            Match match = feedbackRegex.Match(commandArgs);
                            if (match.Success)
                            {
                                string itemName = match.Groups[1].Value; // Group 1: itemName
                                int rating = int.Parse(match.Groups[2].Value); // Group 2: rating
                                string comments = match.Groups[3].Value; // Group 3: comments

                                // Log for debugging
                                Console.WriteLine($"Parsed Feedback - Item: {itemName}, Rating: {rating}, Comments: {comments}");

                                // Call the method to handle feedback
                                response = MenuOperations.FillFeedbackForm(connection, itemName, rating, comments);
                            }
                            else
                            {
                                response = "Invalid feedback format. Please provide a valid format.";
                            }
                            break;
                        default:
                            response = "Invalid command.";
                            break;
                    }

                    // Send response back to client
                    SendResponseToClient(stream, response);
                    Console.WriteLine("Sent to client: {0}", response);

                    // Clean up
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            finally
            {
                // Close database connection
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Disconnected from MySQL database.");
                }
            }
        }

        static void SendResponseToClient(NetworkStream stream, string response)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}
