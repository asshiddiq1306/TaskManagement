
using Serilog;
using TaskManagement.Application;
using TaskManagement.Infrastructure;

namespace TaskManagement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File("logs/taskmanagement-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Starting Task Management API...");

                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog
                builder.Host.UseSerilog();
                // Add services to the container.
                builder.Services.AddControllers();
                // Add Application Services
                builder.Services.AddApplication();
                // Add Infrastructure Services  
                builder.Services.AddInfrastructure(builder.Configuration);
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new()
                    {
                        Title = "Task Management API",
                        Version = "v1",
                        Description = "An API to manage tasks and users"
                    });

                    // Include XML comments
                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
                        c.RoutePrefix = string.Empty;
                    });
                }

                // Add request logging
                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        var requestHost = httpContext.Request.Host.Value ?? "unknown";
                        diagnosticContext.Set("RequestHost", requestHost);

                        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown";
                        diagnosticContext.Set("UserAgent", userAgent);
                    };
                });

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                // Ensure database is created
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagement.Infrastructure.Data.TaskManagementDbContext>();

                    Log.Information("Initializing database...");
                    dbContext.Database.EnsureCreated();
                    Log.Information("Database initialized successfully");
                }

                Log.Information("Task Management API started successfully");
                app.Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
