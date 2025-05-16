using ReportService.Services;

namespace ReportService.Report;

public class MonthHeaderReportRow(string monthYearHeader) : IReportRow
{
    public string MonthYearHeader { get; } = monthYearHeader;

    public void Accept(ReportBuilderVisitor visitor)
    {
        visitor.Accept(this);
    }
}
