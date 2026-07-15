using Autofac;
using Autofac.Extensions.DependencyInjection;
using WorldRank.Api;
using WorldRank.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(
    new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(
    containerBuilder =>
    {
        containerBuilder.RegisterModule<ApiModule>();
        containerBuilder.RegisterModule<InfrastructureModule>();
    });

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