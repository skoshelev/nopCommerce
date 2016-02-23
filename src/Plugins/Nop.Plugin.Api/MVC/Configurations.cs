namespace Nop.Plugin.Api.MVC
{
    public class Configurations
    {
        // time is in minutes (30 days = 43200 minutes)
        public const int AccessTokenExpiration = 3000000;
        public const int RefreshTokenExpiration = 43200;
    }
}