using FluentResults;

namespace ReportService.Errors;

public class ExternalCallError(string error) : Error(error);