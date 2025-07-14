using KanjiReader.Domain.EventHandlers.StartGenerating;
using KanjiReader.Domain.EventHandlers.WatanocParsing;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Database.Repositories;
using KanjiReader.Infrastructure.Redis;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Mapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
    

builder.Services.AddDbContext<KanjiReaderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<KanjiReaderDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("REDIS_URL")!));

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

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<WaniKaniClient>();
builder.Services.AddScoped<WatanocClient>();
builder.Services.AddScoped<WaniKaniService>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddScoped<KanjiService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IKanjiRepository, RedisKanjiRepository>();
builder.Services.AddHostedService<WatanocParsingHandler>();
builder.Services.AddHostedService<StartGeneratingHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



// AI generated texts
// More text sources
// Kanji selection from dictionaty