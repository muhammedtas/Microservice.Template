using System.Threading.Tasks;
using GreenPipes.Util;

namespace IntradayDashboard.WebApi.Model.MQModels.Consumption
{
    public class ConsumptionMQService : IConsumptionMQService
    {
        public Task ConsumptionServiceWorker(string value)
        {
            return TaskUtil.Completed;
        }
    }
}