using ResumeATS.Models.Requests;
using ResumeATS.Models.Responses;

namespace ResumeATS.Services.Interfaces;

/// <summary>
/// Contract for analyzing a resume against a job description using an AI backend.
/// </summary>
public interface IResumeAnalyzerService
{
    /// <summary>
    /// Analyses the given resume against the job description and returns a structured ATS report.
    /// </summary>
    /// <param name="request">The analysis request containing resume text and job description.</param>
    /// <returns>A <see cref="ResumeAnalysisResponse"/> with scores, keywords, and actionable feedback.</returns>
    Task<ResumeAnalysisResponse> AnalyzeAsync(
        ResumeAnalysisRequest request);
}
