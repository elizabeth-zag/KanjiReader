// See https://aka.ms/new-console-template for more information

using System.Text;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.WaniKani;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<KanjiReaderDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<KanjiReaderDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
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
                    ValidIssuer = context.Configuration["Jwt:Issuer"],
                    ValidAudience = context.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(context.Configuration["Jwt:Key"]))
                };
            });
        
        services.AddHttpClient(); 
        services.AddScoped<WaniKaniClient>();
        services.AddScoped<WatanocClient>();
        services.AddScoped<WaniKaniService>();
        services.AddScoped<TextProcessingService>();
    })
    .Build();
    
using var serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;

var myService = provider.GetRequiredService<TextProcessingService>();
await myService.ProcessText();

// SQL storage
// AI generated texts
// More text sources
// Kanji selection from dictionaty
// Account and authorization