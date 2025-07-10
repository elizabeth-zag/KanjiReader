// See https://aka.ms/new-console-template for more information

using System.Text;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Domain.WaniKani;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
    

builder.Services.AddDbContext<KanjiReaderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<KanjiReaderDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Optional: cookie lifetime
    });

// todo: for dev purposes
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
        policy.WithOrigins("http://localhost:5175")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     })
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters()
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//         };
//     });

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<WaniKaniClient>();
builder.Services.AddScoped<WatanocClient>();
builder.Services.AddScoped<WaniKaniService>();
builder.Services.AddScoped<TextProcessingService>();
builder.Services.AddScoped<UserAccountService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// SQL storage
// AI generated texts
// More text sources
// Kanji selection from dictionaty
// Account and authorization