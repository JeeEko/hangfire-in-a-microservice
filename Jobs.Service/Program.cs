using Hangfire;
using Jobs.Service;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire((serviceProvider, configuration) => configuration
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));


var massTransitSettingSection = builder.Configuration.GetSection("MassTransitConfig");
var massTransitConfig = massTransitSettingSection.Get<MassTransitConfigEntities>();
builder.Services.AddMassTransit(x =>
{

    x.AddConsumers(Assembly.GetExecutingAssembly());
    x.SetKebabCaseEndpointNameFormatter();

    x.AddMessageScheduler(new Uri("queue:hangfire"));

    x.UsingRabbitMq((context, cfg) =>
    {
       

        cfg.UseMessageRetry(r =>
        {
            r.Interval(2, 10000);
        });

        cfg.UseMessageScheduler(new Uri("queue:hangfire"));
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

app.UseAuthorization();

app.MapControllers();

app.Run();
