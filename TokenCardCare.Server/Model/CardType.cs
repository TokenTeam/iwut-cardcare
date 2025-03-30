namespace TokenCardCare.Server.Model;

public class CardType(string name, bool multiple, bool showAdding)
{
    public string Name { get; set; } = name;
    public bool Multiple { get; set; } = multiple;
    public bool ShowAdding { get; set; } = showAdding;

    public static readonly IReadOnlyCollection<CardType> List =
    [
        new CardType("身份证", false, true),
        new CardType("学生证", false, true),
        new CardType("银行卡", true, true),
        new CardType("校园卡", false, false)
    ];
}
