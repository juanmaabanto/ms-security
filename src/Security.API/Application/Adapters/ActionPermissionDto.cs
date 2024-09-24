namespace Sofisoft.Accounts.Security.API.Application.Adapters
{
    public class ActionPermissionDto
    {
        public string ActionId { get; set; }
        public bool Allow { get; set; }
        public bool Deny { get; set; }
    }
}