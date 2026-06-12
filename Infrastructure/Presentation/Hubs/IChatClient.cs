using Shared.Chat;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Hubs
{

    
        /// <summary>
        /// Strongly-typed contract for methods the SERVER can invoke on CLIENTS.
        ///
        /// Using a typed interface instead of magic strings (Clients.All.SendAsync("MethodName"))
        /// gives compile-time safety — a rename or signature change is caught immediately.
        ///
        /// Each method name here MUST match the event name the mobile/web client listens for.
        /// </summary>
        public interface IChatClient
        {
            /// <summary>
            /// Pushed to the RECEIVER when a new message arrives.
            /// Client handler: connection.on("ReceiveMessage", (msg) => { ... })
            /// </summary>
            Task ReceiveMessage(MessageResponseDto message);

            /// <summary>
            /// Pushed to the original SENDER when the receiver reads their message.
            /// Client handler: connection.on("MessageRead", (n) => { ... })
            /// </summary>
            Task MessageRead(MessageReadNotificationDto notification);

            /// <summary>
            /// Pushed when a conversation partner comes online.
            /// Client handler: connection.on("UserOnline", (userId) => { ... })
            /// </summary>
            Task UserOnline(string userId);

            /// <summary>
            /// Pushed when a conversation partner goes offline.
            /// Client handler: connection.on("UserOffline", (userId) => { ... })
            /// </summary>
            Task UserOffline(string userId);

        /// <summary>
        /// Pushed to the user when a general notification (Order, System, etc.) arrives.
        /// Client handler: connection.on("ReceiveNotification", (notification) => { ... })
        /// </summary>
        Task ReceiveNotification(NotificationResponseDto notification);



    }
}

