using DomainLayer.Models.session;
using DomainLayer.Models.session; // اتأكدي من الـ Namespace الصح لموديل الـ Session
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Data.ConfigurationClasses
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            // 1. ربط علاقة الخبير (الطرف اللي بيقدم النصيحة)
            builder.HasOne(s => s.Expert)
                   .WithMany(u => u.SessionsAsExpert) // دي الـ Collection اللي لسه ضايفاها في الـ User
                   .HasForeignKey(s => s.ExpertId)
                   .OnDelete(DeleteBehavior.NoAction);

            // 2. ربط علاقة المبتدئ (الطرف اللي حاجز الجلسة)
            builder.HasOne(s => s.Beginner)
                   .WithMany(u => u.SessionsAsBeginner) // دي الـ Collection التانية في الـ User
                   .HasForeignKey(s => s.BeginnerId)
                   .OnDelete(DeleteBehavior.NoAction);

            // 3. علاقة الخدمة
            builder.HasOne(s => s.Service)
                   .WithMany() // لو مفيش Collection جلسات جوه كلاس الخدمة سيبيها فاضية
                   .HasForeignKey(s => s.ExpertServiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 4. علاقة الموعد المتاح
            builder.HasOne(s => s.Availability)
                   .WithMany()
                   .HasForeignKey(s => s.ExpertAvailabilityId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ضبط الـ Decimal لخانة السعر (AmountPaid)
            builder.Property(s => s.AmountPaid)
                   .HasColumnType("decimal(18,2)");
        }
    }
}