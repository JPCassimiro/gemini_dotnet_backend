namespace jsonToGemin.Services;
using Telegram.Bot.Types;
using jsonToGemin.Interfaces;
using Telegram.Bot.Types.Enums;

public class MessageProcessor : IMessageProcessorInterface 
{
    private readonly IChatProviderService _telegram;
    private readonly CreatorController _creatorController;
    private readonly IModelListInterface _modelListService;

    private readonly Dictionary<string, Func<long, string[], Task>> _commands;

    public MessageProcessor(IChatProviderService telegram, CreatorController creatorController, IModelListInterface modelListService)
    {
        _telegram = telegram;
        _creatorController = creatorController;
        _modelListService = modelListService;
        _commands = new()
        {
            ["/providerlist"] = async (id, args) => await TelegramProviderListCommand(id, args),
            ["/changemodel"] = async (id, args) => await TelegramChangeModelCommand(id, args),
            ["/help"] = async (id, args) => await TelegramHelpCommand(id, args)
        };
    }//telegram and aiService services intances

    //makes the bridge between recieving the message, getting the aiService response, and sending the response to the client
    public async Task Process(Update update)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(update.Message?.Text))
                return;
            
            var isCommand = update.Message?.Entities?.FirstOrDefault(e => e.Type == MessageEntityType.BotCommand && e.Offset == 0);

            if (isCommand != null){
                await HandleCommand( update.Message!.Text.Split(' '), update.Message!.Chat.Id);
                return;
            }

            string providerName = _telegram.currentProvider;
            string modelName = _telegram.currentModel;
            Console.WriteLine($"MessageProcessor first");
            AiServiceInterface serviceProvider = _creatorController.FetchProvider(providerName);
            var message = update.Message!.Text;
            var chatId = update.Message!.Chat.Id;

            var resp = await serviceProvider.GenerateAiResponse(message, modelName);

            Console.WriteLine($"MessageProcessor Process resp: {resp}");

            await _telegram.SendMessage(chatId, resp);    
        }
        catch (Exception e)
        {
            Console.WriteLine($"MessageProcessor Process error: {e}");
            throw;
        }
    }

    public async Task HandleCommand(string[] args, long id)
    {
        _commands.TryGetValue(args[0], out var command);
        if (command == null)
        {
            await _telegram.SendMessage(id,  "Comando invalido");    
            return;
        }

        await command(id, args);

    }

    async Task TelegramProviderListCommand(long chatId, string[] args)
    {
        Console.WriteLine($"MessageProcessor TelegramProviderListCommand args: {string.Join(", ", args)}");
        var providerModelList = await _modelListService.ReturnAllModelsList();
        string modelList = "";
        foreach (var provider in providerModelList)
        {
            modelList += $"--- Provedor: {provider.Key}\n ---";
            foreach (var model in provider.Value)
            {
                Console.WriteLine($"MessageProcessor TelegramProviderListCommand provider: {provider.Key}, model: {model}");
                modelList += $"{provider.Key} {model}\n";
            }
        }
        Console.WriteLine($"MessageProcessor TelegramProviderListCommand list: {modelList}");
        await _telegram.SendMessage(id: chatId, text: modelList);
    }

    async Task TelegramChangeModelCommand(long chatId, string[] args)
    {
        if (args.Length != 3)
        {
            await _telegram.SendMessage(id: chatId, text: "Uso correto do comando: /changemodel <modelo> <provedor>");
            return;
        }
        
        _telegram.currentModel = args[2];
        _telegram.currentProvider = args[1];
        await _telegram.SendMessage(id: chatId, text: $"Modelo alterado para -> {_telegram.currentModel}\n do provedor -> {_telegram.currentProvider}");
    }

    async Task TelegramHelpCommand(long chatId, string[] args)
    {   
        List<string> commands = new List<string>(this._commands.Keys);
        await _telegram.SendMessage(id: chatId, text: $"Comandos disponiveis:\n{string.Join("\n", commands)}");
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