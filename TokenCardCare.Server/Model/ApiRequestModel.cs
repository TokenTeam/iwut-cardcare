using System.ComponentModel.DataAnnotations;

namespace TokenCardCare.Server.Model
{
    public class ApiRequestModel
    {
        public record NewCard([Required] string studentNumber, [Required] string cardName, string cardType, [Required] string hash);
        public record RemoveCard([Required] string studentNumber, [Required] string hash);
        public record GetCards([Required] string studentNumber);
        public record ResetCard([Required] string studentNumber, [Required] string hash);
        public record CheckCard(string cardType, [Required] string hash);
    }
}
