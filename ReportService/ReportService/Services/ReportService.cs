using FluentResults;
using Npgsql;
using ReportService.Domain;
using ReportService.Repositories;

namespace ReportService.Services;

public class ReportService : IReportService
{
    private readonly IEmployeeCodeProvider _employeeCodeProvider;
    private readonly IEmployeeSalaryProvider _employeeSalaryProvider;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IEmployeeCodeProvider employeeCodeProvider,
        IEmployeeSalaryProvider employeeSalaryProvider,
        IDepartmentRepository departmentRepository,
        ILogger<ReportService> logger
    )
    {
        _employeeCodeProvider = employeeCodeProvider;
        _employeeSalaryProvider = employeeSalaryProvider;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<Result<string>> GenerateReportAsync(int year, int month, CancellationToken cancellationToken)
    {
        var actions = new List<(Action<Employee, Report>, Employee)>();
        var report = new Report() {S = MonthNameResolver.MonthName.GetName(year, month)};
        var connString = "Host=192.168.99.100;Username=postgres;Password=1;Database=employee";

        var departments = await _departmentRepository.GetActiveDepartments(cancellationToken);
        
        foreach (var department in departments)
        {
            List<Employee> emplist = new List<Employee>();
            var conn1 = new NpgsqlConnection(connString);
            conn1.Open();
            var cmd1 = new NpgsqlCommand("SELECT e.name, e.inn, d.name from emps e left join deps d on e.departmentid = d.id", conn1);
            var reader1 = cmd1.ExecuteReader();
            while (reader1.Read())
            {
                var emp = new Employee() {Name = reader1.GetString(0), Inn = reader1.GetString(1), Department = reader1.GetString(2)};
                var employeeCodeResult = await _employeeCodeProvider.GetCode(emp.Inn, cancellationToken);

                if (employeeCodeResult.IsFailed)
                {
                    _logger.LogWarning("Failed to get employee code for {inn}: {error}", emp.Inn, employeeCodeResult.StringifyErrors());
                    return Result.Fail("Failed to get employee code for " + emp.Inn);
                }

                emp.BuhCode = employeeCodeResult.Value;

                var salaryResult = await _employeeSalaryProvider.GetSalary(emp.Inn, emp.BuhCode, cancellationToken);

                if (salaryResult.IsFailed)
                {
                    _logger.LogWarning("Failed to get employee salary for {inn}: {error}", emp.Inn, salaryResult.StringifyErrors());
                    return Result.Fail("Failed to get employee salary for " + emp.Inn);
                }

                emp.Salary = salaryResult.Value;
                if (emp.Department != department.Name)
                    continue;
                emplist.Add(emp);
            }

            actions.Add((ReportFormatter.NL, new Employee()));
            actions.Add((ReportFormatter.WL, new Employee()));
            actions.Add((ReportFormatter.NL, new Employee()));
            actions.Add((ReportFormatter.WD, new Employee() {Department = department.Name}));
            for (int i = 1; i < emplist.Count(); i++)
            {
                actions.Add((ReportFormatter.NL, emplist[i]));
                actions.Add((ReportFormatter.WE, emplist[i]));
                actions.Add((ReportFormatter.WT, emplist[i]));
                actions.Add((ReportFormatter.WS, emplist[i]));
            }
        }

        actions.Add((ReportFormatter.NL, null));
        actions.Add((ReportFormatter.WL, null));

        foreach (var act in actions)
        {
            act.Item1(act.Item2, report);
        }

        var readyReport = report.GetReport();

        return Result.Ok(readyReport);
    }
}