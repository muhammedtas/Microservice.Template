using System;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Infrastructure.Repositories.CacheManager;
using IntradayDashboard.WebApi.Model;
using IntradayDashboard.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IntradayDashboard.WebApi.Controllers
{
    [Route("api/home")]
    [ApiController]
    [AllowAnonymous]
    //[Authorize]
    public class HomeController : ControllerBase 
    {
        private readonly IOfferApiService _offerApiService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICacheManager _cacheService;

        private readonly IBackGroundJobsService<Offer> _offerBackGroundJobsService;
        private readonly IBackGroundJobsService<Consumption> _consumptionBackGroundJobsService;

        public HomeController(IOfferApiService offerApiService, ILoggerFactory logger, IMapper mapper
        ,ICacheManager cacheService
        ,IBackGroundJobsService<Offer> offerBackGroundJobsService
        ,IBackGroundJobsService<Consumption> consumptionBackGroundJobsService
        )
        {
            _logger = logger.CreateLogger("WebApi.Controllers.HomeController");
            _mapper = mapper;
            _cacheService = cacheService;
            _offerApiService = offerApiService;
            _offerBackGroundJobsService = offerBackGroundJobsService;
            _consumptionBackGroundJobsService = consumptionBackGroundJobsService;

            #region Testing Communications between bgj and esb


            RecurringJob.AddOrUpdate("Call Get Offer Job", () => _offerBackGroundJobsService.GetOffers(), Cron.Daily, TimeZoneInfo.Local, "offer_queue");

            RecurringJob.AddOrUpdate("Create Dummy Offer job", () => _offerBackGroundJobsService.CreateDummyOffer(), Cron.Daily, TimeZoneInfo.Local, "offer_queue");

            RecurringJob.AddOrUpdate("Create Dummy Offer Via Service Bus ", () => _offerBackGroundJobsService.CreateDummyOfferViaServiceBus(), Cron.Minutely, TimeZoneInfo.Local, "offer_queue");

            RecurringJob.AddOrUpdate("Create Dummy Consumption Job", () => _consumptionBackGroundJobsService.CreateDummyConsumption(), Cron.Daily, TimeZoneInfo.Local, "consumption_queue");

            RecurringJob.AddOrUpdate("Create Dummy Donsumption Via Service Bus ", () => _consumptionBackGroundJobsService.CreateDummyConsumptionViaServiceBus(), Cron.Minutely, TimeZoneInfo.Local, "consumption_queue");

            /// Hourly Scheduled

            RecurringJob.AddOrUpdate("Create Dummy Offer job Every Day at 9:30", () => _offerBackGroundJobsService.CreateDummyOffer(), Cron.Daily(9,30), TimeZoneInfo.Local, "offer_queue");
            
            RecurringJob.AddOrUpdate("Create Dummy Offer job Every day at 12 hours", () => _offerBackGroundJobsService.CreateDummyOffer(), Cron.HourInterval(12), TimeZoneInfo.Local, "offer_queue");

            RecurringJob.AddOrUpdate("Create Dummy Offer Via Service Bus every 15 minutes ", () => _offerBackGroundJobsService.CreateDummyOfferViaServiceBus(), Cron.MinuteInterval(15), TimeZoneInfo.Local, "offer_queue");



            #endregion

        }

        // GET api/home/get
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
        
           var offerDatas = await _offerApiService.GetOffers();

           return Ok(offerDatas);
        }

        // GET api/home/getAll
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllOffer()
        {
           var offerDatas = await _offerApiService.GetOffers();

           return Ok(offerDatas);
        }
        

        // GET api/home/get/5
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetOffer(int id)
        {
            var returnData = await _offerApiService.GetOffer(id);
            //var returnData = _cacheService.Get<OfferModel>("offercachedata");
            return Ok(returnData);
            
        }

        [HttpPut("saveOffer")]
        public async Task<IActionResult> SaveOffer(OfferModel offer) {


            //_cacheService.Set("offercachedata", offer, 5);
            var returnData =await _offerApiService.SaveOffer(offer);
            if (returnData)
            return CreatedAtAction(nameof(Offer), new { Data = offer.Data }, offer);
            
            return BadRequest("Can not save offer");

        }

        [HttpDelete("deleteOffer/{id}")]
        public async Task<IActionResult> DeleteOffer(int id) {
            var resultData =  await _offerApiService.DeleteOffer(id);
            if (resultData) return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status301MovedPermanently); 
            
             return BadRequest("Could not be deleted");
        }

    }
}
