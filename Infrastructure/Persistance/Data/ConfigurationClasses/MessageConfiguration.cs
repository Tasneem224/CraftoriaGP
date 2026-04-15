using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.ConfigurationClasses
{
    

    
        /// <summary>
        /// EF Core Fluent-API configuration for the Message entity.
        ///
        /// KEY DECISIONS:
        /// ─────────────
        /// • Two FKs to the same AspNetUsers table (Sender + Receiver).
        ///   Both use DeleteBehavior.Restrict to prevent cascade-delete conflicts.
        /// • Global query filter for soft-deletes (IsDeleted == false).
        /// • Indexes on SenderId, ReceiverId, and CreatedAt for query performance.
        /// </summary>
        public class MessageConfiguration : IEntityTypeConfiguration<Message>
        {
            public void Configure(EntityTypeBuilder<Message> builder)
            {
                // ── Table ────────────────────────────────────────────────────────────
                builder.ToTable("Messages");

                // ── PK ───────────────────────────────────────────────────────────────
                builder.HasKey(m => m.Id);

                // ── Properties ───────────────────────────────────────────────────────
                builder.Property(m => m.SenderId)
                       .IsRequired()
                       .HasMaxLength(450);   // matches nvarchar(450) of AspNetUsers.Id

                builder.Property(m => m.ReceiverId)
                       .IsRequired()
                       .HasMaxLength(450);

                builder.Property(m => m.Content)
                       .IsRequired()
                       .HasMaxLength(2000);

                builder.Property(m => m.CreatedAt)
                       .IsRequired();

                builder.Property(m => m.CreatedBy)
                       .HasMaxLength(450);

                // ── Relationships ─────────────────────────────────────────────────────
                //
                // Sender FK — Restrict so deleting a user does not cascade-delete all
                // sent messages automatically (handle that in application logic).
                builder.HasOne(m => m.Sender)
                       .WithMany()
                       .HasForeignKey(m => m.SenderId)
                       .OnDelete(DeleteBehavior.Restrict);

                // Receiver FK — same rationale.
                builder.HasOne(m => m.Receiver)
                       .WithMany()
                       .HasForeignKey(m => m.ReceiverId)
                       .OnDelete(DeleteBehavior.Restrict);

                // ── Global Query Filter ───────────────────────────────────────────────
                // All LINQ queries on DbSet<Message> will automatically exclude
                // soft-deleted messages — no need to add .Where(m => !m.IsDeleted)
                // in every query.
                builder.HasQueryFilter(m => !m.IsDeleted);

                // ── Indexes ───────────────────────────────────────────────────────────
                // Composite index covering the most frequent query pattern:
                // "give me all messages between user A and user B ordered by time"
                builder.HasIndex(m => new { m.SenderId, m.ReceiverId, m.CreatedAt })
                       .HasDatabaseName("IX_Messages_Conversation");

                // Separate index for inbox query (all messages involving a userId)
                builder.HasIndex(m => m.ReceiverId)
                       .HasDatabaseName("IX_Messages_ReceiverId");

                builder.HasIndex(m => m.SenderId)
                       .HasDatabaseName("IX_Messages_SenderId");
            }
        }
    }

