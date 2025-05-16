using System.Text;
using ReportService.Dtos;

namespace ReportService.Services;

public class ReportVisitor
{
    private readonly StringBuilder _stringBuilder = new();
    
    public void Visit(IReportRow row)
    {
        row.Accept(this);
    }

    public void Accept(EmployeeReportRow row)
    {
        // Format: Employee Name [tab space alignment] Salary + 'р'
        _stringBuilder.AppendLine($"{row.Name,-40} {row.Salary}р");
        _stringBuilder.AppendLine(); // Empty line after each employee
    }

    public void Accept(DepartmentNameReportRow row)
    {
        // Format: Department name followed by empty line
        _stringBuilder.AppendLine(row.Name);
        _stringBuilder.AppendLine(); // Empty line after department name
    }

    public void Accept(DepartmentTotalReportRow row)
    {
        // Format: "Всего по отделу" [tab space alignment] Total + 'р'
        _stringBuilder.AppendLine($"Всего по отделу{new string(' ', 30)}{row.Total}р");
        _stringBuilder.AppendLine(); // Empty line after department total
        _stringBuilder.AppendLine("---"); // Separator line after department total
    }
    
    public void Accept(CompanyTotalReportRow row)
    {
        // Format: "Всего по предприятию" [tab space alignment] Total + 'р'
        _stringBuilder.AppendLine($"Всего по предприятию{new string(' ', 30)}{row.Total}р");
        _stringBuilder.AppendLine();
    }
    
    public string GetReport()
    {
        return _stringBuilder.ToString();
    }
}