using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

public partial class Account
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string AccountType { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();

    [InverseProperty("Account")]
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [InverseProperty("Account")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
