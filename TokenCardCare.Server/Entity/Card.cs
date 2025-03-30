using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TokenCardCare.Server.Model;

namespace TokenCardCare.Server.Entity;

public class Card
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    [StringLength(8)]
    public string Sno { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public CardState State { get; set; }
}
