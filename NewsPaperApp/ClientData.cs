using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    static class ClientData
    {
        private static string ConnectedAccountUsername;
        private static string CurrentNewspaper;
        
        //Modifiers
        public static void SetConnectedAccountUsername(string username)
        {
            ConnectedAccountUsername = username;
        }

        public static void SetCurrentNewspaper(string currentNewspaper)
        {
            CurrentNewspaper = currentNewspaper;
        }

        //Accessors
        public static string GetConnectedAccountUsername()
        {
            return ConnectedAccountUsername;
        }

        public static string GetCurrentNewspaper()
        {
            return CurrentNewspaper;
        }
    }
}
