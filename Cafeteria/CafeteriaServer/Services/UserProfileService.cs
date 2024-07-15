using CafeteriaServer.Models;
using CafeteriaServer.Repositories;
namespace CafeteriaServer.Services
{
    public class UserProfileService
    {
        private readonly UserRepository _userRepository;

        public UserProfileService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserProfile FetchUserProfile(int userId)
        {
            return _userRepository.GetUserProfile(userId);
        }
    }
}
