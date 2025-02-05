using carParkApp.DB;
using carParkApp.Jobs;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();

    //NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Add EF Core with MySQL
    builder.Services.AddDbContext<DBContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(9, 2, 0))
        )
    );

    // Add Quartz services
    builder.Services.AddQuartz(q =>
    {
        var jobKey = new JobKey("CarParkJob");

        q.AddJob<CarParkJob>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("CarParkJob-trigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1) // Run every 10 minutes
                .RepeatForever()));
    });

    // Add Quartz hosted service
    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex,"Stopped program because of exception");
    throw;
}
finally
{
    if(logger != null)
    {
        LogManager.Shutdown();
    }
}


