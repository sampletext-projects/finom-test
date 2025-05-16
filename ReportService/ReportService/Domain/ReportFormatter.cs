namespace ReportService.Domain;

public static class ReportFormatter
{
    public static Action<Employee, Report> NL = (e, s) => s.S = s.S + Environment.NewLine;
    public static Action<Employee, Report> WL = (e, s) => s.S = s.S + "--------------------------------------------";
    public static Action<Employee, Report> WT = (e, s) => s.S = s.S + "         ";
    public static Action<Employee, Report> WE = (e, s) => s.S = s.S + e.Name;
    public static Action<Employee, Report> WS = (e, s) => s.S = s.S + e.Salary + "р";
    public static Action<Employee, Report> WD = (e, s) => s.S = s.S + e.Department;
}