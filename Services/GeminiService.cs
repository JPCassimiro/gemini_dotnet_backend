using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Google.GenAI;
using Google.GenAI.Types;
using jsonToGemin.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace jsonToGemin.Services;

public class GeminiService : AiServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string apiKey = DotNetEnv.Env.GetString("GEMINI_API");
    private readonly Client _client;//gemini client
    protected override string providerName => "gemini";
    protected override string listModelEndpoit => "https://generativelanguage.googleapis.com/v1beta/models";

    public GeminiService(IHttpClientFactory httpClientFactory)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Unable to obtain GEMINI_API");
        }

        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _client = new Client(apiKey: apiKey);//instance gemini client
        
        Console.WriteLine($"GeminiService Consctructor, generating object: {RuntimeHelpers.GetHashCode(this)}");

    }

    //will send the reciedved message to gemini
    public override async Task<string> GenerateAiResponse(string q, string model)
    {
        try
        {
            Console.WriteLine($"GeminiService SendToGemini({RuntimeHelpers.GetHashCode(this)}) q: {q}");

            var clientResp = await _client.Models.GenerateContentAsync(model: model, contents: "Do not use Markdown. Do not use bold, italics, headers, lists, tables, or code fences. Return plain text only." + $"input: {q}");

            //if there is any text on the variable, return it 
            return clientResp.Candidates?
                    .FirstOrDefault()?
                    .Content?
                    .Parts?
                    .FirstOrDefault()?
                    .Text
                    ?? throw new Exception("No text returned from Gemini.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"GeminiService SendToGemini error: {e}");
            throw;
        }
    }

    public override async Task<List<string>> GetModelList()
    {
        try
        {
            // HttpClient _httpClient = _httpClientFactory.CreateClient();
            // using HttpResponseMessage resp = await _httpClient.GetAsync($"{listModelEndpoit}?key={apiKey}");
            // return await resp.Content.ReadAsStringAsync();
            List<string> modelList = [];
            var config = new ListModelsConfig { PageSize = 1000 };
            var modelListPager = await _client.Models.ListAsync(config);
            await foreach(var model in modelListPager)
            {
                modelList.Add(model.Name.Replace("models/",""));
            }
            return modelList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"GeminiService GetModelList error: {e}");
            throw;
        }
    }

}