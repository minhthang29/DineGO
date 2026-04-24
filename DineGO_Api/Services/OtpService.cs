using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DineGO_Api.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace DineGO_Api.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;
        private readonly IMailSenderRepository _mail; // repo gửi mail của bạn

        // cấu hình nhanh
        private const int OtpTtlMinutes = 1;
        private const int OtpMaxAttempts = 1;
        private const int ResendCooldownSeconds = 60;

        public OtpService(IMemoryCache cache, IMailSenderRepository mail)
        {
            _cache = cache;
            _mail = mail;
        }

        public async Task<(bool ok, string message, int? retryAfterSeconds)> SendRegistrationOtpAsync(string emailRaw)
        {
            var email = NormalizeEmail(emailRaw);
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email không hợp lệ", null);

            var key = CacheKey(email);

            if (_cache.TryGetValue(key, out OtpCacheEntry recent))
            {
                var since = DateTime.UtcNow - recent.CreatedAtUtc;
                if (since.TotalSeconds < ResendCooldownSeconds)
                {
                    var wait = ResendCooldownSeconds - (int)since.TotalSeconds;
                    return (false, $"Vui lòng đợi {wait}s để gửi lại OTP.", wait);
                }
            }

            // tạo OTP
            var otp = GenerateSixDigits();
            var salt = GenerateSalt();
            var hash = HashOtp(otp, salt);

            var entry = new OtpCacheEntry
            {
                Hash = hash,
                Salt = salt,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(OtpTtlMinutes),
                Attempts = 0,
                CreatedAtUtc = DateTime.UtcNow
            };
            _cache.Set(key, entry, TimeSpan.FromMinutes(OtpTtlMinutes));

            // gửi mail bằng repo sẵn có
            await Task.Run(() => _mail.SendMail(email, "Mã OTP của bạn",
                () => $"Mã OTP của bạn là: {otp}. Đừng chia sẻ với ai!"));

            // 👇 Thay vì null, trả về cooldown mặc định sau khi gửi thành công
            return (true, "Đã gửi OTP", ResendCooldownSeconds);
        }

        public OtpStatus CheckRegistrationOtp(string emailRaw, string otpInput)
        {
            var email = NormalizeEmail(emailRaw);
            var key = CacheKey(email);

            if (!_cache.TryGetValue(key, out OtpCacheEntry entry))
                return OtpStatus.InvalidOrExpired;

            if (entry.ExpiresAtUtc < DateTime.UtcNow)
            {
                _cache.Remove(key);
                return OtpStatus.InvalidOrExpired;
            }

            if (entry.Attempts >= OtpMaxAttempts)
                return OtpStatus.TooManyAttempts;

            entry.Attempts++;
            _cache.Set(key, entry, entry.ExpiresAtUtc - DateTime.UtcNow);

            var ok = SlowEquals(entry.Hash, HashOtp(otpInput ?? "", entry.Salt));
            if (!ok) return OtpStatus.InvalidOrExpired;

            // OTP đúng → xóa để 1-lần-dùng
            _cache.Remove(key);
            return OtpStatus.Ok;
        }

        // ===== Helpers =====
        private static string CacheKey(string email) => $"otp:register:{email}";
        private static string NormalizeEmail(string? email) => (email ?? "").Trim().ToLowerInvariant();

        private static string GenerateSixDigits()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var n = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
            return n.ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var buf = new byte[16];
            rng.GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        private static string HashOtp(string otp, string salt)
        {
            using var h = new HMACSHA256(Convert.FromBase64String(salt));
            return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(otp)));
        }
        private static bool SlowEquals(string a, string b)
        {
            var ba = Convert.FromBase64String(a);
            var bb = Convert.FromBase64String(b);
            uint diff = (uint)ba.Length ^ (uint)bb.Length;
            for (int i = 0; i < ba.Length && i < bb.Length; i++) diff |= (uint)(ba[i] ^ bb[i]);
            return diff == 0;
        }

        private sealed class OtpCacheEntry
        {
            public string Hash { get; set; } = default!;
            public string Salt { get; set; } = default!;
            public DateTime ExpiresAtUtc { get; set; }
            public int Attempts { get; set; }
            public DateTime CreatedAtUtc { get; set; }
        }
    }

    public enum OtpStatus { Ok, InvalidOrExpired, TooManyAttempts }
}