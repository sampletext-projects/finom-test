namespace ReportService.Domain;

public class Report
{
    public string S { get; set; }

    public string GetReport()
    {
        return S;
    }
}