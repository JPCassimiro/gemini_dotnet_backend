
using jsonToGemin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace jsonToGemin.Controllers;

[Route("api/telegram")]
[ApiController]

public class JsonToGemini: ControllerBase//router etc
{
    private readonly MessageProcessor _processor;//processor instance

    public JsonToGemini(MessageProcessor processor)
    {
        _processor = processor;
    }

    [HttpPost]//when a post requisition reaches this rout, we send the text to the processor
    public async Task<IActionResult> WebHook([FromBody] Update update)
    {
        Console.WriteLine($"JsonToGeminiController update.Id: {update.Id}");
        await _processor.Process(update);
        return Ok();
    }

    // [Route("api/telegram")] + [HttpPost("/ang")]
    [HttpPost("ang")]
    public async Task<IActionResult> SendMessageFromAng([FromBody] string message)
    {
        Console.WriteLine($"JsonToGeminiController SendMessageFromAng: {message}");
        var res = await _processor.AngProcess(message);
        Console.WriteLine($"JsonToGeminiController SendMessageFromAng res: {res}");
        // if (string.IsNullOrWhiteSpace(res))
        // {
        //     return Ok();
        // }
        
        return Ok(new
        {
            response = res
        });
    }

}