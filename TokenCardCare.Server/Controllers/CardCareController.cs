using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TokenCardCare.Server.Entity;
using TokenCardCare.Server.Model;

namespace TokenCardCare.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CardCareController(IConfiguration configuration, AppDbContext dbContext) : ControllerBase
{
    private static IDictionary<string, CardType> CardTypeDict = CardType.List.ToDictionary(x => x.Name, x => x);

    [HttpGet("CardTypes")]
    public IActionResult GetCardTypes()
    {
        return Ok(ApiResponse.Success("获取卡片类型成功", CardType.List));
    }

    [HttpGet("Salt")]
    public IActionResult GetSalt()
    {
        var salt = configuration.GetSection("Salt").Get<string>();
        return Ok(ApiResponse.Success("获取Salt成功", salt));
    }

    [HttpPost("NewCard")]
    public async Task<IActionResult> AddCard([FromBody] ApiRequestModel.NewCard newCard)
    {
        if (!CardTypeDict.TryGetValue(newCard.cardType, out CardType? cardType))
        {
            return Ok(ApiResponse.Fail(1, "不存在的卡片类型"));
        }

        var existingCardQuery = dbContext.Cards.AsNoTracking()
            .Where(x => x.Hash == newCard.hash)
            .Where(x => x.Type == newCard.cardType);
        if (existingCardQuery.Any())
        {
            return Ok(ApiResponse.Fail(1, "已存在相同的卡片"));
        }

        if (!cardType.Multiple)
        {
            var existingSingleCardQuery = dbContext.Cards.AsNoTracking()
                .Where(x => x.Sno == newCard.studentNumber)
                .Where(x => x.Type == newCard.cardType);
            if (existingSingleCardQuery.Any())
            {
                return Ok(ApiResponse.Fail(1, "该类型卡片只能添加一张"));
            }
        }

        // 在招领数据库中查找是否被找到
        var foundCard = dbContext.FoundCards
            .Where(x => x.Type == newCard.cardType)
            .Where(x => x.Hash == newCard.hash)
            .SingleOrDefault();

        var card = new Card
        {
            Sno = newCard.studentNumber,
            Name = newCard.cardName,
            State = foundCard is null ? CardState.Normal : CardState.Found,
            Type = newCard.cardType,
            Hash = newCard.hash
        };

        if (foundCard is not null)
        {
            dbContext.FoundCards.Remove(foundCard);
        }

        await dbContext.Cards.AddAsync(card).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return Ok(ApiResponse.Success("添加卡片成功"));
    }

    [HttpPost("RemoveCard")]
    public async Task<IActionResult> RemoveCard([FromBody] ApiRequestModel.RemoveCard removingCard)
    {
        var existingCardQuery = dbContext.Cards.AsNoTracking()
            .Where(x => x.Sno == removingCard.studentNumber)
            .Where(x => x.Hash == removingCard.hash);

        var cardToRemove = existingCardQuery.SingleOrDefault();
        if (cardToRemove is not null)
        {
            dbContext.Cards.Remove(cardToRemove);
        }

        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return Ok(ApiResponse.Success("删除卡片成功"));
    }

    [HttpPost("GetCards")]
    public async Task<IActionResult> GetCards([FromBody] ApiRequestModel.GetCards req)
    {
        var defaultCardHash = CalcHash(req.studentNumber, "校园卡", configuration.GetSection("Salt").Get<string>()!);
        var defaltCardQuery = dbContext.Cards.AsNoTracking().Where(x => x.Sno == req.studentNumber)
            .Where(x => x.Hash == defaultCardHash);
        if (!defaltCardQuery.Any())
        {
            // 在招领数据库中查找是否被找到
            var foundCard = dbContext.FoundCards
                .Where(x => x.Type == "校园卡")
                .Where(x => x.Hash == defaultCardHash)
                .SingleOrDefault();

            var defualtCard = new Card
            {
                Name = "我的校园卡",
                Type = "校园卡",
                Sno = req.studentNumber,
                Hash = defaultCardHash,
                State = foundCard is null ? CardState.Normal : CardState.Found
            };

            if (foundCard is not null)
            {
                dbContext.FoundCards.Remove(foundCard);
            }

            dbContext.Cards.Add(defualtCard);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var cards = dbContext.Cards.AsNoTracking()
            .Where(x => x.Sno == req.studentNumber)
            .Select(x => new CardInfo
            {
                CardName = x.Name,
                CardType = x.Type,
                Hash = x.Hash,
                State = x.State
            });

        var res = await cards.ToArrayAsync().ConfigureAwait(false);

        return Ok(ApiResponse.Success("获取卡片列表成功", res));
    }

    [HttpPost("ResetCard")]
    public async Task<IActionResult> ResetCard([FromBody] ApiRequestModel.ResetCard req)
    {
        var cardQuery = dbContext.Cards.Where(x => x.Sno == req.studentNumber)
            .Where(x => x.Hash == req.hash);
        var card = cardQuery.SingleOrDefault();
        if (card is null)
        {
            return Ok(ApiResponse.Fail(-1, "卡片不存在"));
        }

        card.State = CardState.Normal;
        await dbContext.SaveChangesAsync().ConfigureAwait(false);

        return Ok(ApiResponse.Success("重置卡片成功"));
    }

    [HttpPost("CheckCard")]
    public async Task<IActionResult> CheckCard([FromBody] ApiRequestModel.CheckCard req)
    {
        var cardQuery = dbContext.Cards.Where(x => x.Type == req.cardType)
            .Where(x => x.Hash == req.hash);

        var card = cardQuery.SingleOrDefault();

        // 卡片不存在，加入招领数据库
        if (card is null)
        {
            var foundCard = new FoundCard
            {
                Type = req.cardType,
                Hash = req.hash,
                FoundTime = DateTime.Now
            };

            dbContext.FoundCards.Add(foundCard);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return Ok(ApiResponse.Success("卡片不存在", new
            {
                Match = false
            }));
        }

        // 卡片存在，变更状态为已找到
        card.State = CardState.Found;
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
        return Ok(ApiResponse.Success("卡片存在", new
        {
            Match = true
        }));
    }

    private static string CalcHash(string id, string cardType, string salt)
    {
        var str = $"{id}{cardType}{salt}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(str));
        return Convert.ToBase64String(hashBytes);
    }
}
