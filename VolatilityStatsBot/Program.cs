using InvestToolsBot.HandlersCache;
using InvestToolsBot;
using Telegram.Bot.Polling;
using Telegram.Bot;
using InvestToolsBot.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

Console.WriteLine("Запущен бот " + MainClass.bot.GetMeAsync().Result.FirstName);

Console.WriteLine("Заполнение кэша хэндлеров...");

var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass && t.IsSubclassOf(typeof(BaseHandler)));

foreach (var type in types)
{
    var handler = (BaseHandler)Activator.CreateInstance(type) ?? throw new Exception($"Type {type.Name} is not a BaseHandler!");

    HandlersCache.Add(handler.GetRequestType(), handler.GetEmojiCommand(), handler.Handle);
}

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, // receive all update types
};
MainClass.bot.StartReceiving(
    MainClass.HandleUpdateAsync,
    MainClass.HandleErrorAsync,
    receiverOptions,
    cancellationToken
);

app.Run();
