using System.Text.Json;
using Google.GenAI;

namespace jsonToGemin.Services;

public class GeminiService
{

    private readonly Client _client;//gemini client

    public GeminiService(IConfiguration configuration)
    {
        _client = new Client(apiKey: "API_KEY");//instance gemini client
    }

    //will send the reciedved message to gemini
    public async Task<string> SendToGemini(string q)
    {
        try
        {
            Console.WriteLine($"GeminiService SendToGemini q: {q}");

            var clientResp = await _client.Models.GenerateContentAsync(model: "gemini-3.1-flash-lite", contents: "Do not use Markdown. Do not use bold, italics, headers, lists, tables, or code fences. Return plain text only." + $"input: {q}");

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
}