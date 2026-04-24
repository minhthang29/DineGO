using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.Client.Custom
{
    /// <summary>
    /// Represents the view model for booking a reservation.
    /// </summary>
    /// <author>Thangtm</author>
    public class CustomReservationViewModel
    {
        public Restaurant Restaurant { get; set; }
        public Customer Customer { get; set; }
        public List<TableArea> Areas { get; set; }
        public List<Table> Tables { get; set; }
    }
}