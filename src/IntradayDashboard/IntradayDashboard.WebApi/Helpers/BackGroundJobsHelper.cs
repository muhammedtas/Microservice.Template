using System;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.Infrastructure.Data.Uow;
using IntradayDashboard.WebApi.Model.MQModels.Consumption;
using IntradayDashboard.WebApi.Model.MQModels.Offer;
using IntradayDashboard.WebApi.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IntradayDashboard.WebApi.Helpers
{
    public class BackGroundJobsHelper<T> : IBackGroundJobsService<T> where T : class
    {
        public T Data { get; set; }
        IRequestClient<OfferMessage> _offerRequestClient;
        IRequestClient<ConsumptionMessage> _consumptionRequestClient;
        public static RequestHandle<OfferMessage> offerRequest;
        public static RequestHandle<ConsumptionMessage> consumptionRequest;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IConsumptionService _consumptionService;
        private readonly IMapper _mapper;

        public BackGroundJobsHelper(
            ILoggerFactory logger,
            IUnitOfWork uow,
            IMapper mapper,
            IConsumptionService consumptionService,
            IRequestClient<OfferMessage> offerRequestClient,
            IRequestClient<ConsumptionMessage> consumptionRequestClient
        )
        {
            _uow = uow;
            _mapper = mapper;
            _consumptionService = consumptionService;
            _offerRequestClient = offerRequestClient;
            _consumptionRequestClient = consumptionRequestClient;
            _logger = logger.CreateLogger("WebApi.Helpers.BackGroundJobsHelper");

        }
        public async Task GetOffers()
        {
            try
            {
                if (offerRequest!= null) {  var response = await offerRequest.GetResponse<OfferDone>();  }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, "Error Occured in CallGetOffer method in this block");
                throw ex;
            }

        }

        public async Task CreateDummyOffer()
        {
            try
            {

                Random rnd = new Random();
                var dummyOffer = new Offer()
                {
                    Data = "Dummy Offer Creator",
                    Value = rnd.Next(1, 1000),
                    TenantId = 1
                };

                var repo = _uow.GetRepository<Offer>();

                await repo.AddAsync(dummyOffer);
                _uow.Commit();
                
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, "Error occured in CallSetOffer method");
                throw;
            }
        }

        public async Task CreateDummyConsumption()
        {
            try
            {
                Random rnd = new Random();
                var dummyConsumption = new Consumption() {
                Data = "Dummy Consumption Creator",
                Value = rnd.Next(1,1000),
                TenantId = 1

                };

                var repo = _uow.GetRepository<Consumption>();

                await repo.AddAsync(dummyConsumption);
                _uow.Commit();
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
            
        }

        public async Task CreateDummyOfferViaServiceBus()
        {
            try
            {
                var dummyOffer = new Offer() {
                Data = "Dummy Offer Creator Via ServiceBus",
                Value = new Random().Next(1,1000),
                TenantId = 1
                };

                offerRequest = _offerRequestClient.Create(dummyOffer);
                var response = offerRequest.GetResponse<OfferDone>().Result;
                var result = response.Message;
                var dbData = _mapper.Map<Offer>(result);
                var resultData = Task.FromResult<object>(offerRequest);

                var repo = _uow.GetRepository<Offer>();

                await repo.AddAsync(dbData);
                _uow.Commit();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occured in CreateDummyOfferViaServiceBus");
                throw;
            }
        }

        public async Task CreateDummyConsumptionViaServiceBus()
        {
            try
            {
                var dummyConsumption = new Consumption()
                {
                    Data = "Dummy Consumption Creator Via ServiceBus",
                    Value = new Random().Next(1, 1000),
                    TenantId = 1
                };

                consumptionRequest = _consumptionRequestClient.Create(dummyConsumption);
                var response = consumptionRequest.GetResponse<ConsumptionDone>().Result; 
                var result = response.Message;
                var dbData = _mapper.Map<Consumption>(result);
                var resultData = Task.FromResult<object>(consumptionRequest);

                var repo = _uow.GetRepository<Consumption>();

                await repo.AddAsync(dbData);
                _uow.Commit();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occured in CreateDummyOfferViaServiceBus");
                throw;
            }
        }
    }
}