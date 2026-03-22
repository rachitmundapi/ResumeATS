using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using ResumeATS.Infrastructure.Configuration;

namespace ResumeATS.Infrastructure.AI;

/// <summary>
/// Low-level HTTP client wrapper for the Groq Chat Completions API.
/// </summary>
public sealed class GroqClient
{
    private readonly HttpClient _httpClient;
    private readonly GroqOptions _options;
    private readonly ILogger<GroqClient> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false
    };

    public GroqClient(HttpClient httpClient, IOptions<GroqOptions> options, ILogger<GroqClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    /// <summary>
    /// Sends a system + user prompt pair to the Groq chat completions endpoint and returns the
    /// assistant's reply as a plain string.
    /// </summary>
    /// <param name="systemPrompt">Instructions that shape the model's behaviour.</param>
    /// <param name="userPrompt">The actual user message / content to process.</param>
    /// <param name="modelOverride">
    /// Optional model override. When null, falls back to <see cref="GroqOptions.Model"/>.
    /// </param>
    public async Task<string> ChatAsync(
        string systemPrompt,
        string userPrompt,
        string? modelOverride = null)
    {
        var requestBody = new
        {
            model = modelOverride ?? _options.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userPrompt   }
            },
            max_tokens = _options.MaxTokens,
            temperature = _options.Temperature
        };

        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Sending request to Groq. Model: {Model}", requestBody.model);

        using var response = await _httpClient.PostAsync("chat/completions", content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Groq API error {StatusCode}: {Body}", (int)response.StatusCode, responseBody);
            throw new HttpRequestException(
                $"Groq API returned {(int)response.StatusCode}: {responseBody}",
                null,
                response.StatusCode);
        }

        // Extract content from choices[0].message.content
        var node = JsonNode.Parse(responseBody)
            ?? throw new InvalidOperationException("Empty response from Groq API.");

        var assistantContent = node["choices"]?[0]?["message"]?["content"]?.GetValue<string>()
            ?? throw new InvalidOperationException("Unexpected Groq response shape.");

        return assistantContent;
    }
}
