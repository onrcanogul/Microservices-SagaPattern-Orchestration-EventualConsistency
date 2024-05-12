using MassTransit;
using Shared.PaymentEvents;
using Shared.Settings;

namespace Payment.API.Consumers
{
    public class PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider) : IConsumer<PaymentStartedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
        {

            var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

            if(true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
                {

                };
                await sendEndPoint.Send(paymentCompletedEvent);
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems,
                    Message = "payment-failed"
                };
                await sendEndPoint.Send(paymentFailedEvent);
            }
        }
    }
}
