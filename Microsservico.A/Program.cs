using Microsservico.A;
using Microsservico.A.Services;
using Microsservico.A.Services.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings.RabbitMQ>(builder.Configuration.GetSection(AppSettings.RabbitMQ.Identifier));

builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddScoped<IBrokerService, RabbitMQPublisher>();



builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
	return Results.Ok(new GetResponse("Microsservico A"));
});

app.MapGet("/health", () =>
{
	return Results.Ok();
});

app.Run();
