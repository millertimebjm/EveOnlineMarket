namespace Eve.Models;

public class TypeSearchFilterModel
{
    public string Keyword { get; set; } = "";
    public int Skip { get; set; } = 0;
    private const int MAX_TAKE = 100;
    private int _take = MAX_TAKE;
    public int Take
    {
        get => _take;
        set
        {
            if (_take < 1) _take = MAX_TAKE;
            if (_take > 100) _take = MAX_TAKE;
            _take = value;
        }
    }
}