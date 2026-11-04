using Microsoft.EntityFrameworkCore;
using MiniDpp.api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db=scope.ServiceProvider.GetRequiredService<DppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok(new {durum = "OK"}));

app.Run();
