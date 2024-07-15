using System;
using MySql.Data.MySqlClient;
using CafeteriaServer.Services;
using CafeteriaServer.Repositories;
namespace CafeteriaServer.Operations
{
    public class DiscardMenu
    {
        private readonly DiscardMenuService _discardMenuService;

        public DiscardMenu(MySqlConnection connection)
        {
            _discardMenuService = new DiscardMenuService(connection);
        }
        public string FetchTodayNotificationsForEmployees(MySqlConnection dbConnection, int userTypeId)
        {
            return _discardMenuService.FetchTodayNotificationsForEmployees(dbConnection, userTypeId);
        }

        public string FetchMenuItemsWithNegativeFeedback(MySqlConnection dbConnection)
        {
            return _discardMenuService.FetchMenuItemsWithNegativeFeedback(dbConnection);
        }

        public string RemoveMenuItem(MySqlConnection dbConnection, string itemName)
        {
            return _discardMenuService.RemoveMenuItem(dbConnection, itemName);
        }

        public string RollOutFoodForDetailedFeedback(MySqlConnection dbConnection, string itemName)
        {
            return _discardMenuService.RollOutFoodForDetailedFeedback(dbConnection, itemName);
        }

        public string SubmitDetailedFeedback(MySqlConnection dbConnection, string itemName, string question1Response, string question2Response, string question3Response)
        {
            return _discardMenuService.SubmitDetailedFeedback(dbConnection, itemName, question1Response, question2Response, question3Response);
        }

        public string GetFeedbackQuestionsForItem(string itemName)
        {
            // Assuming GetFeedbackQuestionsForItem doesn't need a DB connection
            return _discardMenuService.GetFeedbackQuestionsForItem(itemName);
        }

        public string FetchCurrentRolloutFeedbackItems(MySqlConnection dbConnection)
        {
            return _discardMenuService.FetchCurrentRolloutFeedbackItems(dbConnection);
        }
    }
}
