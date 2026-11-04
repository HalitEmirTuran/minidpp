
namespace MiniDpp.api.Data;

public class ProductModel
{
    public int ID {get; set;}
    public string Gtin {get; set;} = string.Empty;
    public string Name {get; set;} = string.Empty;
    public decimal CottonPercent {get; set;}
    public decimal ElastanePercent {get; set;}
    public List<Lot> Lots {get; set;} = new();
}

