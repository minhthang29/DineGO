namespace Core.Constant
{
    /// <summary>
    /// Provides constant paths used throughout the system (e.g., for file storage, images, etc.).
    /// Helps avoid typos and makes it easier to maintain or refactor path strings.
    /// Use PathConstants.[NAME] instead of hard-coded strings in your code.
    /// </summary>
    /// <author>Phuonghh</author>
    public static class PathConstants
    {
        public const string IMAGE_PATH = "wwwroot/client/images";
        public const string CUSTOMER_IMAGE_PATH = "customer";
    }
}