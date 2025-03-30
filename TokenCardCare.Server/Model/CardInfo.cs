namespace TokenCardCare.Server.Model;

public class CardInfo
{
    public string CardName { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public CardState State { get; set; }
}

public enum CardState
{
    Normal,
    Found
}
