using Shared.Chat;
using Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServiceAbstraction
{

    
        /// <summary>
        /// ════════════════════════════════════════════════════════════════════════
        ///  THE CIRCULAR-DEPENDENCY BRIDGE
        /// ════════════════════════════════════════════════════════════════════════
        ///
        /// PROBLEM
        /// ───────
        /// MessageService (Service project) must push real-time events.
        /// SignalR lives in ChatHub (Presentation project).
        /// Service CANNOT reference Presentation → circular dependency.
        ///
        /// SOLUTION — Dependency Inversion (SOLID - D)
        /// ────────────────────────────────────────────
        /// 1. Place this INTERFACE in ServiceAbstraction (neutral layer).
        /// 2. MessageService depends only on this interface — knows nothing about
        ///    SignalR or IHubContext.
        /// 3. SignalRNotificationService (Presentation project) IMPLEMENTS the
        ///    interface using IHubContext&lt;ChatHub, IChatClient&gt;.
        /// 4. CraftoriaApp/Program.cs wires the two together via DI:
        ///       services.AddScoped&lt;IRealtimeNotificationService,
        ///                           SignalRNotificationService&gt;();
        ///
        /// Dependency graph (no cycles):
        ///   Service → ServiceAbstraction ← Presentation → SignalR
        /// ════════════════════════════════════════════════════════════════════════
        /// </summary>
        public interface IRealtimeNotificationService
        {
            /// <summary>
            /// Pushes a new message to ALL active connections of the specified receiver.
            /// Invokes the "ReceiveMessage" client method on ChatHub.
            /// </summary>
            Task SendMessageToUserAsync(string receiverId, MessageResponseDto message);

            /// <summary>
            /// Notifies the original SENDER that their message was read.
            /// Invokes the "MessageRead" client method on ChatHub.
            /// </summary>
            Task NotifyMessageReadAsync(string originalSenderId, MessageReadNotificationDto notification);

            /// <summary>
            /// Broadcasts online/offline status changes to the receiver.
            /// Invokes "UserOnline" or "UserOffline" on ChatHub.
            /// </summary>
            Task NotifyPresenceAsync(string targetUserId, string onlineUserId, bool isOnline);

        /// <summary>
        /// Pushes a general notification to all active connections of a specific user.
        /// </summary>
        Task SendNotificationToUserAsync(string userId, NotificationResponseDto notification);


    }
    }

