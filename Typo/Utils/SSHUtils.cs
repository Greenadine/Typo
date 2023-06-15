using Renci.SshNet;
using System;

namespace Typo.Utils
{
    public static class SshUtils
    {
        private static string _ipAdress = "145.44.233.128";
        private static string _user = "student";
        private static string _password = "16cent!";
        private static AuthenticationMethod _method = new PasswordAuthenticationMethod(_user, _password);
        private static ConnectionInfo _info = new(_ipAdress, _user, _method);
        public static SshClient client = new(_info);
        private static ForwardedPortLocal _port = new("127.0.0.1", 1433, "127.0.0.1", 1433);

        /// <summary>
        /// Attempts to establish a connection with the database server through SSH. Also handles portforwarding.
        /// </summary>
        /// <returns>'true' if establishing the connection was successful, 'false' if establishing the connection failed.</returns>
        public static bool Connect()
        {
            try
            {
                if (client.IsConnected)
                {
                    return true;
                }
                client.Connect();
                client.AddForwardedPort(_port);
                _port.Start();
                return client.IsConnected;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}