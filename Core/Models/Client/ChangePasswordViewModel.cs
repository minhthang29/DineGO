using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client
{
    public class ChangePasswordViewModel
    {
        public Customer Customer { get; set; }
        public bool HasPassword { get; set; }
        
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }
        
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmNewPassword { get; set; }
    }

}