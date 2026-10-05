using jsonToGemin.Interfaces;
using jsonToGemin.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IChatProviderService, TelegramService>();//one instance for every requisition
// builder.Services.AddSingleton<AiService, GeminiService>();
builder.Services.AddSingleton<CreatorController>();
builder.Services.AddScoped<IMessageProcessorInterface, MessageProcessor>();//instance is created on a requisition basis, if the same requisition is made there will be no new instance
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("Angular");


app.Run();

