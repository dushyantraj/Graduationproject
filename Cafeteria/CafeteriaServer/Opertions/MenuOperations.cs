using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Models;
using CafeteriaServer.Services;
using CafeteriaServer.Utilities;
using CafeteriaServer.Recommendation;
using System.Data;
using CafeteriaServer.Models.DTO;

namespace CafeteriaServer.Operations
{
    public class MenuOperations
    {
        private readonly MenuService _menuService;
        private readonly FeedbackService _feedbackService;
        private readonly RecommendationService _recommendationService;
        private readonly SelectionService _selectionService;

        public MenuOperations()
        {
            _menuService = new MenuService();
            _feedbackService = new FeedbackService();
            _recommendationService = new RecommendationService();
            _selectionService = new SelectionService();
        }
        public string FetchMenuItemsWithFeedback(MySqlConnection connection)
        {
            try
            {
                var menuItems = _menuService.FetchMenuItems(connection);
                var feedbackDict = _feedbackService.FetchFeedback(connection);

                var recommendedItems = RecommendationService.GetRecommendedItems(menuItems, feedbackDict);


                return _recommendationService.FormatRecommendedItemsResponse(recommendedItems);
            }
            catch (Exception ex)
            {
                return $"Error fetching menu items: {ex.Message}";
            }
        }
        public string FetchMenu(MySqlConnection connection)
        {
            try
            {
                var menuItems = _menuService.FetchMenuItems(connection);
                var feedbackDict = _feedbackService.FetchAllFeedback(connection);
                return GenerateMenuResponse(menuItems, feedbackDict);
            }
            catch (Exception ex)
            {
                return $"Error fetching menu items: {ex.Message}";
            }
            finally
            {
                Console.WriteLine("Closing connection...");
            }
        }

        public string FetchRecommendedMenuItems(MySqlConnection connection)
        {
            try
            {
                var menuItems = _menuService.FetchMenuItems(connection);
                var feedbackDict = _feedbackService.FetchFeedback(connection);


                var recommendedItems = RecommendationService.GetRecommendedItems(menuItems, feedbackDict);

                return _recommendationService.FormatRecommendedItemsResponse(recommendedItems);
            }
            catch (Exception ex)
            {
                return $"Error fetching recommended menu items: {ex.Message}";
            }
        }

        public string AddMenuItemToMenu(MySqlConnection connection, string menuType, string itemName, decimal price, int available, string foodType)
        {
            try
            {
                int menuId = RelatedUtilites.GetMenuId(connection, menuType);
                if (menuId == -1)
                    return "Invalid menu type.";

                return _menuService.AddMenuItem(connection, menuId, itemName, price, available, foodType);
            }
            catch (Exception ex)
            {
                return $"Error adding menu item: {ex.Message}";
            }
        }

        public string UpdateMenuItemInMenu(MySqlConnection connection, string itemName, decimal price, int available)
        {
            try
            {
                return _menuService.UpdateMenuItem(connection, itemName, price, available);
            }
            catch (Exception ex)
            {
                return $"Error updating menu item: {ex.Message}";
            }
        }

        public string DeleteMenuItemFromMenu(MySqlConnection connection, int itemId)
        {
            try
            {
                return _menuService.DeleteMenuItem(connection, itemId);
            }
            catch (Exception ex)
            {
                return $"Error deleting menu item: {ex.Message}";
            }
        }

        public string SelectFoodItemsForNextDay(MySqlConnection connection, int userId, int[] rolloutIds)
        {
            try
            {
                EnsureConnectionOpen(connection);
                return _selectionService.SelectFoodItemsForNextDay(connection, userId, rolloutIds);
            }
            catch (Exception ex)
            {
                return $"Error selecting food items for next day: {ex.Message}";
            }
            finally
            {
                EnsureConnectionClosed(connection);
            }
        }

        private void EnsureConnectionOpen(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        private void EnsureConnectionClosed(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private string GenerateMenuResponse(
            Dictionary<int, ItemDTO> menuItems,
            Dictionary<string, List<FeedbackDTO>> feedbackDict)
        {
            StringBuilder response = new StringBuilder();
            response.AppendLine("Menu Items:");

            foreach (var item in menuItems)
            {
                int itemId = item.Key;
                string itemName = item.Value.Name;
                decimal price = item.Value.Price;
                int available = item.Value.Available;

                response.AppendLine($"Item ID: {itemId}, Name: {itemName}, Price: {price:F2}, Available: {available}");

                if (feedbackDict.ContainsKey(itemName))
                {
                    var feedbackEntries = feedbackDict[itemName];

                    if (feedbackEntries.Count > 0)
                    {
                        var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(feedbackEntries);

                        response.AppendLine($"  Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}");
                    }
                }
            }

            return response.ToString();
        }


    }
}
