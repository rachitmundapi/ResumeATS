namespace ResumeATS.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration options for the Groq API.
/// Bound from the "Groq" section of appsettings.json.
/// </summary>
public sealed class GroqOptions
{
    public const string SectionName = "Groq";

    /// <summary>Your Groq Cloud API key.</summary>
    public string ApiKey { get; set; }

    /// <summary>Base URL of the Groq REST API.</summary>
    public string BaseUrl { get; set; }

    /// <summary>Model identifier to use for chat completions.</summary>
    public string Model { get; set; } 

    /// <summary>Maximum tokens the model may generate in a single response.</summary>
    public int MaxTokens { get; set; }

    /// <summary>Sampling temperature (0 = deterministic, 1 = creative).</summary>
    public double Temperature { get; set; } 

    /// <summary>Request timeout in seconds.</summary>
    public int TimeoutSeconds { get; set; } 
}
