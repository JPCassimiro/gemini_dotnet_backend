namespace jsonToGemin.Interfaces;
public interface IChatProviderService
{
    string currentModel{get; set;}
    string currentProvider{get; set;}
    Task SendMessage(long id, string text);
}