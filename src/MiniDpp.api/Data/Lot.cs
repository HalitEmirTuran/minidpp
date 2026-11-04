namespace MiniDpp.api.Data;

public class Lot{
    public int ID{get; set;}
    public string LotNum{get; set;} = string.Empty;
    public string ProductionPlace {get; set;} = string.Empty;

    public decimal CarbonKG {get; set;}
    public int ProductModelId {get; set;}
    public ProductModel ProductModel {get; set; } = null!;
}


    
 

