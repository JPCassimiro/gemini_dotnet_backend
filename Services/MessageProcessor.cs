namespace jsonToGemin.Services;
using Telegram.Bot.Types;

public class MessageProcessor
{
    private readonly TelegramService _telegram;
    private readonly GeminiService _gemini;

    public MessageProcessor(
        TelegramService telegram,
        GeminiService gemini)
    {
        _telegram = telegram;
        _gemini = gemini;
    }//telegram and gemini services intances

    //makes the bridge between recieving the message, getting the gemini response, and sending the response to the client
    public async Task Process(Update update)
    {
        try
        {
            var message = update.Message?.Text;

            if (string.IsNullOrWhiteSpace(message))
                return;

            var chatId = update.Message!.Chat.Id;

            var resp = await _gemini.SendToGemini(message);

            Console.WriteLine($"MessageProcessor Process resp: {resp}");

            await _telegram.SendMessage(chatId, resp);    
        }
        catch (Exception e)
        {
            Console.WriteLine($"MessageProcessor Process error: {e}");
            throw;
        }
        
    }

    public async Task<string> AngProcess(string message)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "";
            }

            var resp = await _gemini.SendToGemini(message);

            return resp;
        } catch(Exception e)
        {
            Console.WriteLine($"MessageProcessor AngProcess error: {e}");
            throw;
        }
    }
}