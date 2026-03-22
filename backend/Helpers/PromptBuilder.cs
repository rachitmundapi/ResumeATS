namespace ResumeATS.Helpers;

/// <summary>
/// Centralises all prompt construction logic so prompt text stays out of service classes.
/// </summary>
public static class PromptBuilder
{
    /// <summary>Returns the system-level instructions for ATS resume analysis.</summary>
    public static string BuildSystemPrompt() =>
        """
        You are an expert ATS (Applicant Tracking System) resume analyzer with deep knowledge of
        HR practices, technical recruiting, and resume optimization.

        Your task is to evaluate a candidate's resume against a specific job description and return
        a structured JSON analysis. Be objective, detailed, and actionable.

        IMPORTANT: You MUST respond with ONLY valid JSON — no markdown fences, no extra text.
        The JSON schema must match exactly:
        {
          "atsScore": <integer 0-100>,
          "summary": "<string>",
          "matchedKeywords": ["<string>", ...],
          "missingKeywords": ["<string>", ...],
          "suggestions": ["<string>", ...],
          "sectionBreakdown": [
            { "section": "<string>", "feedback": "<string>", "score": <integer 0-100> }
          ]
        }
        """;

    /// <summary>
    /// Builds the user prompt by embedding the resume and job description into a template.
    /// </summary>
    public static string BuildUserPrompt(string resumeText, string jobDescription) =>
        $"""
        === JOB DESCRIPTION ===
        {jobDescription.Trim()}

        === CANDIDATE RESUME ===
        {resumeText.Trim()}

        Analyse the resume against the job description and return the JSON response.
        """;
}
