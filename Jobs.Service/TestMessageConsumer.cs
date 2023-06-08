using Masstransit.Contracts;
using MassTransit;
using System.Diagnostics;

namespace Jobs.Service
{
    public class AhmedKassemConsumer : IConsumer<RecurringMessage>
    {
        public Task Consume(ConsumeContext<RecurringMessage> context)
        {
            Debug.WriteLine(context.Message.Name);
            return Task.CompletedTask;
        }
    }
}
    