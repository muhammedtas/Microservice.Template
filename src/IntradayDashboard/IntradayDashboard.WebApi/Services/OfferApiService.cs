using System.Collections.Generic;
using System.Threading.Tasks;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.Core.Repositories.Database;
using IntradayDashboard.Infrastructure.Data.Uow;
using IntradayDashboard.WebApi.Model;
using IntradayDashboard.WebApi.Services.Interfaces;
using System.Linq;
using AutoMapper;
using IntradayDashboard.WebApi.Dto;

namespace IntradayDashboard.WebApi.Services
{

    public class OfferApiService : IOfferApiService
    {
        private readonly IUnitOfWork _uow;
        private readonly IOfferService _offerService;
        private readonly ITenantService _tenantService;
        private readonly IMapper _mapper;

        public OfferApiService(IUnitOfWork uow, IOfferService offerService, IMapper mapper, ITenantService tenantService)
        {
            _uow = uow;
            _offerService = offerService;
            _mapper = mapper;
            _tenantService = tenantService;
        }
        public async Task<List<OfferModel>> GetOffers()
        {
            
           var datas = await _offerService.GetAllAsync();
           var returnData = _mapper.Map<List<OfferModel>>(datas);

           return returnData;
        }

        public async Task<OfferModel> GetOffer(int id) {
            var model = await _offerService.GetAsync(id);
            var mappedData = _mapper.Map<OfferModel>(model);
            return mappedData;
        }

        public async Task<bool> SaveOffer(OfferModel offer)
        {

            try
            {
                var repo = _uow.GetRepository<Offer>();

                var mapped = _mapper.Map<Offer>(offer);
                await repo.AddAsync(mapped);
                return _uow.Commit()>0;

            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<bool> DeleteOffer(int id ) {


            var repo =  _uow.GetRepository<Offer>();

            var offer = await repo.GetAsync(id);

            repo.DeleteAsync(offer);

            return _uow.Commit()>0;
        }

    }
}