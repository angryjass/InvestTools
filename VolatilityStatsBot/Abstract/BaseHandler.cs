using Telegram.Bot;
using Telegram.Bot.Types;

namespace InvestToolsBot.Abstract
{
    public abstract class BaseHandler
    {
        public abstract Task Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        public abstract string GetEmojiCommand();
        public abstract Telegram.Bot.Types.Enums.UpdateType GetRequestType();
    }
}
