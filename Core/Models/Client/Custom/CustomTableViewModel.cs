using System.Collections.Generic;

namespace Core.Models.Client.Custom
{
    public class CustomTableViewModel
    {
        public List<Table> Tables { get; set; } = new List<Table>();
        public List<TableArea> Areas { get; set; } = new List<TableArea>();
        public int SelectedAreaId { get; set; } // area_id đang chọn
    }
}
