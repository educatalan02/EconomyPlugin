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
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using UnityEngine;
using Rocket.API;
using Rocket.API.Serialisation;

namespace EconomyPlugin
{
    public class EconomyPlugin : RocketPlugin<Configuration>
    {
        public DatabaseManager Database;
        public static EconomyPlugin Instance;

        //SalaryDictionary
        public Dictionary<string, DateTime> Salaries = new Dictionary<string, DateTime>();

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
            UnturnedPlayerEvents.OnPlayerDeath += onPlayerDeath;
        }

        private void onPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (cause == EDeathCause.SUICIDE)
                if (Configuration.Instance.SuicideFine > 0)
                    {
                    Database.IncreaseBalance(player, -Configuration.Instance.SuicideFine);
                    UnturnedChat.Say(player, Translate("suicide_fine", Configuration.Instance.SuicideFine, Configuration.Instance.MoneySymbol));
                }
            else
                  if (Configuration.Instance.DeathFine > 0)
                    {
                    Database.IncreaseBalance(player, -Configuration.Instance.DeathFine);
                    UnturnedChat.Say(player, Translate("death_fine", Configuration.Instance.DeathFine, Configuration.Instance.MoneySymbol));
                    }
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
            UnturnedPlayerEvents.OnPlayerDeath -= onPlayerDeath;
        }

        private void Update()
        {
            foreach (SteamPlayer steamplayer in Provider.clients) 
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamplayer);

                if (Salaries.TryGetValue(player.Id, out DateTime time) && time < DateTime.Now)
                {
                    Permission per = player.GetPermissions().FirstOrDefault(x => x.Name.StartsWith("economy.salary."));

                    if (per != null)
                        if (int.TryParse(per.Name.Split('.').Last(), out int money))
                        {
                            if (money > 0)
                            {
                                Database.IncreaseBalance(player, money);
                            }
                            Salaries.Remove(player.Id); //Removes old salary time
                            Salaries.Add(player.Id, DateTime.Now.AddSeconds(Configuration.Instance.SalaryIntervalInSeconds)); //Adds next salary time
                            UnturnedChat.Say(player, Translate("received_salary", Database.GetBalance(player.Id).ToString(), Configuration.Instance.MoneySymbol));
                        }

                }
            }
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                { "received_salary", "You have got your salary! Your new balance is {1}{0}!"},
                { "pay", "You have paid {1}{0} to {2}" },
                { "not_enough_money", "You don't have enough money!" },
                { "not_enought_experience", "You don't have enough experience!" },
                { "invalid_ammount", "Invalid ammount!" },
                { "player_not_found", "Player not found" },
                { "suicide_fine", "You committed suicide! Suicide fine is: {1}{0}" },
                { "death_fine", "You died! Death fine is: {1}{0}" },
                { "self_balance", "My balance is {1}{0}" },
                { "other_balance", "{2}'s balance is {1}{0}" },
                { "money_to_xp", "You have successfully changed {1}{0} to {2} XP" },
                { "xp_to_money", "You have successfully changed {0} XP to {1}{2}" }
            };
    }
}
