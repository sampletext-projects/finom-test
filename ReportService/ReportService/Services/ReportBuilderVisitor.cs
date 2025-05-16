using System.Text;
using ReportService.Report;

namespace ReportService.Services;

public class ReportBuilderVisitor
{
    private readonly StringBuilder _stringBuilder = new();

    public void Visit(IReportRow row)
    {
        row.Accept(this);
    }

    public void Accept(MonthHeaderReportRow row)
    {
        _stringBuilder.AppendLine(row.MonthYearHeader);
        _stringBuilder.AppendLine();
    }

    public void Accept(EmployeeReportRow row)
    {
        _stringBuilder.AppendLine($"{row.Name,-40} {row.Salary}р");
        _stringBuilder.AppendLine();
    }

    public void Accept(DepartmentNameReportRow row)
    {
        _stringBuilder.AppendLine(row.Name);
        _stringBuilder.AppendLine();
    }

    public void Accept(DepartmentTotalReportRow row)
    {
        _stringBuilder.AppendLine($"Всего по отделу\t\t{row.Total}р");
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine("---");
    }

    public void Accept(CompanyTotalReportRow row)
    {
        _stringBuilder.AppendLine($"Всего по предприятию\t\t{row.Total}р");
        _stringBuilder.AppendLine();
    }

    public string GetReport()
    {
        return _stringBuilder.ToString();
    }
}