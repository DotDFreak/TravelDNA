using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelDNA.Core.Interfaces;
using TravelDNA.Infrastructure.Data;
using TravelDNA.Infrastructure.Repositories;
using TravelDNA.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var mySqlConnectionString = builder.Configuration.GetConnectionString("MySql")
    ?? throw new InvalidOperationException("Connection string 'MySql' not found.");

builder.Services.AddDbContext<TravelDnaDbContext>(options =>
    options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGoogleTokenVerifier, GoogleTokenVerifier>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TravelDNA";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TravelDNA.Web";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalWeb", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("LocalWeb");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();