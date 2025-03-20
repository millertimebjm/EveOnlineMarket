using Eve.Models.EveApi;

namespace Eve.Mvc.Services;

public static class ViewHelperService
{
    public static string MinimalHumanReadableTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan == TimeSpan.Zero)
            return "0s";

        if (timeSpan.Days > 0)
            return $"{timeSpan.Days}d";
        if (timeSpan.Hours > 0)
            return $"{timeSpan.Hours}h";
        if (timeSpan.Minutes > 0)
            return $"{timeSpan.Minutes}m";
        return $"{timeSpan.Seconds}s";
    }

    public static string HumanReadableTimeSpan(TimeSpan t)
    {
        if (t.TotalSeconds <= 1)
        {
            return $@"{t:s\.ff} seconds";
        }
        if (t.TotalMinutes <= 1)
        {
            return $@"{t:%s} seconds";
        }
        if (t.TotalHours <= 1)
        {
            return $@"{t:%m} minutes";
        }
        if (t.TotalDays <= 1)
        {
            return $@"{t:%h} hours";
        }

        return $@"{t:%d} days";
    }

    public static string EveReadableIskValue(decimal d)
    {
        if (d > 1000000000) // billion
        {
            return (d / 1000000000).ToString("F2") + "b";
        }
        if (d > 1000000) // million
        {
            return (d / 1000000).ToString("F2") + "m";
        }
        if (d > 1000) // thousand
        {
            return (d / 1000).ToString("F2") + "k";
        }
        return d.ToString("00");
    }

    public static decimal ConvertTypeListToVolume(Dictionary<int, int> typeQuantities, List<EveType> eveTypes)
    {
        var result = 0m;
        foreach (var typeQuantity in typeQuantities)
        {
            var eveType = eveTypes.Single(et => et.TypeId == typeQuantity.Key);
            result += eveType.Volume * typeQuantity.Value;
        }
        return result;
    }
}