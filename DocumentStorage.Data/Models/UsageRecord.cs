using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

public partial class UsageRecord
{
    [Key]
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    [StringLength(100)]
    public string ResourceType { get; set; } = null!;

    public long UsedValue { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PeriodStart { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PeriodEnd { get; set; }

    [ForeignKey("SubscriptionId")]
    [InverseProperty("UsageRecords")]
    public virtual Subscription Subscription { get; set; } = null!;
}
