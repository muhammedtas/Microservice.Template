using System.Threading.Tasks;
using IntradayDashboard.Infrastructure.Data.Uow;
using MassTransit;

namespace IntradayDashboard.WebApi.Model.MQModels.Offer
{
    public class OfferConsumer : IConsumer<OfferMessage>
    {
        private readonly IOfferMQService _service;
        private readonly IUnitOfWork _uow;

        public OfferConsumer(IOfferMQService service, IUnitOfWork uow)
        {
            _service = service;
            _uow = uow;
        }
        public async Task Consume(ConsumeContext<OfferMessage> context) 
        {
            try
            {

                await _service.OfferServiceWorker(context.Message.Data);

                await  context.RespondAsync<OfferDone>( 
                    new
                    {
                        Id = context.Message.Id,
                        Data = $"Received: {context.Message.Data} with Value: {context.Message.Value}",
                        Value = context.Message.Value,
                        TenantId = context.Message.TenantId   
                    }
                );
            }
            catch (System.Exception ex )
            {
                throw ex;
            }
            

        }


    }
}