namespace CafeteriaClient.Operations
{
    public static class UserSession
    {
        private static string loggedInUsername;

        public static string LoggedInUsername
        {
            get { return loggedInUsername; }
            private set { loggedInUsername = value; }
        }

        public static void SetSession(string username)
        {
            LoggedInUsername = username;
        }

        public static void ClearSession()
        {
            LoggedInUsername = null;
        }

        public static bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(LoggedInUsername);
        }
    }
}
