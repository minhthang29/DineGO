using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.Client.Custom
{
    public class CustomHomeViewModel
    {
        public List<Restaurant> Restaurants { get; set; }
        public List<Blog> Blogs { get; set; }
    }
}