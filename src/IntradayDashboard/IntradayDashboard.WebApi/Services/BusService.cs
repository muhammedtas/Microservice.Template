using System;
using System.Threading;
using System.Threading.Tasks;
using IntradayDashboard.WebApi.Model.MQModels;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace IntradayDashboard.WebApi.Services
{
    public class BusService: IHostedService
    {
        private readonly IBusControl _busControl;

        public BusService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }

        // public static IBusControl InitializeBus()
        // {
        //     return Bus.Factory.CreateUsingRabbitMq(cfg =>
        //     {
        //         var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
        //         {
        //             h.Username("guest");
        //             h.Password("guest");
        //         });
        //         cfg.ReceiveEndpoint(host, "IntradayDashboard", e =>
        //         e.Consumer<SendMessageConsumer>());
        //     });
        // }
    }
}