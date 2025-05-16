using ReportService.Services;

namespace ReportService.Report;

public interface IReportRow
{
    public void Accept(ReportBuilderVisitor visitor);
}