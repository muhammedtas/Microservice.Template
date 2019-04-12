using System.Threading.Tasks;
using IntradayDashboard.Core.Model.Entities;

namespace IntradayDashboard.WebApi.Services.Interfaces
{
    public interface IAuthHelper
    {
         Task<string> GenerateJwtToken(User user);
         void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
         bool VerifyUserPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}