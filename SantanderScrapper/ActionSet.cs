using SantanderScrapper.Models;
using System;

namespace SantanderScrapper
{
    public class ActionSet
    {
        public Action<AccountBalance> AccountBalance { get; set; }

        public Action<Transaction> Transaction { get; set; }
    }
}
