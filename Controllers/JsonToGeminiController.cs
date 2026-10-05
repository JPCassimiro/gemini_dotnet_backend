
using System.Reflection.Emit;
using jsonToGemin.Interfaces;
using jsonToGemin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using jsonToGemin.Models;

namespace jsonToGemin.Controllers;

[Route("api/telegram")]
[ApiController]

public class JsonToGemini: ControllerBase//router etc
{
    private readonly IMessageProcessorInterface _processor;//processor instance
    private readonly CreatorController _creatorController;//processor instance
    private readonly ModelListService _modelListService;

    public JsonToGemini(IMessageProcessorInterface processor, CreatorController creatorController)
    {
        _processor = processor;
        _creatorController = creatorController;
        _modelListService = new ModelListService(creatorController);
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
    public async Task<IActionResult> SendMessageFromAng([FromBody] FrontEndMessage message)
    {
        Console.WriteLine($"JsonToGeminiController SendMessageFromAng: {message}");
        var res = await _processor.AngProcess(message.Provider, message.Model, message.Message);
        Console.WriteLine($"JsonToGeminiController SendMessageFromAng res: {res}");
        
        return Ok(new
        {
            response = res
        });
    }

    [HttpGet("ang/modelList")]
    public async Task<IActionResult> GetAiModelsList()
    {
        Console.WriteLine($"JsonToGeminiController GetAiModelsList");
        Dictionary<string,List<string>> list = await _modelListService.ReturnAllModelsList();
        return Ok(list);
    }

}