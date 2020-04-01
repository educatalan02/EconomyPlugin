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
        public string MoneySymbol;
        public string DatabaseTableName;
        public bool EnableUI;
        public ushort EffectId;
        public int DatabasePort;
        public string UITextColor;
        public bool UsingXP;
        public decimal DeathFine;
        public decimal SuicideFine;
        public bool SalaryEnabled; //Permission for it: economy.salary.<number>
        public int SalaryIntervalInSeconds;


        public void LoadDefaults()
        {
            UsingXP = true;
            UITextColor = "red";
            EnableUI = true;
            DatabaseAddress = "localhost";
            DatabaseUsername = "root";
            DatabasePassword = "root";
            DatabaseName = "unturned";
            DefaultMoney = 15;
            MoneySymbol = "$";
            EffectId = 15;
            DatabaseTableName = "Economy";
            DatabasePort = 3306;
            DeathFine = 100;
            SuicideFine = 300;
            SalaryEnabled = true;
            SalaryIntervalInSeconds = 1800;
        }
    }
}