using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homy.Domin.Contract_Repo
{
    public interface INotification_Repo : IGenric_Repo<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(long notificationId);
    }
}
