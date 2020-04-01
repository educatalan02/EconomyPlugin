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
    public class ChangeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => throw new NotImplementedException();

        public string Name => "change";

        public string Help => Syntax;

        public string Syntax => "Syntax: /change xp | money <amount>";

        public List<string> Aliases => new List<string> { "exchange", "exc", "moneychange", "mch" };

        public List<string> Permissions => new List<string>() { "EconomyPlugin.change" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 2)
            {
                if (command[0].ToLower() == "xp")
                {
                    decimal amount = 0;
                    if (!Decimal.TryParse(command[1], out amount) || amount <= 0)
                    {
                        UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("invalid_ammount"));
                        return;
                    }

                    decimal myBalance = player.Experience;
                    if ((myBalance - amount) <= 0)
                    {
                        UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("not_enought_experience"));
                        return;
                    }
                    else
                    {
                        EconomyPlugin.Instance.Database.IncreaseBalance(player, amount);
                        player.Experience -= (uint)amount;
                        UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("xp_to_money", amount, EconomyPlugin.Instance.Configuration.Instance.MoneySymbol, EconomyPlugin.Instance.Database.GetBalance(player.Id)));
                    }
                }
                else if (command[0].ToLower() == "money")
                {
                    decimal amount = 0;
                    if (!Decimal.TryParse(command[1], out amount) || amount <= 0)
                    {
                        UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("invalid_ammount"));
                        return;
                    }

                    decimal myBalance = EconomyPlugin.Instance.Database.GetBalance(caller.Id);
                    if ((myBalance - amount) <= 0)
                    {
                        UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("not_enought_money"));
                        return;
                    }
                    else
                    {
                        EconomyPlugin.Instance.Database.IncreaseBalance(player, -amount);
                        player.Experience += (uint)amount;
                        UnturnedChat.Say(caller, EconomyPlugin.Instance.Translate("money_to_xp", amount, EconomyPlugin.Instance.Configuration.Instance.MoneySymbol, player.Experience));
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, Syntax);
                    return;
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
