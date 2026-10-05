namespace jsonToGemin.Interfaces;
public interface IChatProviderService
{
    Task SendMessage(long id, string text);
}