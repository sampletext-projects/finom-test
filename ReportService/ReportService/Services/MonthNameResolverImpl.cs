using MonthNameResolver;

namespace ReportService.Services;

public class MonthNameResolverImpl : IMonthNameResolver
{
    public string GetName(int year, int month)
    {
        return MonthName.GetName(year, month);
    }
}
