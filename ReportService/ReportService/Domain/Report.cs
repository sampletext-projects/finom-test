namespace ReportService.Domain
{
    public class Report
    {
        public string S { get; set; }
        public void Save()
        {
            System.IO.File.WriteAllText("D:\\report.txt", S);
        }
    }
}
