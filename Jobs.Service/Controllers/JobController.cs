using Hangfire;
using Masstransit.Contracts;
using MassTransit;
using MassTransit.Scheduling;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading;

namespace Jobs.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private IMessageScheduler _scheduler;
        private IBusControl _bus;
        public JobController( IMessageScheduler scheduler, IBusControl bus)
        {   
            _scheduler = scheduler;
         
            _bus = bus;
        }

        [HttpGet("Start")]
        public async Task IssueAJob() {

            //await using var scope = _serviceProvider.CreateAsyncScope();
            //var scheduler = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();
            //await scheduler.SchedulePublish<SendNotification>(
            //    DateTime.UtcNow + TimeSpan.FromSeconds(30), new()
            //    {
            //        EmailAddress = "frank@nul.org",
            //        Body = "Thank you for signing up for our awesome newsletter!"
            //    });


            await _scheduler.SchedulePublish<DoSomething>(DateTime.Now.AddSeconds(10), new { Id = 1, Name = "test" });
        }

        [HttpGet("StartRecurring")]
        public async Task IssueARecuuringJob()
        {
            Uri sendEndpointUri = new("queue:hangfire");

            var sendEndpoint = await _bus.GetSendEndpoint(sendEndpointUri);
            await sendEndpoint.ScheduleRecurringSend<RecurringMessage>( new Uri($"queue:ahmed-kassem"), new recurring { TimeZoneId="Egypt", EndTime=DateTime.Now.AddMinutes(1).AddSeconds(20),StartTime = DateTime.Now, ScheduleId = "hit2", CronExpression = "* * * * *", },new RecurringMessage { Id =21, Name = "test" });
            await _bus.StartAsync();
            //await _recurringScheduler.ScheduleRecurringSend<DoSomething>( new Uri("http://localhost:5243/hangfire"), new recurring { StartTime=DateTime.Now, ScheduleId="hit", CronExpression= "* * * * *", }, new { Id = 1, Name = "test" });
        } 
        [HttpGet("StartRecurring2")]
        public async Task IssueARecuuringJob2()
        {
            Uri sendEndpointUri = new("queue:hangfire");

            var sendEndpoint = await _bus.GetSendEndpoint(sendEndpointUri);
            await sendEndpoint.ScheduleRecurringSend<RecurringMessage>( new Uri($"queue:ahmed-kassem"), new recurring { StartTime = DateTime.Now, ScheduleId = "hit3", CronExpression = "* * * * *", },new RecurringMessage { Id =21, Name = "test" });
            await _bus.StartAsync();
            //await _recurringScheduler.ScheduleRecurringSend<DoSomething>( new Uri("http://localhost:5243/hangfire"), new recurring { StartTime=DateTime.Now, ScheduleId="hit", CronExpression= "* * * * *", }, new { Id = 1, Name = "test" });
        }

        [HttpGet("StopRecurringJob")]
        public async Task StopRecurringJob(string Id) {
            Uri sendEndpointUri = new("queue:hangfire");

            var sendEndpoint = await _bus.GetSendEndpoint(sendEndpointUri);
            await sendEndpoint.CancelScheduledRecurringSend(Id, "");

        }
    }
    public class recurring : RecurringSchedule
    {
        public string TimeZoneId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public string ScheduleId { get; set; }

        public string ScheduleGroup { get; set; }

        public string CronExpression { get; set; }

        public string Description { get; set; }

        public MissedEventPolicy MisfirePolicy { get; set; }
    }
}
