using InvestToolsBot.Abstract;

namespace InvestToolsBot.Classifiers
{
    public class ContainsClassifier : BaseClassifier
    {
        public ContainsClassifier(Dictionary<string, Delegate> handlers, Telegram.Bot.Types.Update update) : base(handlers, update) { }
        public override Delegate? Classify()
        {
            foreach (var handler in _handlers)
            {
                if (_update.Message == null)
                    continue;

                if (_update.Message.Text == null)
                    continue;

                if (_update.Message.Text.Contains(handler.Key))
                    return handler.Value;
            }

            return null;
        }
    }
}
