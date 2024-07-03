using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Models.DTO;
using CafeteriaServer.Services;
using CafeteriaServer.Recommendation;
namespace CafeteriaServer.Operations
{
    public class MenuOperations
    {
        private readonly MenuService _menuService;
        private readonly FeedbackService _feedbackService;
        private readonly RecommendationService _recommendationService;
        private readonly SelectionService _selectionService;

        public MenuOperations(
            MenuService menuService,
            FeedbackService feedbackService,
            RecommendationService recommendationService,
            SelectionService selectionService)
        {
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _recommendationService = recommendationService ?? throw new ArgumentNullException(nameof(recommendationService));
            _selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
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
                return $"Error fetching menu items with feedback: {ex.Message}";
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
                return $"Error fetching menu: {ex.Message}";
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

        public string AddMenuItemToMenu(MySqlConnection connection, AddMenuItemDTO dto)
        {
            try
            {
                return _menuService.AddMenuItem(connection, dto);
            }
            catch (Exception ex)
            {
                return $"Error adding menu item: {ex.Message}";
            }
        }

        public string UpdateMenuItemInMenu(MySqlConnection connection, UpdateMenuItemDTO dto)
        {
            try
            {
                return _menuService.UpdateMenuItem(connection, dto);
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

        private string GenerateMenuResponse(Dictionary<int, ItemDTO> menuItems, Dictionary<string, List<FeedbackDTO>> feedbackDict)
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

    public class UpdateMenuItemDTO
    {
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
    }
    public class AddMenuItemDTO
    {
        public string MenuType { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public string FoodType { get; set; }
    }
}