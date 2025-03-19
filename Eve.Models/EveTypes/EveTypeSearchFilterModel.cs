namespace Eve.Models.EveTypes;

public class EveTypeSearchFilterModel
{
    public HashSet<int>? Ids { get; set; }
    public bool IsMarketableType { get; set; } = true;
    public string? Keyword { get; set; }
    public int _take = 20;
    public int Take { 
        get { return _take; }
        set { _take = value > 100 ? 100 : _take < 1 ? 1 : value; }
    }
    public int _skip = 0;
    public int Skip 
    {
        get { return _skip; }
        set { _skip = value < 0 ? 0 : value; }
    }
}