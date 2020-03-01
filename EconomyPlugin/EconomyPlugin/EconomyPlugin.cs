using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logger = Rocket.Core.Logging.Logger;
using System.Threading.Tasks;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned.Events;
using SDG.Unturned;

namespace EconomyPlugin
{
    public class EconomyPlugin : RocketPlugin<Configuration>
    {
        public DatabaseManager Database;
        public static EconomyPlugin Instance;

        //UpdateBalance Event
        public delegate void PlayerUpdateBalance(UnturnedPlayer player, decimal balance);
        public event PlayerUpdateBalance OnBalanceUpdate;


        protected override void Load()
        {
            Logger.Log("Plugin loaded correctly!");
            Database = new DatabaseManager();
            U.Events.OnPlayerConnected += OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerUpdateExperience += OnUpdateExperience;
        }


        //UpdateExperience Event
        private void OnUpdateExperience(UnturnedPlayer player, uint experience)
        {

            OnBalanceUpdate(player, decimal.Parse(player.Experience.ToString()));
            UpdateUI(player);
        }


        //UpdateUI Method
        private void UpdateUI(UnturnedPlayer player)
        {
            EffectManager.sendUIEffect(Configuration.Instance.EffectId, short.Parse(Configuration.Instance.EffectId.ToString()), player.CSteamID, true, player.Experience.ToString());
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            Database.CheckSetupAccount(player); //Setup account!
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerUpdateExperience -= OnUpdateExperience;
        }

        
        


    }
}
