using Google.Apis.Http;
using jsonToGemin.Interfaces;
using jsonToGemin.Services;

// public struct providerInfo{
//     public string providerName;
//     public AiServiceInterface provider;
// }

abstract class AiCreator
{
    public abstract AiServiceInterface Factory();
}

public class CreatorController
{
    // private providerInfo[] providerArray = [];
    private readonly Dictionary<string, AiServiceInterface> providers = new();
    private readonly Dictionary<string, AiCreator> creators = new();

    public CreatorController(System.Net.Http.IHttpClientFactory httpClientFactory)
    {
        creators["gemini"] = new GeminiCreator(httpClientFactory);
        creators["groq"] = new GroqCreator(httpClientFactory);
        InitializeServiceProviders();
    }

    public AiServiceInterface FetchProvider(string p)
    {
        if (providers.TryGetValue(p, out var provider))
        {
            return provider;
        }

        creators.TryGetValue(p, out var creator);

        if (creator != null)
        {
            provider = creator.Factory();
            providers[p] = provider; 
            return provider;
        }
        else
        {
            return null;
        }
    }

    public void InitializeServiceProviders()
    {
        foreach(KeyValuePair<string, AiCreator> creator in creators)
        {
            string k = creator.Key;
            var v = creator.Value.Factory();
            providers[k] = v; 
        }
    }

    public List<string> ReturnProviderNameList()
    {
        List<string> names = [];
        foreach(KeyValuePair<string, AiCreator> creator in creators)
        {
            names.Add(creator.Key);
        }
        Console.WriteLine($"AiServiceFactory ReturnProviderNameList names: {names.Count}");
        return names;
    }

}

class GeminiCreator : AiCreator
{
    private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;

    public GeminiCreator(System.Net.Http.IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override AiServiceInterface Factory()
    {
        GeminiService gmService = new GeminiService(_httpClientFactory);
        return gmService;
    }
}

class GroqCreator : AiCreator
{
    private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;

    public GroqCreator(System.Net.Http.IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override AiServiceInterface Factory()
    {
        GroqService service = new GroqService(_httpClientFactory);
        return service;
    }
}