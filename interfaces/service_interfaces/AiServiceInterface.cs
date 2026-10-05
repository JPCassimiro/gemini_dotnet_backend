namespace jsonToGemin.Interfaces;

public interface AiServiceInterface
{
    Task<string> GenerateAiResponse(string q, string model);
    string GetProviderName();
    public Task<List<string>> GetModelList();
}

public abstract class AiServiceBase : AiServiceInterface
{
    protected abstract string providerName {get;}
    protected abstract string listModelEndpoit {get;}

    public abstract Task<string> GenerateAiResponse(string q, string model);

    public string GetProviderName()
    {
        return providerName;
    }

    public abstract Task<List<string>> GetModelList();

}