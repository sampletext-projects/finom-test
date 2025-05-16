using FluentResults;
using ReportService.Report;
using ReportService.Repositories;

namespace ReportService.Services;

public class ReportService(
    IEmployeeCodeProvider employeeCodeProvider,
    IEmployeeSalaryProvider employeeSalaryProvider,
    IDepartmentRepository departmentRepository,
    IEmployeeRepository employeeRepository,
    IMonthNameResolver monthNameResolver,
    ILogger<ReportService> logger
) : IReportService
{
    public async Task<Result<string>> GenerateReportAsync(int year, int month, CancellationToken cancellationToken)
    {
        long companyTotal = 0;
        var allRows = new List<IReportRow>();

        string monthYearHeader = monthNameResolver.GetName(year, month);
        allRows.Add(new MonthHeaderReportRow(monthYearHeader));

        var departments = await departmentRepository.GetActiveDepartments(cancellationToken);

        foreach (var department in departments)
        {
            var departmentEmployees = await employeeRepository.GetEmployeesByDepartmentIdForReportAsync(department.Id, cancellationToken);

            allRows.Add(new DepartmentNameReportRow(department.Name));
            long departmentTotal = 0;

            foreach (var employee in departmentEmployees)
            {
                var employeeCodeResult = await employeeCodeProvider.GetCode(employee.Inn, cancellationToken);

                if (employeeCodeResult.IsFailed)
                {
                    logger.LogWarning("Failed to get employee code for {inn}: {error}", employee.Inn, employeeCodeResult.StringifyErrors());
                    return Result.Fail("Failed to get employee code for " + employee.Inn);
                }

                var buhCode = employeeCodeResult.Value;

                var salaryResult = await employeeSalaryProvider.GetSalary(employee.Inn, buhCode, cancellationToken);

                if (salaryResult.IsFailed)
                {
                    logger.LogWarning("Failed to get employee salary for {inn}: {error}", employee.Inn, salaryResult.StringifyErrors());
                    return Result.Fail("Failed to get employee salary for " + employee.Inn);
                }

                allRows.Add(new EmployeeReportRow(employee.Name, salaryResult.Value));
                departmentTotal += salaryResult.Value;
            }

            allRows.Add(new DepartmentTotalReportRow(departmentTotal));
            companyTotal += departmentTotal;
        }
        allRows.Add(new CompanyTotalReportRow(companyTotal));

        var reportVisitor = new ReportBuilderVisitor();
        foreach (var row in allRows)
        {
            reportVisitor.Visit(row);
        }

        var readyReport = reportVisitor.GetReport();

        return Result.Ok(readyReport);
    }
}