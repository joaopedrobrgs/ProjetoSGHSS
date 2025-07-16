using SGHSS_Backend.Models.Exceptions;
using static SGHSS_Backend.Models.Enums;

namespace SGHSS_Backend.Models;

public class ApiResponse
{
    public int? Code { get; set; }
    public StatusEnum? Status { get; set; }
}

public class CompletedResponse : ApiResponse
{
    public dynamic? Data { get; set; }
    public bool ShowMessage { get; set; } = false;
}

public class ErrorResponse : ApiResponse
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public bool ShowMessage { get; set; } = false;
}
