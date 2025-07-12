using KanjiReader.Domain.EventHandler;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.UserAccount;
using KanjiReader.Domain.WaniKani;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<WaniKaniClient>();
builder.Services.AddScoped<WatanocClient>();
builder.Services.AddScoped<WaniKaniService>();
builder.Services.AddScoped<TextProcessingService>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddScoped<StartGeneratingHandler>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddHostedService<EventHandlerService>();

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