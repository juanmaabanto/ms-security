using System.Collections.Generic;

namespace Sofisoft.Accounts.Security.API.Application.Adapters
{
    public class OptionDto
    {
        public string OptionId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public IEnumerable<ActionDto> Actions { get; set; }
        public bool Favorite { get; set; }
        
    }
}