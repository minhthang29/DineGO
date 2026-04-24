namespace Core.Constant
{
    /// <summary>
    /// Provides constant session key names used throughout the system.
    /// Helps avoid typos and makes it easier to maintain or refactor session key strings.
    /// Use SessionConstants.[NAME] instead of hard-coded strings in your code.
    /// </summary>
    /// <author>Phuonghh</author>
    public static class SessionConstants
    {
        public const string TOKEN = "token";
        public const string BEARER = "Bearer";
        public const string CUSTOMER_NAME = "cus_name";
        public const string CUSTOMER_ID = "cus_id";
        public const string RESTAURANT_ID = "res_id";
        public const string RESTAURANT_OWNER_ID = "resOwner_id";
        public const string RESTAURANT_OWNER_NAME = "res_owner_name";
        
        public const string NOTIFICATION_LIST = "Notifications";
        public const string NOTIFICATION_NUMBER = "NumberNoti";
    }
}