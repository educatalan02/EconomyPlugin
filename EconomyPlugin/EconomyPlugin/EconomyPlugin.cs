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
using Steamworks;

namespace EconomyPlugin
{
    public class EconomyPlugin : RocketPlugin<Configuration>
    {
        public DatabaseManager Database;
        public static EconomyPlugin Instance;

        //UpdateBalance Event
        public delegate void PlayerUpdateBalance(UnturnedPlayer player, decimal balance);
        public event PlayerUpdateBalance OnBalanceUpdate;


        internal void BalanceUpdated(UnturnedPlayer SteamID, decimal amt)
        {
            if (OnBalanceUpdate != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceUpdate(player, amt);
            }
        }

        protected override void Load()
        {
            Logger.Log("Plugin loaded correctly!");
            Database = new DatabaseManager();
            U.Events.OnPlayerConnected += OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerUpdateExperience += OnUpdateExperience;
            Player.player.quests.questCompleted += onQuestCompleted;
        }

        

        private void onQuestCompleted(PlayerQuests sender, QuestAsset asset)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(sender.player);
            if(base.Configuration.Instance.UsingXP)
            {
                UpdateUI(player);
            }
        }


        //UpdateExperience Event
        private void OnUpdateExperience(UnturnedPlayer player, uint experience)
        {
            if(base.Configuration.Instance.UsingXP)
            {
                OnBalanceUpdate(player, decimal.Parse(player.Experience.ToString()));
                UpdateUI(player);
            }
        }


        //UpdateUI Method
        private void UpdateUI(UnturnedPlayer player)
        {
            if(base.Configuration.Instance.EnableUI)
            {
                EffectManager.sendUIEffect(Configuration.Instance.EffectId, short.Parse(Configuration.Instance.EffectId.ToString()), player.CSteamID, true, "<color=" + base.Configuration.Instance.UITextColor + ">" + Database.GetBalance(player.CSteamID.ToString()) + "</color>".ToString());
            }
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            Database.CheckSetupAccount(player); //Setup account!
            if (base.Configuration.Instance.EnableUI)
            {
                UpdateUI(player);
            }
        }

        protected override void Unload()
        {
            Player.player.quests.questCompleted -= onQuestCompleted;
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerUpdateExperience -= OnUpdateExperience;
        }

        
        


    }
}
