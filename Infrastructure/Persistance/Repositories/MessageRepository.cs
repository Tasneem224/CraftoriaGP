using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Contracts;
using DomainLayer.Models.Chat;
using global::Persistance.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories
{
    

    
        /// <summary>
        /// Provides all database operations specific to the Message entity.
        /// Inherits CRUD from GenericRepository; adds complex chat-specific queries.
        /// </summary>
        public class MessageRepository : GenericRepository<Message, int>, IMessageRepository
        {
            private readonly StoreDbContext _context;

            public MessageRepository(StoreDbContext context) : base(context)
            {
                _context = context;
            }

            // ── 1. Get Conversation (paginated) ──────────────────────────────────────

            public async Task<IEnumerable<Message>> GetConversationAsync(
                string userId1,
                string userId2,
                int pageNumber,
                int pageSize)
            {
                return await _context.Messages
                    // All messages exchanged between exactly these two users
                    .Where(m =>
                        (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
                    // Newest first so the client can render immediately,
                    // then reverse client-side for chronological display
                    .OrderByDescending(m => m.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .AsNoTracking()
                    .ToListAsync();
            }

            // ── 2. Conversation count (for pagination meta) ──────────────────────────

            public async Task<int> GetConversationCountAsync(string userId1, string userId2)
            {
                return await _context.Messages
                    .Where(m =>
                        (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
                    .CountAsync();
            }

            // ── 3. Inbox — latest message per conversation ───────────────────────────

            public async Task<IEnumerable<Message>> GetInboxAsync(string userId)
            {
                // Step A: find the MAX message Id per conversation partner.
                // This avoids EF Core issues with FirstOrDefault inside GroupBy Select.
                var latestIds = await _context.Messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                    .Select(g => g.Max(m => m.Id))
                    .ToListAsync();

                // Step B: fetch those specific messages with navigation props loaded.
                return await _context.Messages
                    .Where(m => latestIds.Contains(m.Id))
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .OrderByDescending(m => m.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }

            // ── 4. Mark single message as read ───────────────────────────────────────

            public async Task<Message?> MarkMessageAsReadAsync(int messageId, string readerId)
            {
                // The readerId MUST be the receiver — prevents senders marking own msgs
                var message = await _context.Messages
                    .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == readerId);

                if (message is null || message.IsRead)
                    return null;   // not found, not authorised, or already read

                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                message.UpdatedAt = DateTime.UtcNow;

                // NOTE: SaveChanges is called by the SERVICE layer (Unit of Work pattern)
                return message;
            }

            // ── 5. Bulk mark conversation as read ─────────────────────────────────────

            public async Task<int> MarkConversationAsReadAsync(
                string currentUserId,
                string otherUserId)
            {
                var now = DateTime.UtcNow;

                // ExecuteUpdateAsync is EF Core 7+ — one round-trip, no object hydration
                return await _context.Messages
                    .Where(m =>
                        m.SenderId == otherUserId &&
                        m.ReceiverId == currentUserId &&
                        !m.IsRead)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(m => m.IsRead, m=>true)
                        .SetProperty(m => m.ReadAt,m=> now)
                        .SetProperty(m => m.UpdatedAt, m=>now));
            

        }

            // ── 6. Total unread count ─────────────────────────────────────────────────

            public async Task<int> GetTotalUnreadCountAsync(string userId)
            {
                return await _context.Messages
                    .Where(m => m.ReceiverId == userId && !m.IsRead)
                    .CountAsync();
            }

            // ── 7. Unread count per sender ────────────────────────────────────────────

            public async Task<Dictionary<string, int>> GetUnreadCountPerSenderAsync(
                string receiverId)
            {
                return await _context.Messages
                    .Where(m => m.ReceiverId == receiverId && !m.IsRead)
                    .GroupBy(m => m.SenderId)
                    .Select(g => new { SenderId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.SenderId, x => x.Count);
            }

            // ── 8. Get single message with users ─────────────────────────────────────

            public async Task<Message?> GetMessageByIdWithUsersAsync(
                int messageId,
                string userId)
            {
                return await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .FirstOrDefaultAsync(m =>
                        m.Id == messageId &&
                        (m.SenderId == userId || m.ReceiverId == userId));
            }

            // ── 9. Soft delete ─────────────────────────────────────────────────────────

            public async Task<bool> SoftDeleteMessageAsync(
                int messageId,
                string requestingUserId)
            {
                var message = await _context.Messages
                    .FirstOrDefaultAsync(m => m.Id == messageId && m.SenderId == requestingUserId);

                if (message is null) return false;

                message.IsDeleted = true;
                message.UpdatedAt = DateTime.UtcNow;

                return true;   // SaveChanges handled by UoW in Service layer
            }
        }
    }

