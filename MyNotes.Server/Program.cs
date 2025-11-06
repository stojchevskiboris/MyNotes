using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MyNotes.Server.Common;
using MyNotes.Server.Configs;
using MyNotes.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration Section ---
AppParameters.ConnectionString = builder.Configuration.GetConnectionString("devDb2") ?? "";
builder.Services.AddDbContext<MyNotesDbContext>(options =>
    options.UseSqlServer(AppParameters.ConnectionString));

var config = builder.Configuration.GetSection("AppSettings").Get<AppSettingsModel>()
    ?? throw new Exception("AppSettings is null");
AppParameters.AppSettings.AesSecretKey = config.AesSecretKey;
AppParameters.AppSettings.GoogleAuth = config.GoogleAuth;

// --- Repository Registration ---
builder.Services.ConfigureRepositories();

// --- Service Registration ---
builder.Services.ConfigureServices();

// --- Validators Registration ---
builder.Services.ConfigureValidators();

// --- CORS Configuration  ---
builder.Services.ConfigureCors();

// --- Swagger Dev Authorization ---
builder.Services.ConfigureSwaggerAuth();

// --- Authentication Configuration ---
builder.Services.ConfigureAuthentication();

// --- Controllers Registration ---
builder.Services.AddControllers();

// --- Serilog Configuration ---
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext() // Enrich logs with additional context data
        .WriteTo.Console() // Log to the console
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// --- Swagger Configuration ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (string.IsNullOrEmpty(AppParameters.ConnectionString))
{
    Log.Error("AppParameters.ConnectionString is null or empty.");
    return;
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyNotesDbContext>();
    dbContext.Database.Migrate(); // dotnet ef database update
}

// --- Middleware Configuration --- 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            Log.Error(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");
        }
        context.Response.StatusCode = 500; // Internal Server Error
        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseSerilogRequestLogging();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<JwtMiddleware>();
app.MapControllers();
app.MapFallbackToFile("/index.html");

// --- Run the Application ---
app.Run();
