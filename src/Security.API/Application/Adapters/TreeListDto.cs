using System.Collections.Generic;

namespace Sofisoft.Accounts.Security.API.Application.Adapters
{
    public class TreeListDto
    {
        public string OptionId { get; set; }
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public string Icon { get; set; }
        public bool Leaf { get; set; }
        public bool Collapsible { get; set; }
        public List<TreeListDto> Children { get; set; }
        public List<string> Paths { get; set; }
    }
}