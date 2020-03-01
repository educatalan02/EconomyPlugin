using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyPlugin
{
    public class BankAccount
    {
        public decimal Balance;
        public ulong PlayerId;
        public DateTime LastUpdate;

        public BankAccount()
        {

        }

        public BankAccount(ulong playerid, decimal balance)
        {
            this.PlayerId = playerid;
            this.Balance = balance;
            this.LastUpdate = DateTime.Now;
        }
    }
}
