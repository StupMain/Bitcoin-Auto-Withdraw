using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BitcoinAuto.Withdraw
{
    public class Telegram
    {
        TelegramBotClient botClient;
        string chatID = String.Empty;


        public Telegram(string token, string chat_id)
        {
            this.botClient = new TelegramBotClient(token);
            this.chatID = chat_id;
        }


        public async Task sendNotificationForNewTransaction()
        {
            await botClient.SendTextMessageAsync(chatID, "huy");
        }

        public async Task sendNotification(string message)
        {
            await botClient.SendTextMessageAsync(chatID, message);
        }
    }
}
