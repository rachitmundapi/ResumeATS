using Microsoft.Extensions.Options;
using ResumeATS.Infrastructure.AI;
using ResumeATS.Infrastructure.Configuration;
using ResumeATS.Services.Implementations;
using ResumeATS.Services.Interfaces;

namespace ResumeATS.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to keep Program.cs clean.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers Groq configuration, the GroqClient HttpClient, and all application services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind and validate configuration
        services
            .AddOptions<GroqOptions>()
            .Bind(configuration.GetSection(GroqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register a typed HttpClient for GroqClient
        services.AddHttpClient<GroqClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<GroqOptions>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
            client.Timeout     = TimeSpan.FromSeconds(opts.TimeoutSeconds);
        });

        // Application services
        services.AddScoped<IResumeAnalyzerService, ResumeAnalyzerService>();

        return services;
    }

    /// <summary>
    /// Registers Swagger/OpenAPI generation with XML comment support.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title   = "ATS Resume Analyzer API",
                Version = "v1",
                Description = "Analyze resumes against job descriptions using AI."
            });
        });

        return services;
    }
}
