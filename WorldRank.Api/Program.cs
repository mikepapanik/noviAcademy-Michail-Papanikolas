using WorldRank.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Logging);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet(
        "/",
        () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();