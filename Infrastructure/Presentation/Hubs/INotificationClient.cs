using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Hubs
{
    public interface INotificationClient
    {
        /// <summary>
        /// Pushed to the user when a general notification (Order, System, etc.) arrives.
        /// Client handler: connection.on("ReceiveNotification", (notification) => { ... })
        /// </summary>
        Task ReceiveNotification(NotificationResponseDto notification);
    }
}
