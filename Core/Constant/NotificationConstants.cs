namespace Core.Constant
{
    /// <summary>
    /// Provides constant notification messages used throughout the system.
    /// Helps avoid typos and makes it easier to maintain or refactor notification content.
    /// Use NotificationConstants.[NAME] instead of hard-coded strings in your code.
    /// </summary>
    /// <author>Phuonghh</author>
    public static class NotificationConstants
    {
        public const string EMAIL_INVALID = "Email không hợp lệ.";
        public const string PHONE_INVALID = "Số điện thoại không hợp lệ.";

        public const string NAME_REQUIRED = "Họ và tên không được để trống.";
        public const string USERNAME_REQUIRED = "Tài khoản không được để trống.";
        public const string PASSWORD_INVALID = "Mật khẩu phải có ít nhất 3 ký tự.";
        public const string CONFIRM_PASSWORD_NOT_MATCH = "Xác nhận mật khẩu không khớp.";
        public const string EMAIL_REQUIRED = "Vui lòng nhập email.";
        public const string EMAIL_NOT_EXIST = "Email không tồn tại trong hệ thống.";
        public const string PASSWORD_SENT = "Mật khẩu mới đã được gửi đến email của bạn.";
        public const string REGISTER_SUCCESS = "Đăng ký thành công. Vui lòng đăng nhập.";
        public const string LOGOUT_SUCCESS = "Bạn đã đăng xuất tài khoản.";
        public const string LOGIN_SUCCESS = "Chào mừng bạn đến với DineGo.";
        public const string USERNAME_EXISTED = "Tên tài khoản đã tồn tại.";
        public const string RESPONSE_CONTENT_IS_EMPTY = "The response content is empty.";
        public const string CUSTOMER_NOT_FOUND = "Không tìm thấy tài khoản!";
        public const string DATA_INVALID = "Dữ liệu không hợp lệ!";
        public const string UPDATE_SUCCESS = "Cập nhật thành công!";
        public const string UPDATE_FAILED = "Cập nhật thất bại!";
        public const string YOU_NOT_LOGIN = "Bạn chưa đăng nhập!";
        public const string CURRENT_PASSWORD_NOT_CORRECT = "Mật khẩu hiện tại không chính xác!";
        public const string PASSWORD_NOT_MATCH = "Mật khẩu mới và xác nhận mật khẩu không khớp!";
        public const string CHANGE_PASSWORD_SUCCESS = "Đổi mật khẩu thành công!";
        public const string CHANGE_PASSWORD_FAILED = "Đổi mật khẩu thất bại!";
        public const string DATE_NOT_YYYY_MM_DD_HH_MM = "Ngày giờ không hợp lệ! Định dạng phải là yyyy-MM-dd HH:mm.";
        public const string DATE_CAN_NOT_PAST = "Không thể đặt chỗ trong quá khứ!";
        public const string RESERVATION_SUCCESS = "Đặt chỗ thành công!";
        public const string RESERVATION_FAILED = "Đặt chỗ thất bại!";
        public const string RESTAURANT_CREATE_SUCCESS = "Tạo nhà hàng thành công!";
        public const string RESTAURANT_CREATE_FAILED = "Tạo nhà hàng thất bại!";
        public const string RESTAURANT_NAME_REQUIRED = "Tên nhà hàng không được để trống!";
        public const string CUSTOMER_INFO_NOT_FOUND = "Không tìm thấy thông tin khách hàng!";
        public const string USER_REGISTERED_SUCCESSFULLY = "User registered successfully.";
        public const string GOOGLE_ACCOUNT_LINKED = "Your Google account has been linked with a password. You can now log in manually.";
        public const string USERNAME_ALREADY_EXISTS = "Username đã tồn tại.";
        public const string INVALID_USERNAME_OR_PASSWORD = "Invalid username or password.";
        public const string TOKEN_INVALID_EMAIL = "Token does not contain a valid email.";
        public const string CUSTOMER_ID_MISMATCH = "Customer ID mismatch.";

        public const string CUSTOMER_WITH_ID_NOT_FOUND = "Customer with ID {0} not found";
        public const string RESERVATION_WITH_ID_NOT_FOUND = "Reservation with ID {0} not found";
        public const string RESERVATION_ID_MISMATCH = "Reservation ID mismatch.";
        public const string RESTAURANT_WITH_ID_NOT_FOUND = "Restaurant with ID {0} not found";
        public const string ADD_CART_SUCCESS = "Đã thêm món vào giỏ hàng!";
        public const string ADD_CART_FAIL = " Thêm vào giỏ hàng thất bại!";
        public const string CATEGORY_CREATE_SUCCESS = "Tạo danh mục thành công!";
        public const string CATEGORY_CREATE_FAILED = "Tạo danh mục thất bại!";
        public const string CATEGORY_UPDATE_SUCCESS = "Cập nhật danh mục thành công!";
        public const string CATEGORY_UPDATE_FAILED = "Cập nhật danh mục thất bại!";
        public const string CATEGORY_DELETE_SUCCESS = "Xóa danh mục thành công!";
        public const string CATEGORY_DELETE_FAILED = "Xóa danh mục thất bại!";
        public const string CATEGORY_NAME_REQUIRED = "Tên danh mục không được để trống!";
        public const string CATEGORY_NOT_FOUND = "Không tìm thấy danh mục!";
        public const string BLOG_CREATE_SUCCESS = "Tạo blog thành công!";
        public const string BLOG_CREATE_FAILED = "Tạo blog thất bại!";
        public const string BLOG_UPDATE_SUCCESS = "Cập nhật blog thành công!";
        public const string BLOG_UPDATE_FAILED = "Cập nhật blog thất bại!";
        public const string BLOG_DELETE_SUCCESS = "Xóa blog thành công!";
        public const string BLOG_DELETE_FAILED = "Xóa blog thất bại!";
        public const string BLOG_NOT_FOUND = "Không tìm thấy blog!";
        public const string BLOG_TITLE_REQUIRED = "Tiêu đề blog không được để trống!";
        public const string EMAIL_EXISTED = "Email đã được sử dụng.";
        public const string SERVER_INVALID_RESPONSE = "Phản hồi không hợp lệ từ máy chủ.";
        public const string SERVER_ERROR = "Không thể kết nối đến máy chủ.";
        public const string UNKNOWN_ERROR = "Đã xảy ra lỗi không xác định.";
        public const string EMAIL_ALREADY_EXISTS = "Email đã tồn tại.";

        public const string NAME_FORMAT_INVALID = "Họ tên chỉ được chứa chữ cái và khoảng trắng.";
        public const string NAME_TOO_LONG = "Họ tên tối đa 100 ký tự.";

        public const string USERNAME_FORMAT_INVALID = "Tên tài khoản chỉ chứa chữ, số hoặc dấu gạch dưới.";
        public const string USERNAME_TOO_LONG = "Tên tài khoản tối đa 50 ký tự.";

        public const string PASSWORD_TOO_LONG = "Mật khẩu tối đa 255 ký tự.";

        public const string EMAIL_TOO_LONG = "Email tối đa 100 ký tự.";

        public const string PHONE_TOO_LONG = "Số điện thoại tối đa 20 ký tự.";

        public const string NAME_TOO_SHORT = "Họ tên phải có ít nhất 2 ký tự.";
        public const string USERNAME_TOO_SHORT = "Tên tài khoản phải có ít nhất 4 ký tự.";
        public const string PASSWORD_TOO_SHORT = "Mật khẩu phải có ít nhất 3 ký tự.";
        public const string EMAIL_TOO_SHORT = "Email phải có ít nhất 6 ký tự.";
        public const string PHONE_TOO_SHORT = "Số điện thoại phải có ít nhất 10 chữ số.";
        public const string CREATE_POST_SUCCESS = "Đăng bài viết thành công. Bài viết sẽ được duyệt trong thời gian sớm nhất.";
        public const string EDIT_POST_SUCCESS = "Cập nhật bài viết thành công. Bài viết sẽ được duyệt trong thời gian sớm nhất.";
        public const string DELETE_POST_SUCCESS = "Xóa bài viết thành công.";
        public const string CREATE_COMMENT_SUCCESS = "Bình luận thành công.";
        public const string EDIT_COMMENT_SUCCESS = "Cập nhật bình luận thành công.";
        public const string DELETE_COMMENT_SUCCESS = "Xóa bình luận thành công.";
        public const string RESTAURANT_UPDATE_SUCCESS = "Cập nhật nhà hàng thành công!";
        public const string RESTAURANT_UPDATE_FAILED = "Cập nhật nhà hàng thất bại!";
        public const string RESTAURANT_DELETE_SUCCESS = "Xóa nhà hàng thành công!";
        public const string RESTAURANT_DELETE_FAILED = "Xóa nhà hàng thất bại!";
        public const string CUSTOMER_CREATE_SUCCESS = "Tạo khách hàng thành công!";
        public const string CUSTOMER_CREATE_FAILED = "Tạo khách hàng thất bại!";
        public const string CUSTOMER_UPDATE_SUCCESS = "Cập nhật khách hàng thành công!";
        public const string CUSTOMER_UPDATE_FAILED = "Cập nhật khách hàng thất bại!";
        public const string CUSTOMER_DELETE_SUCCESS = "Xóa khách hàng thành công!";
        public const string CUSTOMER_DELETE_FAILED = "Xóa khách hàng thất bại!";
        public const string RATING_SUCCESS = "Đánh giá nhà hàng thành công!";
    }
}