using Telegram.Bot;
using jsonToGemin.Interfaces;
using Telegram.Bot.Types;

namespace jsonToGemin.Services;


public class TelegramService: IChatProviderService
{
    private readonly TelegramBotClient _bot;//bot variable
    private readonly string telegramKey = DotNetEnv.Env.GetString("TELEGRAM_API");
    private string _currentModel;
    private string _currentProvider;
    public string currentModel { get => _currentModel; set => _currentModel = value; }
    public string currentProvider { get => _currentProvider; set => _currentProvider = value; }

    public TelegramService(IConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(telegramKey))
        {
            throw new InvalidOperationException("Unable to get TELEGRAM_API");
        }

        _bot = new TelegramBotClient(telegramKey);//new bot instance

    }

    //sends a response message to the client
    public async Task SendMessage(long id, string text)
    {
        try
        {
            Console.WriteLine($"TelegramService SendMessage");
            await _bot.SendMessage(chatId: id, text: text);
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"TelegramService SendMessage error: {e}");
        }
    }
}