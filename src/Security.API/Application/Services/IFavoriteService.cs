using System.Collections.Generic;
using System.Threading.Tasks;
using Sofisoft.Accounts.Security.API.Application.Adapters;
using Sofisoft.Accounts.Security.API.Models;

namespace Sofisoft.Accounts.Security.API.Application.Services
{
    public interface IFavoriteService
    {
        Task<Favorite> AddAsync(string optionId);
        Task<bool> CheckFavoriteAsync(string optionId);
        Task DropAsync(string optionId);
        Task<(long total, IEnumerable<FavoriteListDto> data)> ListAsync(string name,
            string sort, int pageSize, int start);
    }
}