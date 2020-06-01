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
            if (location.Longitude > 45 || location.Longitude < 25.5 || 
                location.Latitude > 42.5 || location.Latitude < 36)
            {
                return await _telegramService.SendMessage(new TelegramMessage
                {
                    ChatId = chatId.ToString(),
                    Text = "Koordinatlar sınır dışında kalıyor."
                });
            }

            var response = await _amazonDynamoDbService.UpdateAsync(chatId, location.Latitude, location.Longitude);

            if (!response)
            {
                return await _telegramService.SendMessage(new TelegramMessage
                {
                    ChatId = chatId.ToString(),
                    Text = "Bir hata oluştu."
                });
            }

            var replyMarkup = new TelegramInlineKeyboardMarkup
            {
                InlineKeyboard = new List<IEnumerable<TelegramInlineKeyboardButton>>{
                    new List<TelegramInlineKeyboardButton>
                    {
                        new TelegramInlineKeyboardButton { Text = "250", CallBackData = "250" },
                        new TelegramInlineKeyboardButton { Text = "100", CallBackData = "100" },
                        new TelegramInlineKeyboardButton { Text = "50", CallBackData = "50" },
                        new TelegramInlineKeyboardButton { Text = "25", CallBackData = "25" },
                        new TelegramInlineKeyboardButton { Text = "10", CallBackData = "10" },
                        new TelegramInlineKeyboardButton { Text = "5", CallBackData = "5" },
                        new TelegramInlineKeyboardButton { Text = "2", CallBackData = "2" },
                        new TelegramInlineKeyboardButton { Text = "1", CallBackData = "1" }
                    }
                }
            };

            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = "Pafta ölçeğini seçiniz (1:___.000):",
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

            var messageText = $"Arama sonucu (1:{scale}.000): ";

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
