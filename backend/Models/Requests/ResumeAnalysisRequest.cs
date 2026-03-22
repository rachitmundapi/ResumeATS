using System.ComponentModel.DataAnnotations;

namespace ResumeATS.Models.Requests;

/// <summary>
/// Represents the payload sent by the client to analyse a resume against a job description.
/// </summary>
public sealed class ResumeAnalysisRequest
{
    /// <summary>Plain-text or raw content of the candidate's resume.</summary>
    [Required(ErrorMessage = "Resume text is required.")]
    [MinLength(50, ErrorMessage = "Resume text is too short to be meaningful.")]
    public string ResumeText { get; set; } = string.Empty;

    /// <summary>The job description the resume should be evaluated against.</summary>
    [Required(ErrorMessage = "Job description is required.")]
    [MinLength(30, ErrorMessage = "Job description is too short.")]
    public string JobDescription { get; set; } = string.Empty;

    /// <summary>
    /// Optional: override the model used for this specific request.
    /// Falls back to the value configured in appsettings.json when null or empty.
    /// </summary>
    public string? ModelOverride { get; set; }
}
