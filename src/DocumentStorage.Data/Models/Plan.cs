using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

[Index("Code", Name = "UQ__Plans__A25C5AA734660007", IsUnique = true)]
public partial class Plan
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(50)]
    public string BillingCycle { get; set; } = null!;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<PlanLimit> PlanLimits { get; set; } = new List<PlanLimit>();

    [InverseProperty("Plan")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
