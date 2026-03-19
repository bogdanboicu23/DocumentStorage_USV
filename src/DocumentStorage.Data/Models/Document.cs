using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocumentStorage.Data.Models;

public partial class Document
{
    [Key]
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    [StringLength(255)]
    public string FileName { get; set; } = null!;

    public long SizeBytes { get; set; }

    [StringLength(100)]
    public string ContentType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Documents")]
    public virtual Account Account { get; set; } = null!;
}
