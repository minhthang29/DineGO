using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DineGO_Api.Repository
{
    // Interface for handling email sending
    public interface IMailSenderRepository
    {
        /// <summary>
        /// Sends an OTP code to the specified email address.
        /// </summary>
        /// <param name="mailSender">Recipient's email address</param>
        /// <example>
        /// mailRepo.SendOTP("user@example.com");
        /// </example>
        void SendOTP(string mailSender);

        /// <summary>
        /// Sends an email with a custom message.
        /// </summary>
        /// <param name="mailSender">Recipient's email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="bodyGenerator">Function to generate the email content</param>
        /// <example>
        /// mailRepo.SendMail("user@example.com", "Hello", () => "Thank you for signing up!");
        /// </example>
        void SendMail(string mailSender, string subject, Func<string> bodyGenerator);
    }
}
