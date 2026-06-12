using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
namespace Presentation.Hubs
{
    

    
        /// <summary>
        /// Strongly-typed SignalR Hub for real-time chat.
        ///
        /// AUTHENTICATION
        /// ──────────────
        /// [Authorize] ensures only JWT-authenticated users can connect.
        /// The JWT token is extracted from the query string "?access_token=..."
        /// by the OnMessageReceived event configured in Program.cs — this is the
        /// standard pattern for WebSocket/SSE connections that cannot send headers.
        ///
        /// USER ROUTING
        /// ────────────
        /// Clients.User(userId) routes messages to ALL connections of a specific user
        /// (supports multiple devices / browser tabs).  SignalR maps the connection to
        /// a user via IUserIdProvider, which reads ClaimTypes.NameIdentifier from the
        /// validated JWT — matching exactly what your JWT middleware sets.
        ///
        /// PRESENCE TRACKING
        /// ─────────────────
        /// A static ConcurrentDictionary tracks userId → set of connectionIds.
        /// Static = shared across all hub instances in the same process.
        /// For multi-server deployments, replace with a Redis-backed store.
        /// </summary>
        [Authorize]
        public class ChatHub : Hub<IChatClient>
    {
            // ── Presence store: userId → { connectionId, connectionId, ... } ──────────
            private static readonly ConcurrentDictionary<string, HashSet<string>> _connections
                = new(StringComparer.OrdinalIgnoreCase);

            // ── Lock per user to safely mutate the inner HashSet ─────────────────────
            private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks
                = new(StringComparer.OrdinalIgnoreCase);

            // ════════════════════════════════════════════════════════════════════════
            // CONNECTION LIFECYCLE
            // ════════════════════════════════════════════════════════════════════════

            public override async Task OnConnectedAsync()
            {
                var userId = GetUserId();
                if (userId is null)
                {
                    Context.Abort();
                    return;
                }

                await TrackConnectionAsync(userId, Context.ConnectionId, connected: true);

                // Notify the connecting user of their own presence (useful for debugging)
                await Clients.Caller.UserOnline(userId);

                await base.OnConnectedAsync();
            }

            public override async Task OnDisconnectedAsync(Exception? exception)
            {
                var userId = GetUserId();
                if (userId is not null)
                {
                    bool isNowOffline = await TrackConnectionAsync(
                        userId, Context.ConnectionId, connected: false);

                    // Only broadcast offline if this was the user's LAST connection
                    if (isNowOffline)
                        await Clients.Others.UserOffline(userId);
                }

                await base.OnDisconnectedAsync(exception);
            }

            // ════════════════════════════════════════════════════════════════════════
            // PUBLIC PRESENCE UTILITY (used by SignalRNotificationService)
            // ════════════════════════════════════════════════════════════════════════

            /// <summary>Returns true if the user has at least one active connection.</summary>
            public static bool IsUserOnline(string userId)
                => _connections.TryGetValue(userId, out var conns) && conns.Count > 0;

            // ════════════════════════════════════════════════════════════════════════
            // PRIVATE HELPERS
            // ════════════════════════════════════════════════════════════════════════

            private string? GetUserId()
                => Context.UserIdentifier;   // set by IUserIdProvider from ClaimTypes.NameIdentifier

            /// <summary>
            /// Thread-safe add/remove of a connectionId for a userId.
            /// Returns true when the user transitions to "offline" (no more connections).
            /// </summary>
            private async Task<bool> TrackConnectionAsync(
                string userId,
                string connectionId,
                bool connected)
            {
                var semaphore = _locks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
                await semaphore.WaitAsync();

                try
                {
                    if (connected)
                    {
                        var conns = _connections.GetOrAdd(userId, _ => new HashSet<string>());
                        conns.Add(connectionId);
                        return false;   // not offline
                    }
                    else
                    {
                        if (_connections.TryGetValue(userId, out var conns))
                        {
                            conns.Remove(connectionId);
                            if (conns.Count == 0)
                            {
                                _connections.TryRemove(userId, out _);
                                return true;   // user is now offline
                            }
                        }
                        return false;
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }
    }

