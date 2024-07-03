using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Models.DTO;
using CafeteriaServer.Repositories;

namespace CafeteriaServer.Services
{
    public class EmployeeSelectionService
    {
        private readonly EmployeeSelectionRepository _selectionRepository;

        public EmployeeSelectionService(EmployeeSelectionRepository selectionRepository)
        {
            _selectionRepository = selectionRepository;
        }

        public string FetchEmployeeSelectionsForToday()
        {
            try
            {
                DateTime today = DateTime.Today;
                var selections = _selectionRepository.GetEmployeeSelectionsForToday(today);

                return FormatSelectionsResponse(selections);
            }
            catch (Exception ex)
            {
                LogException("Error fetching employee selections", ex);
                return "Error fetching employee selections: " + ex.Message;
            }
        }

        private string FormatSelectionsResponse(List<EmployeeSelectionDTO> selections)
        {
            if (selections.Count == 0)
            {
                return "No employee selections found for today.";
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine("Employee Selections for Today:");

            foreach (var selection in selections)
            {
                response.AppendLine($"Employee ID: {selection.UserId}, Rollout ID: {selection.RolloutId}, Item Name: {selection.ItemName}");
            }

            return response.ToString();
        }

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
