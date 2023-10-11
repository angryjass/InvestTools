namespace InvestToolsBot.Abstract
{
    public abstract class BaseClassifier
    {
        protected Dictionary<string, Delegate> _handlers;
        protected Telegram.Bot.Types.Update _update;

        protected BaseClassifier(Dictionary<string, Delegate> handlers, Telegram.Bot.Types.Update update)
        {
            _handlers = handlers;
            _update = update;
        }

        public abstract Delegate? Classify();
    }
}
