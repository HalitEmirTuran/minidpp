using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniDpp.api.Data;
using MiniDpp.api.Import;

namespace MiniDpp.tests;

public class CsvImportServiceTests : IDisposable
{
    private const string Baslik = 
    "gtin,model_adi,batch_no,elyaf_pamuk_yuzde,elyaf_elastan_yuzde,uretim_yeri,karbon_kg_co2e";
    private readonly SqliteConnection _connection; 
    private readonly DppDbContext _db;
    private readonly CsvImportService _importer;
    public CsvImportServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<DppDbContext>()
        .UseSqlite(_connection)
        .Options;
        _db = new DppDbContext(options);
        _db.Database.EnsureCreated();
        _importer = new CsvImportService(_db);

    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private static Stream OrnekCsv() => File.OpenRead(Path.Combine(AppContext.BaseDirectory, "urunler_duzeltilmis_test.csv"));
    private static Stream MetindenCsv(string icerik) => new MemoryStream(Encoding.UTF8.GetBytes(icerik));

    [Fact]
    public async Task Ornek_CSV_3_gecerli_satiri_kaydeder()
    {
        var sonuc = await _importer.ImportAsync(OrnekCsv());
        Assert.Equal(3, sonuc.SavedCount);
        Assert.Equal(new[]{4,5}, sonuc.Errors.Select(e => e.Line));
        Assert.Equal(3, _db.ProductModels.Count());
        Assert.Equal(3, _db.Lots.Count());
    }

    [Fact]
    public async Task Ayni_Csv_ikikez()
    {
        await _importer.ImportAsync(OrnekCsv());
        await _importer.ImportAsync(OrnekCsv());

        Assert.Equal(3, _db.ProductModels.Count());
        Assert.Equal(3, _db.Lots.Count());

    }

    [Fact]
    public async Task ayni_gtinde_farkli_lot_ayni_modele()
    {
        var csv = Baslik + "\r\n"  + "08690000000012,Slim Denim 501,B1,98,2,Denizli,12.4\r\n" + "08690000000012,Slim Denim 501,B2,98,2,Bursa,11.9\r\n";
    
        var sonuc = await _importer.ImportAsync(MetindenCsv(csv));

        Assert.Equal(2, sonuc.SavedCount);
        var model = Assert.Single(_db.ProductModels.Include(p=>p.Lots));
        Assert.Equal(2, model.Lots.Count);
    }

}