using MBankScrapper.Models;
using System;

namespace MBankScrapper
{
    public class ActionSet
    {
        public Action<AccountBalance> AccountBalance { get; set; }

        public Action<Transaction> Transaction { get; set; }
    }
}
