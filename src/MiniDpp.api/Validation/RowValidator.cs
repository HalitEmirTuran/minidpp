using System.Globalization;

namespace MiniDpp.api.Validation;

public record ProductRow(
    string? Gtin,
    string? ModelName,
    string? LotNum, 
    string? CottonPercent,
    string? ElastanePercent,
    string? ProductionPlace,
    string? CarbonKG
);

public static class RowValidator
{
    public static List<string> Validate(ProductRow row){
        var errors = new List<string>();

        var gtinError = GTINvalidator.Validate(row.Gtin);
        if (gtinError is not null)
        errors.Add(gtinError);

        if (string.IsNullOrWhiteSpace(row.ModelName))
        errors.Add("Model adi bos.");

        if (string.IsNullOrWhiteSpace(row.LotNum))
        errors.Add("Batch numarasi bos");

        if (string.IsNullOrWhiteSpace(row.ProductionPlace))
        errors.Add("Uretim Yeri Bos.");

        var cotton = ParsePercent(row.CottonPercent, "elyaf_pamuk_yuzde", errors);
        var elastane = ParsePercent(row.ElastanePercent, "elyaf_elastan_yuzde", errors);
        if (cotton is not null && elastane is not null && cotton + elastane != 100)
        errors.Add($"Elyaf toplamı %100 olmalı (pamuk {cotton} + elastan {elastane} = {cotton + elastane}).");

        if(!TryParseDecimal(row.CarbonKG, out var carbon))
        errors.Add($"karbon_kg_co2e sayisal degil");
        else if(carbon < 0)
        errors.Add($"karbon_kg_co2e negatif olamaz.(gelen: { carbon}).");

        return errors;

    }

    private static decimal? ParsePercent(string? value, string field, List<string> errors)
    {
        if (!TryParseDecimal(value, out var number))
        {
            errors.Add($"{field} sayisal olmak zorundadir. (gelen: '{value}')");
            return null;
        }

        if (number<0 || number > 100)
        {
            errors.Add($"{field} 0-100 arasinda olmali. (gelen: {number})");
            return null;
        }
        return number;
    }

    public static bool TryParseDecimal(string? value, out decimal number) =>
    decimal.TryParse(
        value?.Trim(),
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
        CultureInfo.InvariantCulture,
        out number);
}