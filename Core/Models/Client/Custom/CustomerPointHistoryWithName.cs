using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class CustomerPointHistoryWithName
    {
        public int HistoryId { get; set; }
        public int ChangeAmount { get; set; }
        public int BalanceAfter { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CustomerName { get; set; }
    }
}