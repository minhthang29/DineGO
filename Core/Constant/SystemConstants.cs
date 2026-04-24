namespace Core.Constant
{
    /// <summary>
    /// Provides constant system values used throughout the system (e.g., status names).
    /// Helps avoid typos and makes it easier to maintain or refactor system-related strings.
    /// Use SystemConstants.[NAME] instead of hard-coded strings in your code.
    /// </summary>
    /// <author>Phuonghh</author>
    public static class SystemConstants
    {
        public const string CONFIRMED = "Đã xác nhận";
        public const string REJECTED = "Từ chối";
        public const string PENDING = "Chờ xử lý";
    }
}