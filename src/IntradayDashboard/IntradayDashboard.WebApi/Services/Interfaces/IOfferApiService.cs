using System.Collections.Generic;
using System.Threading.Tasks;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Repositories.Database;
using IntradayDashboard.WebApi.Model;

namespace IntradayDashboard.WebApi.Services.Interfaces
{
    public interface IOfferApiService
    {
        Task<List<OfferModel>> GetOffers();
        Task<OfferModel> GetOffer(int id);
        Task<bool> SaveOffer(OfferModel model);
        Task<bool> DeleteOffer(int id);
    }
}