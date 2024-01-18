using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        const string ARGUMENT_EXCEPTION = "System.ArgumentException";

        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("---> Consuming faulty creation");

            var exception = context.Message.Exceptions.First();

            if(exception.ExceptionType == ARGUMENT_EXCEPTION) {
                context.Message.Message.Model = "FooBar";
                await context.Publish(context.Message.Message);
            }
            else
            {
                await Console.Out.WriteLineAsync("--> Not an argument exception - update error dashboard somewhere");
            }
        }
    }

    
}

