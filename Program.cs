// Program.cs – SteveAPI (.NET 8) – listo para Railway
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;         // 👈 necesario para UseUrls
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SteveAPI.Data;
using SteveAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ───────────────────── Railway: escuchar en el puerto dinámico ─────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ──────────────────────────────────────────────────────────────────────────
// 1) Entity Framework Core + MySQL (Pomelo)
// ──────────────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("MySqlRailway")
                      ?? throw new InvalidOperationException("Connection string 'MySqlRailway' no encontrada.");

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOpts => mySqlOpts.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null))
);

// ──────────────────────────────────────────────────────────────────────────
// 2) CORS
// ──────────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ──────────────────────────────────────────────────────────────────────────
// 3) Autenticación JWT
// ──────────────────────────────────────────────────────────────────────────
var jwtKey      = builder.Configuration["Jwt:Key"]
                 ?? throw new InvalidOperationException("Jwt:Key no puede ser nulo.");
var jwtIssuer   = builder.Configuration["Jwt:Issuer"]
                 ?? throw new InvalidOperationException("Jwt:Issuer no puede ser nulo.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
                 ?? throw new InvalidOperationException("Jwt:Audience no puede ser nulo.");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = jwtIssuer,
            ValidateAudience         = true,
            ValidAudience            = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ──────────────────────────────────────────────────────────────────────────
// 4) Servicios de la capa Service
// ──────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<EncriptarService>();
builder.Services.AddScoped<DesencriptarService>();
builder.Services.AddScoped<JwtTokenGenerator>();

// ──────────────────────────────────────────────────────────────────────────
// 5) MVC + Swagger con esquema Bearer (candadito 🔒)
// ──────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "SteveAPI",
        Version     = "v1",
        Description = "API protegida con JWT"
    });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        Type         = SecuritySchemeType.Http,
        In           = ParameterLocation.Header,
        Description  = "Ingresa: **Bearer {tu_token}**",
        Reference    = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id   = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ──────────────────────────────────────────────────────────────────────────
// 6) Migraciones automáticas (desactiva en producción si lo prefieres)
// ──────────────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// ──────────────────────────────────────────────────────────────────────────
// 7) Pipeline HTTP
// ──────────────────────────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
