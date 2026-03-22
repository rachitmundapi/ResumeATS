using ResumeATS.Extensions;
using ResumeATS.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerDocumentation();

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────────

// 1. Global exception handler — always first so it wraps everything
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. Swagger — available in all environments (guarded by env check if desired)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ATS Resume Analyzer v1");
    c.RoutePrefix = "swagger"; // UI at /swagger  (avoids root redirect conflict)
});

// 3. HTTPS redirect AFTER Swagger so the /swagger route isn't caught by it
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
//app.Run("http://0.0.0.0:5213");
