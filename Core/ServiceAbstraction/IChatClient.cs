using Shared.Message;
using Shared.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IChatClient
    {
        // الميثودز اللي الموبايل هيفضل سامعها
        Task ReceiveMessage(MessageDto message);
        Task ReceiveNotification(NotificationDto notification);
    }
}
