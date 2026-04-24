using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DineGO_Api.Services
{
    public class DashboardBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DashboardBackgroundService> _logger;

        public DashboardBackgroundService(IServiceScopeFactory scopeFactory, ILogger<DashboardBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Kiểm tra DB đã sẵn sàng chưa
                    if (!await db.Database.CanConnectAsync(stoppingToken))
                    {
                        _logger.LogWarning("Database not ready, retrying in 1 minute...");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                        continue;
                    }

                    var aggregator = scope.ServiceProvider.GetRequiredService<DashboardStatsAggregationService>();
                    await aggregator.AggregateAsync();
                    _logger.LogInformation("Dashboard stats aggregated at {Time}", DateTime.Now);

                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error aggregating dashboard stats");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}