using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class MenuWithFoodsViewModel
    {
        public Menu Menu { get; set; }
        public List<Food> Foods { get; set; }
    }
}