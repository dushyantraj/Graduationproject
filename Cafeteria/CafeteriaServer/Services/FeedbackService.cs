using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Models.DTO;
using CafeteriaServer.Repositories;

namespace CafeteriaServer.Services
{
    public class FeedbackService
    {
        private readonly FeedbackRepository _feedbackRepository;


        public FeedbackService(MySqlConnection connection)
        {
            _feedbackRepository = new FeedbackRepository(connection);
        }
        public Dictionary<string, List<FeedbackDTO>> FetchDetailedFeedback(MySqlConnection connection, string todayString)
        {
            return _feedbackRepository.FetchDetailedFeedback(todayString);
        }

        public Dictionary<string, List<FeedbackDTO>> FetchFeedback(MySqlConnection connection)
        {
            return _feedbackRepository.FetchAllFeedback(connection);
        }

        public Dictionary<string, List<FeedbackDTO>> FetchAllFeedback(MySqlConnection connection)
        {
            return _feedbackRepository.FetchAllFeedback(connection);
        }

        public string FillFeedbackForm(MySqlConnection connection, string itemName, int rating, string comments)
        {
            try
            {
                FetchFeedbackItems(connection);

                int feedbackId = GetFeedbackId(itemName);

                if (feedbackId == -1)
                {
                    return "Item not found. Please provide a valid food item name.";
                }

                InsertFeedbackDetails(feedbackId, rating, comments);

                UpdateOverallSentiment(itemName);

                return "Feedback submitted successfully.";
            }
            catch (Exception ex)
            {
                LogException("Error filling feedback form", ex);
                return $"Error filling feedback form: {ex.Message}";
            }
        }

        public string SubmitFeedback(MySqlConnection connection, string itemName)
        {
            try
            {
                itemName = itemName.Trim();

                if (string.IsNullOrEmpty(itemName))
                {
                    return "Item name cannot be empty.";
                }

                int rowsAffected = InsertFeedback(itemName);

                return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
            }
            catch (Exception ex)
            {
                LogException("Error submitting feedback", ex);
                return $"Error submitting feedback: {ex.Message}";
            }
        }

        public string FetchFeedbackItems(MySqlConnection connection)
        {
            return _feedbackRepository.FetchFeedbackItems();
        }

        public string FetchFeedbackDetails(MySqlConnection connection, string itemName)
        {
            return _feedbackRepository.FetchFeedbackDetails(itemName);
        }

        private int GetFeedbackId(string itemName)
        {
            return _feedbackRepository.GetFeedbackId(itemName);
        }

        private void InsertFeedbackDetails(int feedbackId, int rating, string comments)
        {
            _feedbackRepository.InsertFeedbackDetails(feedbackId, rating, comments);
        }

        private int InsertFeedback(string itemName)
        {
            return _feedbackRepository.InsertFeedback(itemName);
        }

        private void UpdateOverallSentiment(string itemName)
        {
            _feedbackRepository.UpdateOverallSentiment(itemName);
        }

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
