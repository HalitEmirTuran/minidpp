using MiniDpp.api.Validation;

namespace MiniDpp.tests;

public class GtinValidatorTests
{
    [Theory]
    [InlineData("08690000000012")]
    [InlineData("08690000000029")]
    [InlineData("08690000000043")]
    [InlineData("08690000000050")]
    public void Gecerli_GTIN_hatasiz(string gtin)
    {
        var hata = GTINvalidator.Validate(gtin);
        Assert.Null(hata);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BosGtin_hatali(string? gtin)
    {
        var hata = GTINvalidator.Validate(gtin);
        Assert.NotNull(hata);
    }

    [Theory]
    [InlineData("8690000000012")]
    [InlineData("086900000000123")]
    [InlineData("0869000000001A")]
    public void GTINuzunluk_karakter_hatali(string gtin)
    {
        var hata = GTINvalidator.Validate(gtin);
        Assert.NotNull(hata);
    }

    [Fact]
    public void GTINkontrol_basamak_hatasi()
    {
        var hata = GTINvalidator.Validate("08690000000013");
        Assert.NotNull(hata);
        Assert.Contains("kontrol", hata);
    }
}