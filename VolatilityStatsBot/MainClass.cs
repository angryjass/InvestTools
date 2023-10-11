using InvestToolsBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace InvestToolsBot
{
    public static class MainClass
    {
        public static ITelegramBotClient bot = new TelegramBotClient("6609972088:AAFQZGNKg-XEItnYGcoD6shvlkf-JeuHOyU");

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

                var handlers = HandlersCache.HandlersCache.Get(update.Type);

                var classifiersTypes = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && t.IsSubclassOf(typeof(BaseClassifier)));

                Delegate? classifiedHandler = null;

                foreach (var classifierType in classifiersTypes) 
                {
                    var classifier = (BaseClassifier)Activator.CreateInstance(classifierType, handlers, update) ?? throw new Exception($"Type {classifierType.Name} is not a BaseClassifier!");
                    classifiedHandler = classifier.Classify();

                    if (classifiedHandler != null) break;
                }

                if (classifiedHandler == null)
                    throw new Exception($"No handler for {update.Type}");

                var task = (Task)classifiedHandler.DynamicInvoke(botClient, update, cancellationToken);

                if (task != null) await task;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
