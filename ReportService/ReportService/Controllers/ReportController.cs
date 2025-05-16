using Microsoft.AspNetCore.Mvc;
using ReportService.Dtos;
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

        if (reportResult.IsFailed)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(reportResult.StringifyErrors()));
        }

        return Ok(reportResult.Value);
    }
}