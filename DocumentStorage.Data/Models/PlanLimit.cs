using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

public partial class PlanLimit
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlanId { get; set; }

    [StringLength(100)]
    public string ResourceType { get; set; } = null!;

    public long MaxValue { get; set; }

    public bool IsHardLimit { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("PlanLimits")]
    public virtual Plan Plan { get; set; } = null!;
}
