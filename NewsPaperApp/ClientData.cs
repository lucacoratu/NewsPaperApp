using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    static class ClientData
    {
        private static string ConnectedAccountUsername;

        public static void SetConnectedAccountUsername(string username)
        {
            ConnectedAccountUsername = username;
        }

        public static string GetConnectedAccountUsername()
        {
            return ConnectedAccountUsername;
        }
    }
}
