using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.AdminModel.Custom
{
    public class CustomerUpdateProfileViewModel
    {
        public int cus_id { get; set; }
        public string cus_name { get; set; }
        public string cus_password { get; set; }
        public string cus_phone { get; set; }
        public string cus_email { get; set; }
        public string? cus_address { get; set; }
        public DateTime? cus_birthday { get; set; }
        public bool? cus_gender { get; set; }
        public string? cus_image { get; set; }
    }
}