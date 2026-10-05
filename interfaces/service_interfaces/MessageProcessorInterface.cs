using Telegram.Bot.Types;
namespace jsonToGemin.Interfaces;
public interface IMessageProcessorInterface
{
    Task Process(Update update); 
    Task<string> AngProcess(string provider,string model, string message);
}