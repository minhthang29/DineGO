using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.AuthModel
{
    /// <summary>
    /// Login API response model.
    /// </summary>
    /// <author>Phuonghh</author>
    public class LoginResponse
    {
        public string token { get; set; }
        public int cus_id { get; set; }
        public string cus_name { get; set; }
    }
}