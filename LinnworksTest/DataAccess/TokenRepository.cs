using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinnworksTest.DataAccess
{
    public class TokenRepository : GenericRepository<Token>, ITokenRepository
    {
        public TokenRepository(CategoriesManagementContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> IsValidTokenAsync(Guid token)
        {
            return (await GetAll().Where(o => o.Value == token).ToListAsync()).Any();
        }
    }
}
