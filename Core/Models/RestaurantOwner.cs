using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class RestaurantOwner
    {
        [Key]
        public int res_owner_id { get; set; }

        public int cus_id { get; set; }

        [Required, MaxLength(500)]
        public string res_owner_name { get; set; }

        public DateTime res_owner_created_date { get; set; }

        public int res_owner_follower_count { get; set; }

        public bool res_owner_is_use { get; set; }

        public bool res_owner_is_deleted { get; set; }

        public Customer? customer { get; set; }

        public ICollection<Restaurant>? restaurants { get; set; }
        public ICollection<Blog>? blogs { get; set; }
        public ICollection<Follower>? followers { get; set; }
        public ICollection<AdRegistration>? adRegistrations { get; set; }

    }
}