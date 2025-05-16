using ReportService.Services;

namespace ReportService.Dtos;

public class DepartmentTotalReportRow(long total) : IReportRow
{
    public long Total { get; set; } = total;

    public void Accept(ReportVisitor visitor)
    {
        visitor.Accept(this);
    }
}