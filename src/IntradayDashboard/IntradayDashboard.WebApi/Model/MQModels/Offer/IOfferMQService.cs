using System.Threading.Tasks;

namespace IntradayDashboard.WebApi.Model.MQModels.Offer
{
    public interface IOfferMQService
    {
        Task OfferServiceWorker(string value);
    }
}