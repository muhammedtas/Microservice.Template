using System.Threading.Tasks;
using MassTransit.Util;

namespace IntradayDashboard.WebApi.Model.MQModels.Offer
{
    public class OfferMQService : IOfferMQService
    {
        public Task OfferServiceWorker(string value)
        {
            var data = TaskUtil.Completed;
            return data;
        }
    }
}