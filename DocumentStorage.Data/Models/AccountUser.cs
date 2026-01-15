using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

[PrimaryKey("AccountId", "UserId")]
public partial class AccountUser
{
    [Key]
    public Guid AccountId { get; set; }

    [Key]
    public Guid UserId { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = null!;

    [ForeignKey("AccountId")]
    [InverseProperty("AccountUsers")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AccountUsers")]
    public virtual User User { get; set; } = null!;
}
