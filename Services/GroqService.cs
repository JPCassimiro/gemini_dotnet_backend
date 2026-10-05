using jsonToGemin.Interfaces;
using Groq;
using Namotion.Reflection;
using GenerativeAI.Types;

namespace jsonToGemin.Services;

public class GroqService : AiServiceBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    protected override string providerName => "groq";
    private readonly string apiKey = DotNetEnv.Env.GetString("GROQ_API");
    protected override string listModelEndpoit => $"https://api.groq.com/openai/v1/models";
    private readonly GroqClient _client;

    public GroqService(IHttpClientFactory httpClientFactory)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Unable to obtain GROQ_API");
        }

        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",apiKey);
        _client = new GroqClient(apiKey);
    }

    public override async Task<string> GenerateAiResponse(string q, string model)
    {
        //use http client with api key on header and message on json body

        // try
        // {
        //     var requestBody = new
        //     {
        //         messages = new[]
        //         {
        //             new { role = "user", content= q}
        //         },
        //         model = model
        //     };

        //     var resp = await _httpClient.PostAsJsonAsync(requestUri:model, requestBody);
        //     resp.EnsureSuccessStatusCode();

        //     return await resp.Content.ReadAsStringAsync();
        // } catch(Exception e)
        // {
        //     Console.WriteLine($"GroqService GenerateAiResponse error: {e}");
        //     throw;
        // }
        try
        {
            Enum.TryParse<CreateChatCompletionRequestModel>(model, ignoreCase:true, out var desiredModel);

            IList<ChatCompletionRequestMessage> messages = [
                new ChatCompletionRequestUserMessage {
                    Role = ChatCompletionRequestUserMessageRole.User,
                    Content = "Do not use Markdown. Do not use bold, italics, headers, lists, tables, or code fences. Return plain text only." + $"input: {q}"
                }
            ];
            CreateChatCompletionRequest request = new()
            {
                Messages = messages,
                Model = desiredModel
            };
            var resp = await _client.Chat.CreateChatCompletionAsync(request);
            return resp.Choices[0].Message.Content;
        } catch(Exception e)
        {
            Console.WriteLine($"GroqService GenerateAiResponse error: {e}");
            throw;
        }
    }

    public override async Task<List<string>> GetModelList()
    {
        try
        {
            // List<string> result = [];
            // var resp = await _httpClient.GetAsync(listModelEndpoit);
            // resp.EnsureSuccessStatusCode();
            // var json = await resp.Content.ReadAsStringAsync();
            // Console.WriteLine($"GroqService GetModelList response: {json}");            

            // return result;

            List<string> result = [];


            foreach(var model in Enum.GetNames(typeof(CreateChatCompletionRequestModel)))
            {
                result.Add(model);
            }

            return result;

        } catch(Exception e)
        {
            Console.WriteLine($"GroqService GetModelList error: {e}");
            throw;
        }
    }
}