namespace ServerLogAnalyzer;
public static class DateTimeUtils
{
    public static bool EqualsOnly(this DateTime date, DateOnly dateOnly)
    {
        return date.Year == dateOnly.Year
            && date.Month == dateOnly.Month
            && date.Day == dateOnly.Day;
    }
}
