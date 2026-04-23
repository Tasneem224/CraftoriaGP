using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Chat
{
   
        /// <summary>
        /// Represents a direct chat message between two ApplicationUsers.
        ///
        /// ⚠️  AUDIT NOTE: If your BaseEntity&lt;TKey&gt; already declares CreatedAt,
        /// CreatedBy, UpdatedAt, and IsDeleted, remove those four properties from
        /// this class to avoid duplication. They are kept here because the
        /// requirement explicitly asks for them on the Message entity.
        /// </summary>
        public class Message : BaseEntity<int>
        {
            // ── Foreign Keys ────────────────────────────────────────────────────────

            /// <summary>
            /// ID of the user who SENT the message.
            /// ALWAYS populated from the authenticated user's JWT claims on the
            /// server — never accepted from the client payload.  This is the fix
            /// for the FK constraint error: the client cannot supply a fake / missing
            /// SenderId that does not exist in AspNetUsers.
            /// </summary>
            public string SenderId { get; set; } = string.Empty;

            /// <summary>
            /// ID of the user who will RECEIVE the message.
            /// </summary>
            public string ReceiverId { get; set; } = string.Empty;

            // ── Message Content ─────────────────────────────────────────────────────

            /// <summary>Text body of the message.</summary>
            public string Content { get; set; } = string.Empty;

            // ── Read-Receipt ────────────────────────────────────────────────────────

            /// <summary>Has the receiver opened / acknowledged this message?</summary>
            public bool IsRead { get; set; } = false;

            /// <summary>UTC timestamp of when the receiver read the message.</summary>
            public DateTime? ReadAt { get; set; }

            // ── Audit Fields ────────────────────────────────────────────────────────
            // Remove these four if BaseEntity<TKey> already provides them.

            /// <summary>UTC timestamp when this record was created (message sent).</summary>
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Mirrors SenderId — satisfies a generic audit trail (who created the row).
            /// </summary>
            public string? CreatedBy { get; set; }

            /// <summary>UTC timestamp of the last modification (e.g., IsRead flip).</summary>
            public DateTime? UpdatedAt { get; set; }

            /// <summary>
            /// Soft-delete flag.  True = logically deleted.
            /// Queries MUST filter IsDeleted == false via a global query filter.
            /// </summary>
            public bool IsDeleted { get; set; } = false;

            // ── Navigation Properties ────────────────────────────────────────────────

            /// <summary>EF Core navigation → the sending ApplicationUser.</summary>
            public ApplicationUser? Sender { get; set; }

            /// <summary>EF Core navigation → the receiving ApplicationUser.</summary>
            public ApplicationUser? Receiver { get; set; }
        }
    }

