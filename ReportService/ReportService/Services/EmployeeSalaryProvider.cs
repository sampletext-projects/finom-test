using FluentResults;
using ReportService.Errors;

namespace ReportService.Services;

public class EmployeeSalaryProvider(HttpClient client) : IEmployeeSalaryProvider
{
    public async Task<Result<int>> GetSalary(string inn, string buhCode, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/empcode/" + inn);
        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(new ExternalCallError("Status code doesn't indicate success: " + (int)response.StatusCode));
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!decimal.TryParse(content, out var salary))
        {
            return Result.Fail(new ExternalCallError("Response is malformed: " + content));
        }
        
        return Result.Ok((int)salary);
    }
}