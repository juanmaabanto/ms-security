using System.Collections.Generic;
using System.Threading.Tasks;
using Sofisoft.Accounts.Security.API.Application.Adapters;

namespace Sofisoft.Accounts.Security.API.Application.WebClients
{
    public interface IWorkingSpaceWebClient
    {
        Task<OptionDto> GetOptionByIdAsync(string optionId);
        Task<List<OptionListDto>> GetOptionsByModuleAsync(string moduleId);
    }
}