using LeisureConnect.Core.Interfaces.Services;
using LeisureConnect.Infrastructure.Data.Context;
using LeisureConnect.Infrastructure.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

const string ANGULAR_ORIGIN = "http://localhost:4200";
const string ANGULAR_CORS_POLICY = "AllowAngularApp";
const string DEFAULT_RATE_POLICY = "default";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions()
    {
        Indented = true,
        SkipValidation = true,
    };
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Cache());
});

// OpenAPI configuration
builder.Services.AddOpenApi();

// Configure DB
builder.Services.AddDbContextFactory<LeisureAustralasiaDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("LEISURE_AUSTRALASIA_DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("LeisureAustralasiaConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
    });
});

// Register services
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAdvertisedPackageService, AdvertisedPackageService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(ANGULAR_CORS_POLICY, policy =>
    {
        policy.WithOrigins(ANGULAR_ORIGIN)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter(DEFAULT_RATE_POLICY, config =>
    {
        config.PermitLimit = 20;
        config.Window = TimeSpan.FromSeconds(10);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 5;
    });
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
else 
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Configure the HTTP pipeline
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseCors(ANGULAR_CORS_POLICY);

// Add security headers
app.Use(async (context, next) => {
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

// Rate limiting
app.UseRateLimiter();

// Output caching
app.UseOutputCache();

app.UseAuthorization();
app.MapControllers();

app.Run();