using System.Collections.Generic;
using System.Threading.Tasks;
using Sofisoft.Accounts.Security.API.Application.Adapters;

namespace Sofisoft.Accounts.Security.API.Application.Services
{
    public interface IAccessService
    {
        Task<MyAccessDto> GetAccessOptionAync(string optionId);
        Task<IEnumerable<TreeListDto>> GetAccessToOptionsAsync(string moduleId);
    }
}