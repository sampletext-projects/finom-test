using ReportService.Services;

namespace ReportService.Report;

public class EmployeeReportRow(string name, int salary) : IReportRow
{
    public string Name { get; set; } = name;
    public int Salary { get; set; } = salary;

    public void Accept(ReportBuilderVisitor visitor)
    {
        visitor.Accept(this);
    }
}