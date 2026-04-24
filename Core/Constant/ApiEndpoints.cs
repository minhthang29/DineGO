/// <summary>
/// Represents the constants required for API configuration.
/// </summary>
namespace Core.Constant
{
    /// <summary>
    /// Provides API endpoint constants for various resources.
    /// </summary>
    public static class ApiEndpoints
    {
        /// <summary>
        /// Endpoint for product-related operations.
        /// </summary>
        public const string PRODUCT = "Product";

        /// <summary>
        /// Endpoint for category-related operations.
        /// </summary>
        public const string CATEGORY = "Category";
        public const string CATEGORY_BY_ID = "Category/id?ID=";

        /// <summary>
        /// Endpoint for retrieving a product by its ID.
        /// /// </summary>
        public const string PRODUCT_BY_ID = "Product/id?id=";

        public const string NOTIFICATION = "Notification";
        public const string NOTIFICATION_BY_ID = "Notification/id?id=";

        public const string CUSTOMER = "Customer";
        public const string CUSTOMER_BY_ID = "Customer/id?id=";
        public const string CUSTOMER_FORGET_PASSWORD = "Customer/forgetpassword?email=";
        public const string CUSTOMER_CHECK_USERNAME = "Customer/username?username=";
        public static string GetCustomerCheckLoginUrl(string username, string password)
        {
            return $"Customer/checklogin?username={username}&password={password}";
        }

        public const string BLOG = "Blog";
        public const string BLOG_BY_ID = "Blog/id?ID=";
        public const string BLOG_DELETE_BY_ID = "Blog?ID=";

        public const string ORDER = "Order";
        public const string ORDER_BY_ID = "Order/id?id=";
        public const string ORDER_BY_CUSTOMER = "Order/GetCusId?customerID=";

        public const string PAYMENT_BY_ID = "Payment/GetCusIdPayment?CustomerID=";

        public const string CART = "Cart";
        public const string CART_BY_CUSID = "Cart/cusId?cusId=";
        public const string CART_BY_ID = "Order/id?id=";

        public const string RESTAURANT = "Restaurant";
        public const string RESTAURANT_BY_ID = "Restaurant/id?id=";
        public const string RESTAURANT_BY_RESTAURANT_OWNER_ID = "Restaurant/res_owner_id?res_owner_id=";

        public const string RESTAURANT_SEARCH = "Restaurant/search?name={0}&address={1}";

        public const string RESTAURANT_OWNER = "RestaurantOwner";
        public const string RESTAURANT_OWNER_BY_ID = "RestaurantOwner/id?Id=";
        public const string RESERVATION = "Reservation";
        public const string CONFIRM_RESERVATION = "Reservation/confirm";
        public const string RESTAURANT_OWNER_BY_CUS_ID = "RestaurantOwner/cusId?Id={0}";
        public const string RESERVATION_BY_CUSID = "Reservation/cus_id?cus_id=";
        public const string RESERVATION_BY_RESID = "Reservation/res_id?res_id=";
        public const string RESERVATION_BY_TABLEID = "Reservation/GetReservationByTable?table_id=";
        public const string PAYMENT = "Payment";
        public const string PAYMENT_BY_CUSID = "Payment/cus_id?cus_id=";

        public const string AUTH_GOOGLE_LOGIN = "auth/login-google";
        public const string AUTH_GOOGLE_RESPONSE = "auth/google-response";
        public const string AUTH_GOOGLE_TOKEN = "auth/google-token";
        public const string AUTH_LOGIN = "auth/login";
        public const string AUTH_REGISTER = "auth/register";
        public const string AUTH_SEND_OTP = "auth/send-otp";
        public const string AUTH_CHECK_OTP = "auth/check-otp?email={0}&otp={1}";

        public const string AUTH_FORGOT_PASSWORD = "auth/forgetpassword?email={0}";
        public const string POST = "Post";
        public const string POST_BY_ID = "Post/id?ID=";
        public const string COMMENT = "Comment";

        public const string FOOD = "Food";
        public const string CART_DELETE = "Cart/cartFoodId?cartFoodId=";
        public const string CART_UPDATE_QUANTITY = "Cart";
        public const string CART_MARK = "Cart/mark-bought";
        public const string CART_CHECKOUT = "Cart/checkout";
        public const string SEND = "chat/send";
        public const string FRIEND_LIST = "chat/friend-list?cusId=";
        public const string CHAT_HISTORY = "chat/history";
        public const string FRIEND_REQUEST = "chat/request";
        public const string FRIEND_ACCEPT = "chat/accept";
        public const string FRIEND_SEARCH = "chat/search";
        public const string CHECK_FRIEND = "chat/check-friend";
        public const string CHAT_MARK_READ = "chat/mark-read";
        public const string CHAT_UNREAD_COUNT = "chat/unread-count";
        public const string FRIEND_REQUEST_RESOWNER = "chat/request-resowner";
        public const string RESOWNER_FOLLOWER = "chat/resowner-follower";
        public const string RESOWNER_FRIENDS = "chat/resowner-friends";
        public const string RESOWNER_CUSTOMER_FOLLOWERS = "Chat/customer-followers";
        public const string RESTAURANTS_BY_CUSTOMER_FOLLOWER = "chat/restaurants-followed-by-customer";
        public const string RESTAURANTS_AND_FOLLOWERS_BY_RESOWNER = "chat/resowner-restaurants-and-followers";
        public const string GET_RESOWNER_ID_BY_RESID = "chat/restaurant/";




        public const string LIKE = "Like";
        public const string LIKECOUNT = "Like/React";
        public const string GET_REACTIONS = "Like/GetReactions";

        public const string AI_SUGGEST_TAGS = "AI/suggest-tags?text=";
        public const string AI_SUGGEST_FOOD = "AI/suggest-food?text=";

        public const string AI_UPDATE_CATEGORY_TAGS = "AI/update-tags";
        public const string AI_UPDATE_PRIORITY = "AI/priority/update?cusId={0}&text={1}";
        public const string AI_ADD_CLICK = "AI/priority/click?cusId={0}&tag={1}";
        public const string AI_SET_WEIGHT = "AI/priority/set-weight?cusId={0}&tag={1}&weight={2}";
        public const string AI_SUGGEST_FULL = "AI/suggest-full";


        public const string RATING = "Rating";
        public const string MENU = "Menu";
        public const string MENU_BY_RESTAURANT = "Menu/restaurant";
        public const string TABLE = "Table";
        public const string TABLE_BY_ID = "Table/id?id=";
        public const string TABLE_BY_RESID = "Table/res_id?res_id=";
        public const string TABLE_AREA = "TableArea";
        public const string AREA_BY_RESID = "TableArea/res_id?res_id=";

        public const string DASHBOARD = "Dashboard";

        public const string FOLLOW_RESTAURANT = "Restaurant/follow";
        public const string UNFOLLOW_RESTAURANT = "Restaurant/unfollow";
        public const string CHECK_FOLLOW_RESTAURANT = "Restaurant/check-follow";
        public const string VOUCHER = "Voucher";
        public const string RATING_HAS_COMPLETED_ORDER = "Rating/has-completed-order/";

        public const string VERIFICATION = "Verification";
        public const string VERIFICATION_BY_RESID = "Verification/res_id?res_id=";
        public const string DASHBOARD_RESTAURANT_OWNER = "Dashboard/restaurant-owner";

        // CustomerPoint endpoints
        public const string CUSTOMER_POINT_UPDATE = "customerpoint/update";
        public const string CUSTOMER_POINT_GET = "customerpoint/{0}";
        public const string CUSTOMER_POINT_HISTORY = "customerpoint/{0}/history";
        public const string CUSTOMER_POINT_AVAILABLE = "customerpoint/{0}/vouchers/available";
        public const string CUSTOMER_POINT_OWNED = "customerpoint/{0}/vouchers/owned";
        public const string CUSTOMER_POINT_REDEEM = "customerpoint/redeem";
        public const string CUSTOMER_POINT_GIFT = "customerpoint/gift";
        public const string CUSTOMER_POINT_DELETE = "CustomerPoint/{0}/voucher/{1}";
        public const string CUSTOMER_POINT_ALL_HISTORY_WITH_NAME = "CustomerPoint/histories/all";
        public const string CUSTOMER_POINT_TRANSFER = "CustomerPoint/transfer";
        public const string CUSTOMER_POINT_USE_VOUCHER = "CustomerPoint/use-voucher";

        // 👇 Thêm 2 endpoints mới
        public const string CART_CLEAR_SELECTED = "Cart/clear-selected";
        public const string ORDER_CREATE_WITH_DETAILS = "Order/create-with-details";
        // ========== Ad Slot ==========
        public const string AD_SLOTS = "Ad/slots";                // GET all slots
        public const string AD_SLOT_CREATE_UPDATE = "Ad/slots";   // POST create/update slot
        public const string AD_SLOT_DELETE = "Ad/slots/{0}";      // DELETE slot
        public const string AD_SLOT_OCCUPIED = "Ad/slots/{0}/occupied"; // GET check slot occupied

        // ========== Ad Registration ==========
        public const string AD_REGISTER = "Ad/register";          // POST register ad
        public const string AD_GET_BY_STATUS = "Ad/ads?isActive={0}"; // GET ads by status

        // ========== Ad History ==========
        public const string AD_HISTORY = "Ad/history";            // GET history (all hoặc theo resOwnerId)

        // ========== Utilities ==========
        public const string AD_DEACTIVATE_EXPIRED = "Ad/deactivate-expired"; // POST deactivate expired
        public const string AD_LOG = "Ad/log";

    }
}