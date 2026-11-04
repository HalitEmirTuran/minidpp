using Microsoft.EntityFrameworkCore;
using MiniDpp.api.Data;
using MiniDpp.api.Import;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<CsvImportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db=scope.ServiceProvider.GetRequiredService<DppDbContext>();
    db.Database.Migrate();
}


app.MapGet("/health", () => Results.Ok(new {durum = "OK"}));

app.MapPost("/api/import", async (IFormFile file, CsvImportService importer)=>
{
    try
    {
        await using var stream = file.OpenReadStream();
        var result = await importer.ImportAsync(stream);
        return Results.Ok(result);

    }

    catch (InvalidDataException ex)
    {
        return Results.BadRequest(new{ hata = ex.Message});
    }
}).DisableAntiforgery();



app.Run();
