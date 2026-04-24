using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Follower
    {
        [Key]
        public int follower_id { get; set; }

        public int res_owner_id { get; set; }
        public int cus_id { get; set; }

        public DateTime follower_created { get; set; }

        public Customer? customer { get; set; }
        public RestaurantOwner? restaurantOwner { get; set; }
    }
}