using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TokenCardCare.Server.Entity;

public class FoundCard
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Type { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public DateTime FoundTime { get; set; }
}
