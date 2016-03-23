namespace Nop.Plugin.Api.MVC
{
    public class Configurations
    {
        // time is in minutes (30 days = 43200 minutes)
        public const int AccessTokenExpiration = 3000000;
        public const int RefreshTokenExpiration = 43200;
        public const int DefaultLimit = 50;
        public const int DefaultPageValue = 1;
        public const int DefaultSinceId = 0;
        public const int DefaultCategoryId = 0;
        public const int MaxLimit = 250;
        public const int MinLimit = 1;
        public const string PublishedStatus = "published";
        public const string UnpublishedStatus = "unpublished";
        public const string AnyStatus = "any";
    }
}