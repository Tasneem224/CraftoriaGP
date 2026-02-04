using DomainLayer.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    [PrimaryKey(nameof(userId),nameof(ItemId))]
    public class UserProductInteract
    {
            string userId { get; set; }
            [ForeignKey(nameof(userId))]
            public ApplicationUser user { get; set; }

            public int ItemId { get; set; }
            public bool? IsFavourite { get; set; }

            [Range(1, 5)]
            public short? Rating { get; set; }
            public string? Review { get; set; }
            public DateTime InteractionDate { get; set; } = DateTime.UtcNow;

        }
    }
