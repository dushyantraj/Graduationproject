using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Models.DTO;

namespace CafeteriaServer.Repositories
{
    public class EmployeeSelectionRepository
    {
        private readonly MySqlConnection _connection;

        public EmployeeSelectionRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public List<EmployeeSelectionDTO> GetEmployeeSelectionsForToday(DateTime date)
        {
            List<EmployeeSelectionDTO> selections = new List<EmployeeSelectionDTO>();

            string query = @"
        SELECT es.user_id, es.rollout_id, ri.item_name 
        FROM EmployeeSelections es 
        JOIN RolloutItems ri ON es.rollout_id = ri.rollout_id 
        WHERE DATE(ri.date_rolled_out) = DATE(@date)";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@date", date);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        HashSet<int> displayedRolloutIds = new HashSet<int>();

                        while (reader.Read())
                        {
                            int rolloutId = reader.GetInt32("rollout_id");

                            // Check if this rolloutId has already been displayed
                            if (displayedRolloutIds.Contains(rolloutId))
                            {
                                continue;
                            }

                            var selection = new EmployeeSelectionDTO
                            {
                                UserId = reader.GetInt32("user_id"),
                                RolloutId = rolloutId,
                                ItemName = reader.GetString("item_name")
                            };

                            selections.Add(selection);
                            displayedRolloutIds.Add(rolloutId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Error fetching employee selections from the database", ex);
            }

            return selections;
        }


        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
