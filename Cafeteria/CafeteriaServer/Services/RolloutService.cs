using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using System.Collections.Generic;
using CafeteriaServer.Repositories;
namespace CafeteriaServer.Services
{
    public class RolloutService
    {
        private readonly RolloutRepository _rolloutRepository;

        public RolloutService(MySqlConnection connection)
        {
            _rolloutRepository = new RolloutRepository(connection);
        }

        public Dictionary<int, RolloutItem> FetchPreferredRolloutItems(string todayString, string foodTypePreference, string cuisinePreference, string spiceLevel)
        {
            return _rolloutRepository.FetchPreferredRolloutItems(todayString, foodTypePreference, cuisinePreference, spiceLevel);
        }

        public Dictionary<int, RolloutItem> FetchAllRolloutItems(string todayString, string foodTypePreference, string cuisinePreference, string spiceLevel)
        {
            return _rolloutRepository.FetchAllRolloutItems(todayString, foodTypePreference, cuisinePreference, spiceLevel);
        }
    }
}


