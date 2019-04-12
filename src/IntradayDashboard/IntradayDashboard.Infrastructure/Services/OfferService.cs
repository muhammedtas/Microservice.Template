using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Services.Interfaces;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using IntradayDashboard.Infrastructure.Repositories.Database;

namespace IntradayDashboard.Infrastructure.Services
{
    public class OfferService : Repository<Offer>, IOfferService
    {
        public OfferService(IntradayDbContext context) : base(context)
        {
        }

    }
}