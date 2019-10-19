using System;
using System.Threading.Tasks;

namespace LinnworksTest.DataAccess
{
    public interface ITokenRepository
    {
        Task<bool> IsValidTokenAsync(Guid token);
    }
}