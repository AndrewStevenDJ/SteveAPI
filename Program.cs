using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SteveAPI.Data;
using SteveAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------------------------
// 1) Configuración de Entity Framework Core + MySQL (Pomelo)
// -------------------------------------------------------------------------
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

// -------------------------------------------------------------------------
// 2) Configuración CORS
// -------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------------------------------------------------------------
// 3) Configuración de JWT Authentication
// -------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"] 
             ?? throw new InvalidOperationException("Jwt:Key no puede ser nulo.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] 
                ?? throw new InvalidOperationException("Jwt:Issuer no puede ser nulo.");
var jwtAudience = builder.Configuration["Jwt:Audience"] 
                  ?? throw new InvalidOperationException("Jwt:Audience no puede ser nulo.");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

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
        ValidIssuer = jwtIssuer,

        ValidateAudience = true,
        ValidAudience = jwtAudience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// -------------------------------------------------------------------------
// 4) Inyección de dependencias de la capa Service
// -------------------------------------------------------------------------
builder.Services.AddScoped<EncriptarService>();
builder.Services.AddScoped<DesencriptarService>();
builder.Services.AddScoped<JwtService>();

// -------------------------------------------------------------------------
// 5) MVC Controllers + Swagger
// -------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------------------------------------------------------
// 6) Migraciones automáticas (desactívalas en producción si lo prefieres)
// -------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// -------------------------------------------------------------------------
// 7) Pipeline HTTP con CORS y autenticación habilitados
// -------------------------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
