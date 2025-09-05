using Microsservico.B;
using Microsservico.B.Listeners;
using Microsservico.B.Services;
using Microsservico.B.Services.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings.RabbitMQ>(builder.Configuration.GetSection(AppSettings.RabbitMQ.Identifier));

builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddSingleton<IBrokerListener, RabbitMQListener>();
builder.Services.AddHostedService<BrokerListener>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
	return Results.Ok(new GetResponse("Microsservico B"));
});

app.MapGet("/health", () =>
{
	return Results.Ok();
});

app.Run();
