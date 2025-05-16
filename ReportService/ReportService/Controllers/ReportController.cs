using Microsoft.AspNetCore.Mvc;
using ReportService.Services;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [Route("{year:int}/{month:int}")]
        public async Task<IActionResult> Download(int year, int month, CancellationToken cancellationToken)
        {
            var reportResult = await _reportService.GenerateReportAsync(year, month, cancellationToken);

            return Ok(reportResult.ReadyReport);
        }
    }
}