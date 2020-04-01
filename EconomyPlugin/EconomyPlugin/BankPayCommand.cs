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
    public class BankPayCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => throw new NotImplementedException();

        public string Name => "bankpay";

        public string Help => Syntax;

        public string Syntax => "Syntax: /pay <player> <quantity>";

        public List<string> Aliases => new List<string> { "apay"  };

        public List<string> Permissions => new List<string>() { "EconomyPlugin.bankpay" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            ulong? otherPlayerID = command.GetCSteamIDParameter(0);
            if (!(command.Length == 2))
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            if (otherPlayer == null || otherPlayerID == 0)
            {
                UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("player_not_found"));
                return;
            }
            else
            {
                decimal amount = 0;
                if (!Decimal.TryParse(command[1], out amount) || amount <= 0)
                {
                    UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("invalid_ammount"));
                    return;
                }
                if (caller is ConsolePlayer)
                {
                    EconomyPlugin.Instance.Database.IncreaseBalance(otherPlayer, amount);
                }
                else
                {
                    EconomyPlugin.Instance.Database.IncreaseBalance(otherPlayer, amount);
                    UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("pay", amount, EconomyPlugin.Instance.Configuration.Instance.MoneySymbol, otherPlayer.CharacterName));
                }
            }
        }
    }
}
