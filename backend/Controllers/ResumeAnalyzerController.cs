using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ResumeATS.Models.Requests;
using ResumeATS.Models.Responses;
using ResumeATS.Services.Interfaces;

namespace ResumeATS.Controllers;

/// <summary>
/// Exposes the ATS resume analysis functionality as a REST API.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Produces(MediaTypeNames.Application.Json)]
public sealed class ResumeAnalyzerController : ControllerBase
{
    private readonly IResumeAnalyzerService _analyzerService;
    private readonly ILogger<ResumeAnalyzerController> _logger;

    public ResumeAnalyzerController(
        IResumeAnalyzerService analyzerService,
        ILogger<ResumeAnalyzerController> logger)
    {
        _analyzerService = analyzerService;
        _logger = logger;
    }

    /// <summary>
    /// Analyses a resume against a job description and returns an ATS compatibility report.
    /// </summary>
    /// <param name="request">The resume text and job description to evaluate.</param>
    /// <returns>A detailed <see cref="ResumeAnalysisResponse"/> report.</returns>
    /// <response code="200">Analysis completed successfully.</response>
    /// <response code="400">Request validation failed.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpPost]
    public async Task<IActionResult> Analyze(
        [FromBody] ResumeAnalysisRequest request)
    {
        _logger.LogInformation("Received analysis request.");
        var result = await _analyzerService.AnalyzeAsync(request);
        return Ok(result);
    }
}
