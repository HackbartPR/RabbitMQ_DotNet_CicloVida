using Microsservico.B;

var builder = WebApplication.CreateBuilder(args);

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
