using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Modelss.AuthModel
{
    /// <summary>
    /// DTO used for user registration.
    /// </summary>
    public class RegisterRequest
    {
        public string Username { get; set; }
        public bool Gender { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Otp { get; set; }
    }
}