using Rocket.API;

namespace EconomyPlugin
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public int DefaultMoney;
        public string DatabaseTableName;
        public ushort EffectId;
        public int DatabasePort;


        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "root";
            DatabasePassword = "root";
            DatabaseName = "unturned";
            DefaultMoney = 15;
            EffectId = 15;
            DatabaseTableName = "Economy";
            DatabasePort = 3306;
        }
    }
}