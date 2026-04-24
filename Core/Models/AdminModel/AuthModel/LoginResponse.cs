using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.AdminModel.AuthModel
{
    public class LoginResponse
    {
        public string ad_token { get; set; }
        public int ad_id { get; set; }
        public string ad_name { get; set; }
        public string? ad_image { get; set; } 
    }
}