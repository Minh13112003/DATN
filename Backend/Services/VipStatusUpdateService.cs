using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Helper.DateTimeVietNam;
namespace DoAnTotNghiep.Services
{
    public class VipStatusUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public VipStatusUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                    var now = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();
                    var users = await dbContext.Users
                        .Where(u => u.ExpirationTime.HasValue && u.ExpirationTime < now && u.IsVip)
                        .ToListAsync();

                    foreach (var user in users)
                    {
                        user.IsVip = false;                                          
                    }

                    if (users.Count > 0)
                    {
                        await dbContext.SaveChangesAsync();
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
