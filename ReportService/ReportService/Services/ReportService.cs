using FluentResults;
using Npgsql;
using ReportService.Domain;
using ReportService.Dtos;
using ReportService.Repositories;

namespace ReportService.Services;

public class ReportService : IReportService
{
    private readonly IEmployeeCodeProvider _employeeCodeProvider;
    private readonly IEmployeeSalaryProvider _employeeSalaryProvider;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IEmployeeCodeProvider employeeCodeProvider,
        IEmployeeSalaryProvider employeeSalaryProvider,
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository, 
        ILogger<ReportService> logger)
    {
        _employeeCodeProvider = employeeCodeProvider;
        _employeeSalaryProvider = employeeSalaryProvider;
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<Result<string>> GenerateReportAsync(int year, int month, CancellationToken cancellationToken)
    {
        var actions = new List<(Action<Employee, Report>, Employee)>();
        var report = new Report() {S = MonthNameResolver.MonthName.GetName(year, month)};

        var departments = await _departmentRepository.GetActiveDepartments(cancellationToken);
        
        var reportVisitor = new ReportVisitor();
        long companyTotal = 0;
        var allRows = new List<IReportRow>();
        
        // Add the month and year header
        // Note: This should be handled by a separate header row class in a proper implementation
        
        foreach (var department in departments)
        {
            var departmentEmployees = await _employeeRepository.GetEmployeesByDepartmentIdForReportAsync(department.Id, cancellationToken);
            
            allRows.Add(new DepartmentNameReportRow(department.Name));
            long departmentTotal = 0;
            
            foreach (var employee in departmentEmployees)
            {
                var employeeCodeResult = await _employeeCodeProvider.GetCode(employee.Inn, cancellationToken);

                if (employeeCodeResult.IsFailed)
                {
                    _logger.LogWarning("Failed to get employee code for {inn}: {error}", employee.Inn, employeeCodeResult.StringifyErrors());
                    return Result.Fail("Failed to get employee code for " + employee.Inn);
                }

                var buhCode = employeeCodeResult.Value;

                var salaryResult = await _employeeSalaryProvider.GetSalary(employee.Inn, buhCode, cancellationToken);

                if (salaryResult.IsFailed)
                {
                    _logger.LogWarning("Failed to get employee salary for {inn}: {error}", employee.Inn, salaryResult.StringifyErrors());
                    return Result.Fail("Failed to get employee salary for " + employee.Inn);
                }
                
                allRows.Add(new EmployeeReportRow(employee.Name, salaryResult.Value));
                departmentTotal += salaryResult.Value;
            }
            
            allRows.Add(new DepartmentTotalReportRow(departmentTotal));
            companyTotal += departmentTotal;
        }
        
        // Add company total row
        allRows.Add(new CompanyTotalReportRow(companyTotal));
        
        // Generate the report using the visitor
        foreach (var row in allRows)
        {
            reportVisitor.Visit(row);
        }
        
        var readyReport = reportVisitor.GetReport();

        return Result.Ok(readyReport);
    }
}