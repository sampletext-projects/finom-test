using ReportService.Services;

namespace ReportService.Dtos;

public class EmployeeReportRow(string name, int salary) : IReportRow
{
    public string Name { get; set; } = name;
    public int Salary { get; set; } = salary;

    public void Accept(ReportVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public interface IReportRow
{
    public void Accept(ReportVisitor visitor);
}