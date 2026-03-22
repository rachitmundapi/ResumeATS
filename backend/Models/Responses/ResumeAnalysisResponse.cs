namespace ResumeATS.Models.Responses;

/// <summary>
/// The structured result returned to the client after ATS analysis.
/// </summary>
public sealed class ResumeAnalysisResponse
{
    /// <summary>Overall ATS compatibility score out of 100.</summary>
    public int AtsScore { get; set; }

    /// <summary>High-level summary of the match quality.</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>Keywords found in both the resume and job description.</summary>
    public List<string> MatchedKeywords { get; set; } = [];

    /// <summary>Important keywords from the job description that are missing from the resume.</summary>
    public List<string> MissingKeywords { get; set; } = [];

    /// <summary>Concrete improvements the candidate can make to increase their score.</summary>
    public List<string> Suggestions { get; set; } = [];

    /// <summary>Section-by-section breakdown of the resume (e.g., "Experience", "Skills").</summary>
    public List<SectionFeedback> SectionBreakdown { get; set; } = [];

    /// <summary>The model that produced this analysis.</summary>
    public string ModelUsed { get; set; } = string.Empty;

    /// <summary>UTC timestamp of when the analysis was performed.</summary>
    public DateTime AnalysedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Feedback for a specific section of the resume.</summary>
public sealed class SectionFeedback
{
    public string Section { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public int Score { get; set; }
}
