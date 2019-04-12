using System.Threading.Tasks;

namespace IntradayDashboard.WebApi.Model.MQModels.Consumption
{
    public interface IConsumptionMQService
    {
        Task ConsumptionServiceWorker(string value);

    }
}