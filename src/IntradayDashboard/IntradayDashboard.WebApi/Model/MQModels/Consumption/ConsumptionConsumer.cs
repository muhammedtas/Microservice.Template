using System.Threading.Tasks;
using MassTransit;

namespace IntradayDashboard.WebApi.Model.MQModels.Consumption
{
    public class ConsumptionConsumer : IConsumer<ConsumptionMessage>
    {
        private readonly IConsumptionMQService _service;

        public ConsumptionConsumer(IConsumptionMQService service)
        {
            _service = service;
        }
        public Task Consume(ConsumeContext<ConsumptionMessage> context)
        {
            _service.ConsumptionServiceWorker(context.Message.Data);

            var returnData =  context.RespondAsync<ConsumptionDone>(new
            {
                Id = context.Message.Id,
                Data = $"Received: {context.Message.Data} with Value: {context.Message.Value}",
                Value = context.Message.Value,
                TenantId = context.Message.TenantId  
            });
            return returnData;
        }
    }
}