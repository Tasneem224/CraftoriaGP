using DomainLayer.Models.ChatBot;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using DomainLayer.Models.Messages;
using DomainLayer.Models.Notifications;
using DomainLayer.Models.RawMaterials;
using DomainLayer.Models.session;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DomainLayer.Models.Identity
{
    public class ApplicationUser : IdentityUser

    {

        public string DisplayName { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string SecondName { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public string? ProfileImage { get; set; } = default!;
        public string? Bio { get; set; } = string.Empty;
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiration { get; set; }

        #region Artisan
        public decimal? CommissionRate { get; set; }//by default is 10%
        public string? Specialization { get; set; } = default!;
        #endregion

        #region Expert_artisan
        public string? Portfolio { get; set; }
        public int? YearsOfExperience { get; set; }

        #endregion

        // ضيفي دول جوه الـ ApplicationUser class
        // جوه ApplicationUser.cs
        #region Booking_System_Relations

        [InverseProperty(nameof(ExpertService.Expert))]
        public virtual ICollection<ExpertService> ExpertServices { get; set; }

        [InverseProperty(nameof(ExpertAvailability.Expert))]
        public virtual ICollection<ExpertAvailability> Availabilities { get; set; }

        // الجلسات اللي أنا "خبير" فيها (بقدم فيها النصيحة)
        [InverseProperty(nameof(Session.Expert))]
        public virtual ICollection<Session> SessionsAsExpert { get; set; }

        // الجلسات اللي أنا "بائع مبتدئ" فيها (حاجزها عشان أتعلم)
        [InverseProperty(nameof(Session.Beginner))]
        public virtual ICollection<Session> SessionsAsBeginner { get; set; }

        #endregion

        // virtual for lazy loading

        [InverseProperty(nameof(Product.Seller))]
        public virtual ICollection<Product>? Products { get; set; }

        [InverseProperty(nameof(RawMaterial.supplier))]
        public virtual ICollection<RawMaterial>? rawMaterials { get; set; }

        //(Reviewer)
        [InverseProperty(nameof(UserInteraction.User))]
        public ICollection<UserInteraction> WrittenReviews { get; set; }

        // 2. (Seller Reviews)
        [InverseProperty(nameof(UserInteraction.TargetUser))]
        public ICollection<UserInteraction> ReceivedReviews { get; set; }

        #region Messages_And_Notifications

        // الرسائل اللي اليوزر بعتها
        [InverseProperty(nameof(Message.Sender))]
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();

        // الرسائل اللي اليوزر استقبلها
        [InverseProperty(nameof(Message.Receiver))]
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        // إشعارات اليوزر
        [InverseProperty(nameof(Notification.User))]
        public virtual ICollection<Notification> UserNotifications { get; set; } = new List<Notification>();

        #endregion

        public ICollection<ChatBotMessages> ChatBotMessages { get; set; }
    }
}
