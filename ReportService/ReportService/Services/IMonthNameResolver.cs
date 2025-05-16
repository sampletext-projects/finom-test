namespace ReportService.Services;

public interface IMonthNameResolver
{
    string GetName(int year, int month);
}
