using System;
using System.Collections.Generic;
using IntradayDashboard.Core.Repositories.Database;
using IntradayDashboard.Infrastructure.Data.DataContexts;
using IntradayDashboard.Infrastructure.Repositories.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntradayDashboard.Infrastructure.Data.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private IDbContextTransaction _transation;
        private bool _disposed;

         public UnitOfWork(DbContext context)
         {
            if (context == null)
                throw new ArgumentNullException("dbContext can not be null.");
 
            try
            {
                _context = context;    
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
            
        }

  
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public bool BeginNewTransaction()
        {
            try
            {
                _transation = _context.Database.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

       
        public bool RollBackTransaction()
        {

            try
            {
                _transation.Rollback();
                _transation = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public int Commit()
        {
            var transaction = _transation != null ? _transation : _context.Database.BeginTransaction();
            using (transaction)
            {
                try
                {
                    if (_context == null)
                    {
                        throw new ArgumentException("Context is null");
                    }
                    int result =  _context.SaveChanges();

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error on save changes ", ex);
                }
            }
        }


         protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
            
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


}