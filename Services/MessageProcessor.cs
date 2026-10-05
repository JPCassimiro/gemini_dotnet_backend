namespace jsonToGemin.Services;
using Telegram.Bot.Types;
using jsonToGemin.Interfaces;

public class MessageProcessor : IMessageProcessorInterface 
{
    private readonly IChatProviderService _telegram;
    private readonly CreatorController _creatorController;

    public MessageProcessor(
        IChatProviderService telegram,
        CreatorController creatorController)
    {
        _telegram = telegram;
        _creatorController = creatorController;
    }//telegram and aiService services intances

    //makes the bridge between recieving the message, getting the aiService response, and sending the response to the client
    public async Task Process(Update update)
    {
        try
        {
            Console.WriteLine($"MessageProcessor first");
            AiServiceInterface serviceProvider = _creatorController.FetchProvider("gemini");
            var message = update.Message?.Text;

            if (string.IsNullOrWhiteSpace(message))
                return;

            var chatId = update.Message!.Chat.Id;

            var resp = await serviceProvider.GenerateAiResponse(message, "model");

            Console.WriteLine($"MessageProcessor Process resp: {resp}");

            await _telegram.SendMessage(chatId, resp);    
        }
        catch (Exception e)
        {
            Console.WriteLine($"MessageProcessor Process error: {e}");
            throw;
        }
    }

    public async Task<string> AngProcess(string provider, string model, string message)
    {
        try
        {
            AiServiceInterface serviceProvider = _creatorController.FetchProvider(provider);

            if (string.IsNullOrWhiteSpace(message))
            {
                return "";
            }

            string resp = await serviceProvider.GenerateAiResponse(message, model);

            return resp;
        } catch(Exception e)
        {
            Console.WriteLine($"MessageProcessor AngProcess error: {e}");
            return "Erro ao processar sua mensagem, tente utilizar um modelo diferente";
            // throw;
        }
    }
}