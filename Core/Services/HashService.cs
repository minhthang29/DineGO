using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;  
using Microsoft.AspNetCore.Cryptography.KeyDerivation; 
using System.Linq;


namespace Core.Services
{
    /// <summary>
    /// Provides password hashing and verification utilities using PBKDF2 with HMACSHA256.
    /// - HashPassword: Tạo ra chuỗi hash từ mật khẩu gốc, kết hợp với salt ngẫu nhiên.
    ///   Chuỗi trả về có dạng: {saltBase64}:{hashBase64}.
    /// - VerifyPassword: Kiểm tra mật khẩu nhập vào có khớp với hash đã lưu không.
    ///   Tách salt và hash từ chuỗi lưu trữ, hash lại mật khẩu nhập vào với salt đó rồi so sánh.
    /// - GenerateRandomPassword: Sinh mật khẩu ngẫu nhiên với ký tự chữ, số và ký tự đặc biệt.
    /// 
    /// Sử dụng PBKDF2 giúp tăng bảo mật so với hash thông thường, vì có salt và lặp nhiều lần.
    /// 
    /// Lưu ý: Không nên lưu mật khẩu dạng plain text, luôn hash trước khi lưu vào database.
    /// </summary>
    public class HashService
    {
        /// <summary>
        /// Hashes a password using PBKDF2 with a random salt.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <returns>Base64 encoded salt and hash, separated by a colon.</returns>
        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifies a password against a stored hash.
        /// </summary>
        /// <param name="password">The plain text password to verify.</param>
        /// <param name="storedHash">The stored hash in format {salt}:{hash}.</param>
        /// <returns>True if password matches, false otherwise.</returns>
        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedPasswordHash = Convert.FromBase64String(parts[1]);

            byte[] hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);
            return hash.SequenceEqual(storedPasswordHash);
        }

        public string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var result = new char[length];
                var buffer = new byte[length];

                rng.GetBytes(buffer);
                for (int i = 0; i < length; i++)
                {
                    result[i] = chars[buffer[i] % chars.Length];
                }

                return new string(result);
            }
        }
    }
}