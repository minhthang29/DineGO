using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class CustomerPointRequest
    {
        public int CusId { get; set; }
        public int ChangeAmount { get; set; }
        public string? Description { get; set; }
    }
}