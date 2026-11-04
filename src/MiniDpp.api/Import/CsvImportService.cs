using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using MiniDpp.api.Data;
using MiniDpp.api.Validation;

namespace MiniDpp.api.Import;

public record RowError(int Line, string? Gtin, List<string> Errors);
public record ImportResult (int SavedCount, List<RowError> Errors);

public class CsvImportService
{
    private static readonly string[] RequiredHeaders = [
        "gtin", "model_adi", "batch_no", "elyaf_pamuk_yuzde", "elyaf_elastan_yuzde", "uretim_yeri", "karbon_kg_co2e"
    ];

    private readonly DppDbContext _db;
    public CsvImportService(DppDbContext db)
    {
        _db = db;
    }

    public async Task<ImportResult> ImportAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions= TrimOptions.Trim
        };
        
        using var csv = new CsvReader(reader, config);

        if (!await csv.ReadAsync())
        throw new InvalidDataException("CSV dosyasi bos");

        csv.ReadHeader();
        var missing = RequiredHeaders.Where( h=> !csv.HeaderRecord!.Contains(h)).ToList();
        if (missing.Count > 0)
        throw new InvalidDataException($"CSV basliginda eksik sutun veya sutunlar: {string.Join(", ", missing)}");
        var saved = 0;
        var errors = new List<RowError>();
        while (await csv.ReadAsync())
        {
            var line = csv.Parser.Row;
            var row = new ProductRow(
                csv.GetField("gtin"),
                csv.GetField("model_adi"),
                csv.GetField("batch_no"),
                csv.GetField("elyaf_pamuk_yuzde"),
                csv.GetField("elyaf_elastan_yuzde"),
                csv.GetField("uretim_yeri"),
                csv.GetField("karbon_kg_co2e"));
            var rowErrors = RowValidator.Validate(row);
            if (rowErrors.Count > 0)
            {
                errors.Add(new RowError (line, row.Gtin, rowErrors));
                continue;
            }

            await SaveAsync(row);
            saved++;
        }
        return new ImportResult(saved, errors);
    }
    private async Task SaveAsync(ProductRow row)
    {
        var model = await _db.ProductModels
        .Include(p => p.Lots)
        .FirstOrDefaultAsync(p => p.Gtin == row.Gtin);
        if (model is null)
        {
            model = new ProductModel{Gtin=row.Gtin!};
            _db.ProductModels.Add(model);
        }

        RowValidator.TryParseDecimal(row.CottonPercent, out var cotton);
        RowValidator.TryParseDecimal(row.ElastanePercent, out var elastane);
        model.Name = row.ModelName!;
        model.CottonPercent = cotton;
        model.ElastanePercent= elastane;

        var lot = model.Lots.FirstOrDefault(l => l.LotNum == row.LotNum);
        if (lot is null)
        {
            lot = new Lot {LotNum = row.LotNum! };
            model.Lots.Add(lot);
        }

    RowValidator.TryParseDecimal(row.CarbonKG, out var carbon);
    lot.ProductionPlace = row.ProductionPlace!;
    lot.CarbonKG= carbon;
    await _db.SaveChangesAsync();

    }
}