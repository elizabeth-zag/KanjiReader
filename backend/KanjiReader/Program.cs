using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using KanjiReader.Domain.Common.Options;
using KanjiReader.Domain.Common.Options.CacheOptions;
using KanjiReader.Domain.Deletion;
using KanjiReader.Domain.DomainObjects.TextProcessingData;
using KanjiReader.Domain.DomainObjects.TextProcessingData.BaseData;
using KanjiReader.Domain.GenerationRules;
using KanjiReader.Domain.Jobs;
using KanjiReader.Domain.Kanji;
using KanjiReader.Domain.Kanji.WaniKani;
using KanjiReader.Domain.TextProcessing;
using KanjiReader.Domain.TextProcessing.Handlers;
using KanjiReader.Domain.TextProcessing.Handlers.GoogleAiGeneration;
using KanjiReader.Domain.TextProcessing.Handlers.NhkParsing;
using KanjiReader.Domain.TextProcessing.Handlers.SatoriParsing;
using KanjiReader.Domain.TextProcessing.Handlers.WatanocParsing;
using KanjiReader.Domain.UserAccount;
using KanjiReader.ExternalServices.EmailSender;
using KanjiReader.ExternalServices.JapaneseTextSources.GoogleGenerativeAI;
using KanjiReader.ExternalServices.JapaneseTextSources.Nhk;
using KanjiReader.ExternalServices.JapaneseTextSources.SatoriReader;
using KanjiReader.ExternalServices.JapaneseTextSources.Watanoc;
using KanjiReader.ExternalServices.KanjiApi;
using KanjiReader.ExternalServices.WaniKani;
using KanjiReader.Infrastructure.Database.DbContext;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Infrastructure.Database.Repositories;
using KanjiReader.Infrastructure.Redis;
using KanjiReader.Infrastructure.Repositories;
using KanjiReader.Infrastructure.Repositories.Cache;
using KanjiReader.Presentation.EventStream;
using KanjiReader.Presentation.Middlewares;
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

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("REDIS_URL")!));

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
    });

// todo: for dev purposes
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpClient(); 

builder.Services.AddScoped<WaniKaniClient>();
builder.Services.AddScoped<KanjiApiClient>();

builder.Services.AddScoped<WatanocClient>();
builder.Services.AddScoped<NhkClient>();
builder.Services.AddScoped<SatoriReaderClient>();

builder.Services.AddScoped<GoogleGenerativeAiClient>();

builder.Services.AddScoped<WatanocParsingHandler>();
builder.Services.AddScoped<NhkParsingHandler>();
builder.Services.AddScoped<SatoriParsingHandler>();
builder.Services.AddScoped<GoogleAiGenerationHandler>();

builder.Services.AddScoped<WaniKaniService>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddScoped<KanjiService>();
builder.Services.AddScoped<TextService>();
builder.Services.AddScoped<TextParsingService>();
builder.Services.AddScoped<DeletionService>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<TextProcessingHandlersFactory>();
builder.Services.AddScoped<IGenerationRulesService<NhkParsingData, NhkParsingBaseData>, NhkRulesService>();
builder.Services.AddScoped<IGenerationRulesService<WatanocParsingData, WatanocParsingBaseData>, WatanocRulesService>();
builder.Services.AddScoped<IGenerationRulesService<SatoriParsingData, SatoriParsingBaseData>, SatoriRulesService>();

builder.Services.AddScoped<IKanjiCacheRepository, RedisKanjiCacheRepository>();
builder.Services.AddScoped<IWatanocCacheRepository, RedisWatanocCacheRepository>();
builder.Services.AddScoped<ISatoriReaderCacheRepository, RedisSatoriReaderCacheRepository>();
builder.Services.AddScoped<INhkCacheRepository, RedisNhkCacheRepository>();
builder.Services.AddScoped<IKanjiRepository, KanjiRepository>();
builder.Services.AddScoped<IProcessingResultRepository, ProcessingResultRepository>();
builder.Services.AddScoped<IUserGenerationStateRepository, UserGenerationStateRepository>();

builder.Services.AddSingleton<ITextBroadcaster, TextBroadcaster>();

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(nameof(EmailOptions)));
builder.Services.Configure<SatoriReaderParsingOptions>(builder.Configuration.GetSection(nameof(SatoriReaderParsingOptions)));
builder.Services.Configure<TextProcessingOptions>(builder.Configuration.GetSection(nameof(TextProcessingOptions)));
builder.Services.Configure<ThresholdOptions>(builder.Configuration.GetSection(nameof(ThresholdOptions)));
builder.Services.Configure<UserDataOptions>(builder.Configuration.GetSection(nameof(UserDataOptions)));
builder.Services.Configure<GoogleAiApiOptions>(builder.Configuration.GetSection(nameof(GoogleAiApiOptions)));
builder.Services.Configure<KanjiApiOptions>(builder.Configuration.GetSection(nameof(KanjiApiOptions)));
builder.Services.Configure<WaniKaniOptions>(builder.Configuration.GetSection(nameof(WaniKaniOptions)));

builder.Services.Configure<SariReaderCacheOptions>(builder.Configuration.GetSection(nameof(SariReaderCacheOptions)));
builder.Services.Configure<NhkCacheOptions>(builder.Configuration.GetSection(nameof(NhkCacheOptions)));
builder.Services.Configure<WatanocCacheOptions>(builder.Configuration.GetSection(nameof(WatanocCacheOptions)));

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(
        c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("SqlConnection"))));

builder.Services.AddHangfireServer(options =>
{
    options.ServerTimeout = TimeSpan.FromMinutes(1);
});

var app = builder.Build();

app.UseHangfireDashboard();

app.UseHttpsRedirection();
app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/api/texts/stream", EventStreamConfiguration.StreamGet);
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

RecurringJob.AddOrUpdate<DeleteUnusedDataJob>(
    nameof(DeleteUnusedDataJob),
    job => job.Execute(CancellationToken.None),
    Cron.Daily);

// retry stuck in processing jobs
var api = JobStorage.Current.GetMonitoringApi();
var processingJobs = api.ProcessingJobs(0, 100);
var servers = api.Servers();
var orphanJobs = processingJobs.Where(j => servers.All(s => s.Name != j.Value.ServerId));
foreach (var orphanJob in orphanJobs)
{
    BackgroundJob.Requeue(orphanJob.Key);
}

app.Run();