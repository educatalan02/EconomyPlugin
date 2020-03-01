using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyPlugin
{
    public class DatabaseManager
    {


        internal DatabaseManager()
        {
            CheckSchema();
        }

        internal void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + EconomyPlugin.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + EconomyPlugin.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId` varchar(32) NOT NULL,`balance` decimal(15,0) NOT NULL DEFAULT '{"+EconomyPlugin.Instance.Configuration.Instance.DefaultMoney+"}',`lastUpdated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`steamId`)) ";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void CheckSetupAccount(UnturnedPlayer player)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                int exists = 0;
                command.CommandText = "SELECT EXISTS(SELECT 1 FROM `" + EconomyPlugin.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` ='" + player.CSteamID + "' LIMIT 1);";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) Int32.TryParse(result.ToString(), out exists);
                connection.Close();

                if (exists == 0)
                {
                    player.Experience = (uint)EconomyPlugin.Instance.Configuration.Instance.DefaultMoney;
                    command.CommandText = "insert ignore into `" + EconomyPlugin.Instance.Configuration.Instance.DatabaseTableName + "` (balance,steamId,lastUpdated) values(" + EconomyPlugin.Instance.Configuration.Instance.DefaultMoney + ",'" + player.CSteamID.ToString() + "',now())";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

        }

        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (EconomyPlugin.Instance.Configuration.Instance.DatabasePort == 0) EconomyPlugin.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", EconomyPlugin.Instance.Configuration.Instance.DatabaseAddress, EconomyPlugin.Instance.Configuration.Instance.DatabaseName, EconomyPlugin.Instance.Configuration.Instance.DatabaseUsername, EconomyPlugin.Instance.Configuration.Instance.DatabasePassword, EconomyPlugin.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }
        public decimal GetBalance(string id)
        {
            decimal output = 0;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `balance` from `" + EconomyPlugin.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = '" + id.ToString() + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) Decimal.TryParse(result.ToString(), out output);
                connection.Close();

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }
    }
}
