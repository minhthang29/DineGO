using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class FriendWithMessage
    {
        public int cus_id { get; set; }
        public string cus_name { get; set; }
        public string? cus_image { get; set; }
        public string? last_message { get; set; }
    }
}