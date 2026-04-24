using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class TableLayoutRequest
    {
        public string Area { get; set; }
        public List<TableDto> Tables { get; set; }
    }

    public class TableDto
    {
        public string id { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string left { get; set; }
        public string top { get; set; }
    }
}
