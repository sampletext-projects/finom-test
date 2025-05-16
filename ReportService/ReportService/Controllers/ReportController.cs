using Microsoft.AspNetCore.Mvc;
using ReportService.Services;

namespace ReportService.Controllers;

[Route("api/[controller]")]
public class ReportController(IReportService reportService) : Controller
{
    [HttpGet]
    [Route("{year:int}/{month:int}")]
    public async Task<IActionResult> Download(int year, int month, CancellationToken cancellationToken)
    {
        var reportResult = await reportService.GenerateReportAsync(year, month, cancellationToken);

        return Ok(reportResult.ReadyReport);
    }
}