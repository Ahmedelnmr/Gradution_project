using Homy.Domin.Contract_Repo;
using Homy.Domin.models;
using Homy.Infurastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Infurastructure.Repository
{
    public class Notification_Repo : Genric_Repo<Notification>, INotification_Repo
    {
        public Notification_Repo(HomyContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await dbset
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await dbset
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(long notificationId)
        {
            var notification = await dbset.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }
    }
}
