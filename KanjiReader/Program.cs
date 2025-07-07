// See https://aka.ms/new-console-template for more information

using System.Text;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Domain.WaniKani;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<WaniKaniClient>();
builder.Services.AddScoped<WatanocClient>();
builder.Services.AddScoped<WaniKaniService>();
builder.Services.AddScoped<TextProcessingService>();
builder.Services.AddScoped<UserAccountService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();

// SQL storage
// AI generated texts
// More text sources
// Kanji selection from dictionaty
// Account and authorization