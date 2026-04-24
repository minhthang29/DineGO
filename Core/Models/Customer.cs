using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Customer
    {
        [Key]
        public int cus_id { get; set; }

        [Required, MaxLength(50)]
        public string cus_username { get; set; }

        [Required, MaxLength(255)]
        public string cus_password { get; set; }

        [Required, MaxLength(100)]
        public string cus_name { get; set; }

        [Required, MaxLength(100)]
        public string cus_email { get; set; }

        [Required, MaxLength(20)]
        public string cus_phone { get; set; }

        [MaxLength(500)]
        public string? cus_address { get; set; }

        public DateTime? cus_birthday { get; set; }
        public bool? cus_gender { get; set; }

        [MaxLength(300)]
        public string? cus_image { get; set; }

        public bool? cus_is_kyc { get; set; }

        [MaxLength(100)]
        public string? google_id { get; set; }

        [MaxLength(20)]
        public string? login_provider { get; set; }

        public int? cus_status { get; set; }
        public DateTime? cus_created_date { get; set; }
        public DateTime? cus_last_login_date { get; set; }
        public double? cus_latitude { get; set; }
        public double? cus_longitude { get; set; }

        [Required]
        public bool cus_is_use { get; set; }
        public bool cus_is_deleted { get; set; }
        public ICollection<Reservation>? reservations { get; set; }
        public ICollection<Payment>? payments { get; set; }
        public ICollection<RestaurantOwner>? restaurantOwners { get; set; }
        public ICollection<Cart>? carts { get; set; }
        public ICollection<Post>? posts { get; set; }
        public ICollection<Comment>? comments { get; set; }
        public ICollection<Like>? likes { get; set; }
        public ICollection<Follower>? followers { get; set; }
        public ICollection<CustomerVoucher>? customerVouchers { get; set; }
        public ICollection<NotificationCustomer>? notificationCustomers { get; set; }
        [JsonIgnore]
        public ICollection<Order>? orders { get; set; }
        public ICollection<Friend>? Friends { get; set; } // Người chủ động kết bạn
        public ICollection<Friend>? FriendOf { get; set; } // Người được kết bạn

        public ICollection<Priority>? priorities { get; set; }
        public ICollection<RestaurantRating> restaurantRatings { get; set; }
        public CustomerPoint? customerPoint { get; set; }

    }
}