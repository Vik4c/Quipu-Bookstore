using Quipu.Bookstore.Api.Authentication;
using Quipu.Bookstore.Api.Authentication.Seeding;
using Quipu.Bookstore.Api.Registers;
using Quipu.Bookstore.Application;
using Quipu.Bookstore.Storage;
using Quipu.Bookstore.Storage.Context;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .RegisterApplication()
    .RegisterStorage(builder.Configuration)
    .RegisterOpenIddict(builder.Environment)
    .RegisterAuthentication(builder.Configuration)
    .RegisterAuthorization()
    .RegisterControllers()
    .RegisterSwagger();

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();

    await app.Services.InitializeDatabaseAsync();
    await app.Services.SeedDevelopmentClientsAsync();
}

bool isRunningInContainer = string.Equals(
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
    "true",
    StringComparison.OrdinalIgnoreCase
);

// Container images listen on HTTP only, so there is no HTTPS endpoint to redirect to.
if (!isRunningInContainer)
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapAuthenticationEndpoints();

app.Run();
