using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Core.Services;
using System.Collections.Generic;
using System.Linq;
using System;
using Core.Constant;
using System.Text.Json;
using System.Collections.Concurrent;
using Core.Models;


namespace DineGO_Client.SignalR
{
    public class ReservationHub : Hub
{
    // Gửi signal khi có đặt bàn mới
    public async Task NotifyReservationUpdated(int tableId, DateTime date)
    {
        await Clients.All.SendAsync("ReservationUpdated", tableId, date);
    }
}
}