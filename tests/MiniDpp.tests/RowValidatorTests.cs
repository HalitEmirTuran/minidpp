using System.Globalization;
using MiniDpp.api.Validation;

namespace MiniDpp.tests;

public class RowValidatorTests
{
    private static ProductRow GecerliSatir() => new(
    "08690000000012", "Slim Denim 501", "B2027-01", "98", "2", "Denizli", "12.4");

    [Fact]
    public void Gecerli_satir_hatasiz()
    {
        var hatalar = RowValidator.Validate(GecerliSatir());
        Assert.Empty(hatalar);
    }

    [Fact]
    public void Bos_GTIN_reddedilir_CSV_satir_4()
    {
        var satir = GecerliSatir() with {Gtin = ""};
        var hatalar = RowValidator.Validate(satir);
        Assert.Single(hatalar);
    }

    [Fact]
    public void Elyaf_toplami_100_degilse_red_CSV_satir_5()
    {
        var satir = GecerliSatir() with {CottonPercent = "95", ElastanePercent = "3"};
        var hatalar = RowValidator.Validate(satir);
        Assert.Single(hatalar);
        Assert.Contains("toplam", hatalar[0]);
    }

    [Theory]
    [InlineData("101", "0")]
    [InlineData("-1", "1")]
    [InlineData("abc", "2")]
    public void Yuzde_Sayisal_veya_0_100_araliginda_degil(string pamuk, string elastan)
    {
        var satir = GecerliSatir() with { CottonPercent = pamuk, ElastanePercent=elastan };
        var hatalar = RowValidator.Validate(satir);
        Assert.NotEmpty(hatalar);
    }

    [Fact]
    public void Bos_Zorunlu_Alanlari_raporla()
    {
        var satir = GecerliSatir() with { ModelName = "", LotNum = "  ", ProductionPlace= null};
        var hatalar = RowValidator.Validate(satir);
        Assert.Equal(3, hatalar.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("-0.5")]
    [InlineData("12,4")]
    public void Gecersiz_Karbon_red(string karbon)
    {
        var satir = GecerliSatir() with { CarbonKG = karbon};
        var hatalar = RowValidator.Validate(satir);
        Assert.Single(hatalar);
    }

    [Fact]
    public void Turkce_bilgisayar_ondaligi()
    {
        var onceki = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo ("tr-TR");
            var satir = GecerliSatir() with { CottonPercent = "97.5", ElastanePercent = "2.5"};
            var hatalar = RowValidator.Validate(satir);
            Assert.Empty(hatalar);
        }
        finally
        {CultureInfo.CurrentCulture = onceki;}
        
    }

}