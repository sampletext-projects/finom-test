namespace ReportService.Projections;

public class EmployeeReportProjection
{
    public string Name { get; set; }

    public long DepartmentId { get; set; }
    
    public string DepartmentName { get; set; }

    public string Inn { get; set; }
}