using ApiTier7Provision.Application;
using ApiTier7Provision.Infrastructure;
using ApiTier7Provision.Infrastructure.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var modelTemplateSeeder = scope.ServiceProvider.GetRequiredService<ModelTemplateSeeder>();
    await modelTemplateSeeder.SeedAsync(CancellationToken.None);
}


app.Run();
