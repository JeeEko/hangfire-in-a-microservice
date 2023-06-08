using Hangfire;
using Hangfire.Server;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHangfire(configuration => configuration
//           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//           .UseSimpleAssemblyNameTypeSerializer()
//           .UseRecommendedSerializerSettings()
//           .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfire(cfg => {
    cfg.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
    cfg.UseRecommendedSerializerSettings();
}
) ;
var massTransitSettingSection = builder.Configuration.GetSection("MassTransitConfig");
var massTransitConfig = massTransitSettingSection.Get<MassTransitConfigEntities>();
builder.Services.AddMassTransit(x =>
{

    //x.AddConsumers(Assembly.GetExecutingAssembly());
    x.SetKebabCaseEndpointNameFormatter();
    //Uri schedulerEndpoint = new Uri("queue:scheduler");
    //x.AddMessageScheduler(schedulerEndpoint);
    x.AddPublishMessageScheduler();
    x.AddHangfireConsumers();
    x.UsingRabbitMq((context, cfg) =>
    {

        cfg.UseMessageRetry(r =>
        {       
            r.Interval(2, 10000);
        });

        cfg.UseInMemoryOutbox();
        cfg.UsePublishMessageScheduler();       
        //cfg.UseMessageScheduler(schedulerEndpoint);
        cfg.ConfigureEndpoints(context);
        cfg.Host(massTransitConfig.Host, massTransitConfig.VirtualHost,
            h =>
            {
                h.Username(massTransitConfig.Username);
                h.Password(massTransitConfig.Password);
            }
        );

    });
});
builder.Services.AddMassTransitHostedService(true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard();
app.UseAuthorization();

app.MapControllers();

app.Run();
