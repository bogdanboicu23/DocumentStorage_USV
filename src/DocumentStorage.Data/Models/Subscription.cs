using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

public partial class Subscription
{
    [Key]
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid PlanId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime PeriodStart { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PeriodEnd { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Subscriptions")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("PlanId")]
    [InverseProperty("Subscriptions")]
    public virtual Plan Plan { get; set; } = null!;

    [InverseProperty("Subscription")]
    public virtual ICollection<UsageRecord> UsageRecords { get; set; } = new List<UsageRecord>();
}
