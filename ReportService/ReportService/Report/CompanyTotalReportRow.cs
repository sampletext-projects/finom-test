using ReportService.Services;

namespace ReportService.Report;

public class CompanyTotalReportRow(long total) : IReportRow
{
    public long Total { get; set; } = total;

    public void Accept(ReportBuilderVisitor visitor)
    {
        visitor.Accept(this);
    }
}
