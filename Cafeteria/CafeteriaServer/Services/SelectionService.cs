using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;

namespace CafeteriaServer.Services
{
    public class SelectionService
    {
        public string SelectFoodItemsForNextDay(MySqlConnection connection, int userId, int[] rolloutIds)
        {
            var responseMessage = new StringBuilder();

            foreach (int rolloutId in rolloutIds)
            {
                if (IsRolloutItemAvailable(connection, rolloutId))
                {
                    int rowsAffected = RecordUserSelection(connection, userId, rolloutId);

                    responseMessage.AppendLine(rowsAffected > 0
                        ? $"Rollout item {rolloutId} selected successfully."
                        : $"Failed to select rollout item {rolloutId}.");
                }
                else
                {
                    responseMessage.AppendLine($"Rollout item {rolloutId} not found or not available.");
                }
            }

            return responseMessage.ToString();
        }

        private bool IsRolloutItemAvailable(MySqlConnection connection, int rolloutId)
        {
            const string selectQuery = "SELECT * FROM RolloutItems WHERE rollout_id = @rolloutId AND available = 1";
            using (var selectCmd = new MySqlCommand(selectQuery, connection))
            {
                selectCmd.Parameters.AddWithValue("@rolloutId", rolloutId);

                using (var reader = selectCmd.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        private int RecordUserSelection(MySqlConnection connection, int userId, int rolloutId)
        {
            const string insertQuery = "INSERT INTO EmployeeSelections (user_id, rollout_id, selected) VALUES (@userId, @rolloutId, true)";
            using (var insertCmd = new MySqlCommand(insertQuery, connection))
            {
                insertCmd.Parameters.AddWithValue("@userId", userId);
                insertCmd.Parameters.AddWithValue("@rolloutId", rolloutId);

                return insertCmd.ExecuteNonQuery();
            }
        }
    }
}
