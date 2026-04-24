using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class RestaurantRatingViewModel
    {
        public int cus_id { get; set; }
        public int res_id { get; set; }
        public int rating_value { get; set; }
        public string? rating_comment { get; set; }
    }
}