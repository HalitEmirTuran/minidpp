namespace MiniDpp.api.Validation;

public static class GTINvalidator
{
    public static string? Validate(string? gtin)
    {
        if( string.IsNullOrWhiteSpace(gtin))
        return "GTIN bos.";

        if(gtin.Length !=14 || !gtin.All(char.IsAsciiDigit))
        return "GTIN 14 haneyi saglamiyor veya sayisal degil.";

        var sum= 0; 
        for (var i = 0; i<13; i++)
        {
            var digit=gtin[i] - '0';
            var weight = i %2 == 0 ? 3 : 1;
            sum+= digit*weight;
        }

        var expected = (10 - sum % 10) %10;
        var actual = gtin[13] - '0';

        if (expected != actual)
        return $"GTIN kontrol basamagi yanlis. (olmasi gereken {expected}, gelen: {actual}).";

        return null;
    }
}