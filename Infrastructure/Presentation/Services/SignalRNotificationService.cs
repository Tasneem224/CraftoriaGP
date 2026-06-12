//using global::Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using ServiceAbstraction;
using Shared.Chat;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Presentation.Services
{
    

    
        /// <summary>
        /// ════════════════════════════════════════════════════════════════════════
        ///  BRIDGE IMPLEMENTATION (see IRealtimeNotificationService for the pattern)
        /// ════════════════════════════════════════════════════════════════════════
        ///
        /// Lives in Presentation — the only project that is ALLOWED to reference
        /// SignalR.  The Service layer holds only the interface reference
        /// (IRealtimeNotificationService) injected via constructor — it never sees
        /// IHubContext or ChatHub.
        ///
        /// Registered in Program.cs:
        ///   services.AddScoped&lt;IRealtimeNotificationService,
        ///                       SignalRNotificationService&gt;();
        /// </summary>
        public class SignalRNotificationService : IRealtimeNotificationService
        {
            private readonly IHubContext<ChatHub, IChatClient> _hub;

            public SignalRNotificationService(IHubContext<ChatHub, IChatClient> hub)
            {
                _hub = hub;
            }

            // ── Push new message to receiver ──────────────────────────────────────────

            /// <inheritdoc/>
            public async Task SendMessageToUserAsync(string receiverId, MessageResponseDto message)
            {
                // Clients.User routes to ALL active connections of receiverId.
                // If the user is offline, SignalR silently drops the call — no exception.
                await _hub.Clients.User(receiverId).ReceiveMessage(message);
            }

            // ── Notify sender that their message was read ─────────────────────────────

            /// <inheritdoc/>
            public async Task NotifyMessageReadAsync(
                string originalSenderId,
                MessageReadNotificationDto notification)
            {
                await _hub.Clients.User(originalSenderId).MessageRead(notification);
            }

            // ── Presence ──────────────────────────────────────────────────────────────

            /// <inheritdoc/>
            public async Task NotifyPresenceAsync(
                string targetUserId,
                string onlineUserId,
                bool isOnline)
            {
                if (isOnline)
                    await _hub.Clients.User(targetUserId).UserOnline(onlineUserId);
                else
                    await _hub.Clients.User(targetUserId).UserOffline(onlineUserId);
            }

        /// <inheritdoc/>
        public async Task SendNotificationToUserAsync(string userId, NotificationResponseDto notification)
        {
            // Clients.User بتضمن إن الإشعار يروح لكل الأجهزة اللي المستخدم فاتح حسابه منها حالياً
            await _hub.Clients.User(userId).ReceiveNotification(notification);
        }
    }
    }

