using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.AdminModel
{
    public class UpdateAvatarRequest
    {
        public string ad_image { get; set; }
        public int ad_id { get; set; }
    }
}