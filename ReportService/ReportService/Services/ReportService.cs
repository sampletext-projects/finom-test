using FluentResults;
using ReportService.Report;
using ReportService.Repositories;

namespace ReportService.Services;

public class ReportService(
    IEmployeeCodeProvider employeeCodeProvider,
    IEmployeeSalaryProvider employeeSalaryProvider,
    IEmployeeRepository employeeRepository,
    IMonthNameResolver monthNameResolver,
    ILogger<ReportService> logger
) : IReportService
{
    public async Task<Result<string>> GenerateReportAsync(int year, int month, CancellationToken cancellationToken)
    {
        long companyTotal = 0;
        var allRows = new List<IReportRow>();

        var monthYearHeader = monthNameResolver.GetName(year, month);
        allRows.Add(new MonthHeaderReportRow(monthYearHeader));

        var departmentEmployeesMap = await employeeRepository.GetEmployeesForAllActiveDepartmentsAsync(cancellationToken);

        foreach (var departmentGroup in departmentEmployeesMap)
        {
            // lookup guarantees that at least one employee exists in the group
            var departmentName = departmentGroup.First()
                .DepartmentName;
            allRows.Add(new DepartmentNameReportRow(departmentName));
            long departmentTotal = 0;

            foreach (var employee in departmentGroup)
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