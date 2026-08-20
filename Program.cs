




using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // <-- Bu satırın varlığından emin olun
using Serilog;

















var builder = WebApplication.CreateBuilder(args);

// Serilog Yapılandırması
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// --- 1. SERVİS KAYITLARI (BUILDER) ---

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// Swagger / OpenAPI Yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ilkrepom API", 
        Version = "v1" 
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer <token>' formatında JWT token'ınızı giriniz."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication Yapılandırması
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"] 
    ?? "BuCokGizliVeUzunBirSecretKeyOlmalidir123456!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "ilkrepomAPI",
        ValidAudience = jwtSettings["Audience"] ?? "ilkrepomClient",
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

// FluentValidation & MediatR Servisleri
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// JWT Token Generator Servis Kaydı
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Rate Limiting Yapılandırması
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// CORS Politikası
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOriginsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- 2. MIDDLEWARE VE PIPELINE YAPILANDIRMASI (APP) ---

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowedOriginsPolicy");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();