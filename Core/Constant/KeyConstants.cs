namespace Core.Constant
{
    /// <summary>
    /// Provides constant key names used throughout the system (e.g., for ModelState, ViewBag, TempData, etc.).
    /// Helps avoid typos and makes it easier to maintain or refactor key names.
    /// Use KeyConstants.[NAME] instead of hard-coded strings in your code.
    /// </summary>
    /// <author>Phuonghh</author>
    public static class KeyConstants
    {
        public const string NAME = "name";
        public const string USERNAME = "username";
        public const string PASSWORD = "password";
        public const string CONFIRM_PASSWORD = "confirmPassword";
        public const string EMAIL = "email";
        public const string PHONE = "phone";
        public const string MESSAGE = "message";
        public const string ERROR_MESSAGE = "ErrorMessage";
        public const string SUCCESS_MESSAGE = "SuccessMessage";
        public const string CUS_ID = "cus_id";
        public const string CUS_NAME = "cus_name";
        public const string CUS_EMAIL = "cus_email";
        public const string CUS_PHONE = "cus_phone";
        public const string CUS_ADDRESS = "cus_address";
        public const string CUS_BIRTHDAY = "cus_birthday";
        public const string CUS_GENDER = "cus_gender";
        public const string CUS_IMAGE = "cus_image";
    }
}