using FluentResults;
using ReportService.Errors;

namespace ReportService.Services;

public class EmployeeCodeProvider(HttpClient client) : IEmployeeCodeProvider
{
    public async Task<Result<string>> GetCode(string inn, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/inn/" + inn);
        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(new ExternalCallError("Status code doesn't indicate success: " + (int)response.StatusCode));
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return Result.Ok(content);
    }
}