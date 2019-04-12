using System.Threading.Tasks;
using IntradayDashboard.WebApi.Model.MQModels.Offer;
using MassTransit;

namespace IntradayDashboard.WebApi.Services.Interfaces
{
    public interface IBackGroundJobsService<T>
    {
        Task GetOffers();
        Task CreateDummyOffer();
        Task CreateDummyConsumption();
        Task CreateDummyOfferViaServiceBus();
        Task CreateDummyConsumptionViaServiceBus();
    }
}