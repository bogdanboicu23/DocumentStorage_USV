using DocumentStorage.Data;
using DocumentStorage.Data.Repositories;
using DocumentStorage.Data.Repositories.Interfaces;
using DocumentStorage.BusinessLayer.Services;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add memory caching
builder.Services.AddMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocumentStorage API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
var key = Encoding.UTF8.GetBytes(secretKey);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7215",
                "http://localhost:5076",
                "http://localhost:5161",
                "http://client:8080",
                "http://api:8080",
                "http://client-svc",
                "http://client-svc:80",
                "http://client-svc:8080"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<DocumentStorageDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDbContext<DocumentStorageDbContextCodeFirst>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var runtimeDefaultCs = builder.Configuration.GetConnectionString("Default");
var runtimeCodeFirstCs = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(runtimeDefaultCs) || string.IsNullOrWhiteSpace(runtimeCodeFirstCs))
{
    throw new InvalidOperationException("Connection strings are not configured for Kubernetes runtime.");
}

string DescribeDbTarget(string connectionString)
{
    try
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        return $"{builder.DataSource} / {builder.InitialCatalog}";
    }
    catch (Exception ex)
    {
        return $"unparsed connection string ({ex.GetType().Name})";
    }
}

Console.WriteLine($"DB target (Default): {DescribeDbTarget(runtimeDefaultCs)}");
Console.WriteLine($"DB target (CodeFirst): {DescribeDbTarget(runtimeCodeFirstCs)}");

// Register repositories and services
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsageService, UsageService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

var app = builder.Build();

// Apply pending EF migrations on startup (retry until SQL Server is ready)
var retryCount = 0;
const int maxRetries = 60;
while (retryCount < maxRetries)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DocumentStorageDbContext>();
        dbContext.Database.Migrate();

        var codeFirstContext = scope.ServiceProvider.GetRequiredService<DocumentStorageDbContextCodeFirst>();
        codeFirstContext.Database.Migrate();
        break;
    }
    catch (Exception ex)
    {
        retryCount++;
        Console.WriteLine($"Database migration attempt {retryCount}/{maxRetries} failed: {ex}");
        if (retryCount >= maxRetries) throw;
        Thread.Sleep(5000);
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("BlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
