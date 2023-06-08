using Masstransit.Contracts;
using MassTransit;
using System.Diagnostics;

namespace Jobs.Service
{
    public class DoSomethingConsumer : IConsumer<DoSomething>
    {
        public Task Consume(ConsumeContext<DoSomething> context)
        
        {
            Debug.WriteLine(context.Message.Name);
            return Task.CompletedTask;
        }
    }
}
