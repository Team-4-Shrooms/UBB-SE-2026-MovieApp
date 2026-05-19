
namespace MovieApp.DataLayer.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class PriceWatcher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int EventId { get; set; }

    public string EventTitle { get; set; } = string.Empty;

    public decimal TargetPrice { get; set; }
}
