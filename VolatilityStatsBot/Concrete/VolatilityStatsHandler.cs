using InvestToolsBot.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VolatilityStats;

namespace InvestToolsBot.Concrete
{
    public class VolatilityStatsHandler : BaseHandler
    {
        public override string GetEmojiCommand()
        {
            return "/volatility";
        }

        public override UpdateType GetRequestType()
        {
            return UpdateType.Message;
        }

        public override async Task Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var message = update.Message ?? throw new Exception("Message is null");

                if (message.Text == null)
                    throw new Exception("Message.Text is null!");

                var arr = message.Text.Replace("@invest_tools_bot", "").Split(' ');

                if (arr.Length < 3)
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Брат, команда имеет вид - /volatility [тикер] [период]");
                }

                var period = arr[2].Trim();
                var ticker = arr[1].Trim().ToUpper();

                if (!Enum.TryParse(typeof(PeriodEnum), period, true, out var _))
                {
                    await botClient.SendTextMessageAsync(message.Chat, 
                        "Брат, допустимы только периоды " + string.Join(", ", Enum.GetValues<PeriodEnum>().Select(a => a.ToString()).ToArray()));
                }

                var periodInEnum = (PeriodEnum)Enum.Parse(typeof(PeriodEnum), period, true);


                var volatility = await new VolatilityStatsAnalyser()
                    .GetVolatilityAsync(periodInEnum, ticker, cancellationToken);

                await botClient.SendTextMessageAsync(message.Chat, "Волатильность этой грязной бумажки " + volatility.ToString() + "% брат");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
