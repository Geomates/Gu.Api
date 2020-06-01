using Gu.PaftaBulucu.Bot.Models.Telegram;
using Gu.PaftaBulucu.Business.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Gu.PaftaBulucu.Bot.Models;

namespace Gu.PaftaBulucu.Bot.Services
{
    public interface IBotService
    {
        Task<bool> AskScaleAsync(int chatId, Location location);
        Task<bool> QuerySheetAsync(int linkedMessageId, string callbackQueryId, int chatId, int scale);
    }

    public class BotService : IBotService
    {
        private readonly ITelegramService _telegramService;
        private readonly IAmazonDynamoDbService _amazonDynamoDbService;
        private readonly ISheetService _sheetService;

        public BotService(ITelegramService telegramService, IAmazonDynamoDbService amazonDynamoDbService, ISheetService sheetService)
        {
            _telegramService = telegramService;
            _amazonDynamoDbService = amazonDynamoDbService;
            _sheetService = sheetService;
        }

        public async Task<bool> AskScaleAsync(int chatId, Location location)
        {
            var response = await _amazonDynamoDbService.UpdateAsync(chatId, location.Latitude, location.Longitude);

            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = "Bir hata oluştu."
            };

            if (!response) 
                return await _telegramService.SendMessage(message);

            var replyMarkup = new TelegramInlineKeyboardMarkup
            {
                InlineKeyboard = new List<IEnumerable<TelegramInlineKeyboardButton>>{
                    new List<TelegramInlineKeyboardButton>
                    {
                        new TelegramInlineKeyboardButton { Text = "1:250.000", CallBackData = "250" },
                        new TelegramInlineKeyboardButton { Text = "1:100.000", CallBackData = "100" },
                        new TelegramInlineKeyboardButton { Text = "1:50.000", CallBackData = "50" },
                        new TelegramInlineKeyboardButton { Text = "1:25.000", CallBackData = "25" },
                        new TelegramInlineKeyboardButton { Text = "1:10.000", CallBackData = "10" },
                        new TelegramInlineKeyboardButton { Text = "1:5.000", CallBackData = "5" },
                        new TelegramInlineKeyboardButton { Text = "1:2.000", CallBackData = "2" },
                        new TelegramInlineKeyboardButton { Text = "1:1.000", CallBackData = "1" }
                    }
                }
            };

            message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = "Pafta ölçeğini seçiniz:",
                ReplyMarkup = JsonConvert.SerializeObject(replyMarkup)
            };

            return await _telegramService.SendMessage(message);
        }

        public async Task<bool> QuerySheetAsync(int linkedMessageId, string callbackQueryId, int chatId, int scale)
        {
            var coordinates = await _amazonDynamoDbService.QueryAsync(chatId);

            var sheets = _sheetService.GetSheetsByCoordinate(coordinates.lat, coordinates.lon, scale);

            if (!sheets.Any())
            {
                return false;
            }

            var deleteMessage = new TelegramDeleteMessage
            {
                ChatId = chatId,
                MessageId = linkedMessageId
            };
            await _telegramService.DeleteMessage(deleteMessage);

            var messageText = "Arama sonucu: ";

            if (sheets.Count() > 1)
            {
                messageText = "\nBirden fazla pafta ile kesişiyor:\n";
                foreach (var sheetDto in sheets)
                {
                    messageText += "\n" + sheetDto.Name;
                }
            }
            else
            {
                messageText += sheets.FirstOrDefault().Name;
            }

            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = messageText
            };

            return await _telegramService.SendMessage(message);
        }
    }
}
