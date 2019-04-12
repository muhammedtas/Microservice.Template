using System;
using IntradayDashboard.Core.Repositories.Database;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace IntradayDashboard.Infrastructure.Data.Uow
{
    public interface IUnitOfWork: IDisposable
    {
        //IntradayContext Context { get;  }
        IRepository<T> GetRepository<T>() where T : class;
        bool BeginNewTransaction();
        bool RollBackTransaction();
        int Commit();
    }
         
}
