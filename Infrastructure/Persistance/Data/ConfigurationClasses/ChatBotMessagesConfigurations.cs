using DomainLayer.Models.ChatBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Data.ConfigurationClasses
{
    public class ChatBotMessagesConfigurations : IEntityTypeConfiguration<ChatBotMessages>
    {
        public void Configure(EntityTypeBuilder<ChatBotMessages> builder)
        {
            builder.HasOne(m => m.User)
        .WithMany(u => u.ChatBotMessages)
        .HasForeignKey(m => m.UserId)
        .OnDelete(DeleteBehavior.Cascade);

         builder.HasIndex(m => m.UserId);
        }
    }
}
