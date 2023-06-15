using System.Data.SqlClient;
using Typo.View;

namespace Typo.Utils
{
    public class DatabaseUtils
    {
        private static readonly string _ConnectionString = BuildConnectionString();

        /// <summary>
        /// Builds the string for establishing a connection with the database.
        /// </summary>
        /// <returns>The connection string.</returns>
        private static string BuildConnectionString()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {
                    DataSource = "127.0.0.1",
                    UserID = "sa",
                    Password = "16Centen!",
                    InitialCatalog = "Typo"
                };
                return builder.ConnectionString;
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                App.ShowMessageBox("Er is een fout opgetreden bij het maken van de verbinding met de database. Dit is een fatale fout. Probeer het later opnieuw.", "Fout: Connection string", System.Windows.MessageBoxButton.OK);
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates a database connection, attempts to open it and returns it.
        /// </summary>
        /// <returns>An opened database connection, or null if the connection could not be opened.</returns>
        public static SqlConnection? OpenDatabaseConnection()
        {
            if (_ConnectionString.Length == 0)
            {
                return null;
            }

            if (!SshUtils.client.IsConnected)
            {
                return null;
            }

            SqlConnection connection = new(_ConnectionString);
            try
            {
                connection.Open();
            }
            catch (SqlException)
            {
                UserUtils.Logout();
                App.CurrentWindow = new LoginView();
                return null;
            }
            return connection;
        }
    }
}