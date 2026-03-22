namespace ResumeATS.Constants;

/// <summary>
/// Contains the available Groq AI model identifiers for use in API requests.
/// </summary>
public static class AiModels
{
    /// <summary>Meta LLaMA 3.3 — 70B parameters, strong general-purpose reasoning.</summary>
    public const string Llama3370B = "llama-3.3-70b-versatile";

    /// <summary>Meta LLaMA 3.1 — 8B parameters, fast and lightweight.</summary>
    public const string Llama318B = "llama-3.1-8b-instant";

    /// <summary>Meta LLaMA 3 — 70B parameters, balanced performance.</summary>
    public const string Llama370B = "llama3-70b-8192";

    /// <summary>Meta LLaMA 3 — 8B parameters, ultra-fast inference.</summary>
    public const string Llama38B = "llama3-8b-8192";

    /// <summary>Mixtral 8x7B — Mixture-of-Experts model, excellent text analysis.</summary>
    public const string Mixtral8x7B = "mixtral-8x7b-32768";

    /// <summary>Google Gemma 2 — 9B parameters.</summary>
    public const string Gemma29B = "gemma2-9b-it";

    /// <summary>Default model used when none is explicitly specified.</summary>
    public const string Default = Llama3370B;
}
