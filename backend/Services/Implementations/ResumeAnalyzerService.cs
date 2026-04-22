using System.Text.Json;
using ResumeATS.Helpers;
using ResumeATS.Infrastructure.AI;
using ResumeATS.Infrastructure.Configuration;
using ResumeATS.Models.Requests;
using ResumeATS.Models.Responses;
using ResumeATS.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ResumeATS.Services.Implementations;

/// <summary>
/// Orchestrates resume analysis by calling the Groq API and deserialising the structured response.
/// </summary>
public sealed class ResumeAnalyzerService : IResumeAnalyzerService
{
    private readonly GroqClient _groqClient;
    private readonly GroqOptions _options;
    private readonly ILogger<ResumeAnalyzerService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ResumeAnalyzerService(
        GroqClient groqClient,
        IOptions<GroqOptions> options,
        ILogger<ResumeAnalyzerService> logger)
    {
        _groqClient = groqClient;
        _options = options.Value;
        _logger = logger;
    }


    public async Task<ResumeAnalysisResponse> AnalyzeAsync(
        ResumeAnalysisRequest request)
    {
        _logger.LogInformation("Starting ATS analysis. Model override: {Override}", request.ModelOverride ?? "none");

        var systemPrompt = PromptBuilder.BuildSystemPrompt();
        var userPrompt   = PromptBuilder.BuildUserPrompt(request.ResumeText, request.JobDescription);

        var rawJson = await _groqClient.ChatAsync(
            systemPrompt,
            userPrompt
            );

        _logger.LogDebug("Raw Groq response: {Response}", rawJson);

        // LLMs sometimes wrap the JSON in markdown fences (```json ... ```) despite instructions.
        // Strip them before deserialising to avoid a JsonException.
        var cleanJson = StripMarkdownFences(rawJson);

        // Deserialise — a malformed response will surface as a structured error via middleware
        var parsed = JsonSerializer.Deserialize<ResumeAnalysisResponse>(cleanJson, _jsonOptions)
            ?? throw new InvalidOperationException("Groq returned a null or empty analysis.");

        parsed.ModelUsed  = request.ModelOverride ?? _options.Model;
        parsed.AnalysedAt = DateTime.UtcNow;

        _logger.LogInformation("ATS analysis complete. Score: {Score}", parsed.AtsScore);

        return parsed;
    }

    /// <summary>
    /// Strips leading/trailing markdown code fences (e.g. ```json ... ```) that
    /// LLMs sometimes include despite being told not to.
    /// </summary>
    private static string StripMarkdownFences(string raw)
    {
        var trimmed = raw.Trim();

        // Handle ```json\n...\n``` or ```\n...\n```
        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline != -1)
                trimmed = trimmed[(firstNewline + 1)..];

            if (trimmed.EndsWith("```", StringComparison.Ordinal))
                trimmed = trimmed[..^3];
        }

        return trimmed.Trim();
    }
}
