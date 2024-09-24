namespace Sofisoft.Accounts.Security.API
{
    public class SecuritySetting
    {
        public ServicesSetting Services { get; set; }
    }

    public class ServicesSetting
    {
        public string LoggingUrl { get; set; }
        public string WorkingSpaceUrl { get; set; }
    }
}