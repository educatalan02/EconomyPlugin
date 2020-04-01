using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyPlugin
{
    public class BalanceCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => throw new NotImplementedException();

        public string Name => "balance";

        public string Help => Syntax;

        public string Syntax => "Syntax: /balance <player>";

        public List<string> Aliases => new List<string> { "money", "bank" };

        public List<string> Permissions => new List<string>() { "EconomyPlugin.balance" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                decimal Balance = EconomyPlugin.Instance.Database.GetBalance(caller.Id);
                UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("self_balance", Balance, EconomyPlugin.Instance.Configuration.Instance.MoneySymbol));
            }
            else if (command.Length == 1 && player.HasPermission("EconomyPlugin.balance.other"))
            {

                UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
                ulong? otherPlayerID = command.GetCSteamIDParameter(0);

                if (otherPlayer == null || otherPlayerID == 0)
                {
                    UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("player_not_found"));
                    return;
                }
                else
                {
                    decimal Balance = EconomyPlugin.Instance.Database.GetBalance(otherPlayer.Id);
                    UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("other_balance", Balance, EconomyPlugin.Instance.Configuration.Instance.MoneySymbol, otherPlayer.CharacterName));
                }
            }
            else
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
        }
    }
}
