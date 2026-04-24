using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Core.Services;
using Core.Constant;
using Core.Models;
using Microsoft.AspNetCore.SignalR;
using DineGO_Client.SignalR; 


namespace DineGO_Client.Background{
    public class ReservationCleanupService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public ReservationCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _scopeFactory.CreateScope();
            var apiService = scope.ServiceProvider.GetRequiredService<ApiService>();
            var tableService = scope.ServiceProvider.GetRequiredService<TableService>();
            // var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<TableHub>>();

            var reservations = await apiService.GetAsync<List<Reservation>>(ApiEndpoints.RESERVATION);
            var now = DateTime.Now;

            var expired = reservations
                .Where(r => r.reser_status == 0 && (now - r.reser_create_at).TotalMinutes >= 10)
                .ToList();

            foreach (var r in expired)
            {
                // await tableService.UpdateTableStatus(r.table_id, 0);
                await tableService.UpdateReservationStatus(r.reser_id, 2);

                // Gửi signal cập nhật trạng thái về client
                // await hubContext.Clients.All.SendAsync("ReceiveTableStatus", r.table_id, 0);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}