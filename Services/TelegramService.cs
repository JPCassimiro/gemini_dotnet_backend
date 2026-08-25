using Telegram.Bot;

namespace jsonToGemin.Services;

public class TelegramService
{
    private readonly TelegramBotClient _bot;//bot variable

    public TelegramService(IConfiguration config)
    {
        _bot = new TelegramBotClient("TELEGRAM_KEY");//new bot instance
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