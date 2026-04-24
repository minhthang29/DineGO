using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class MailSenderRepository : IMailSenderRepository
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "dinego.noreply@gmail.com";
        private readonly string smtpPass = "czmq jjib udxm yhxn";

        public void SendOTP(string mailSender)
        {
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            SendMail(mailSender, "Mã OTP của bạn", () => $"Mã OTP của bạn là: {otp}. Đừng chia sẻ với ai!");
        }

        public void SendMail(string mailSender, string subject, Func<string> bodyGenerator)
        {
            try
            {
                string bodyContent = bodyGenerator?.Invoke() ?? "Không có nội dung.";

                string htmlBody = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: 'Arial', sans-serif;
                    background-color: #f4f4f4;
                    text-align: center;
                    padding: 40px 0;
                }}
                .email-container {{
                    max-width: 550px;
                    background: white;
                    border-radius: 12px;
                    box-shadow: 0px 8px 20px rgba(0, 0, 0, 0.15);
                    margin: auto;
                    overflow: hidden;
                }}
                .email-header {{
                    background: linear-gradient(135deg, #ff512f, #dd2476);
                    padding: 20px;
                    color: white;
                    font-size: 22px;
                    font-weight: bold;
                    text-transform: uppercase;
                }}
                .email-body {{
                    padding: 30px;
                    font-size: 16px;
                    color: #333;
                    line-height: 1.6;
                }}
                .email-footer {{
                    background: #f9f9f9;
                    padding: 15px;
                    font-size: 14px;
                    color: gray;
                    border-top: 1px solid #ddd;
                }}
                .btn {{
                    display: inline-block;
                    padding: 12px 24px;
                    background: #ff512f;
                    color: white;
                    text-decoration: none;
                    font-size: 16px;
                    font-weight: bold;
                    border-radius: 6px;
                    margin-top: 15px;
                    transition: 0.3s;
                }}
                .btn:hover {{
                    background: #dd2476;
                    transform: scale(1.05);
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>{subject}</div>
                <div class='email-body'>
                    <p>{bodyContent}</p>
                </div>
                <div class='email-footer'>© 2025 DineGO. All rights reserved.</div>
            </div>
        </body>
        </html>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(smtpUser);
                mail.To.Add(mailSender);
                mail.Subject = subject;
                mail.Body = htmlBody;
                mail.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
            }
        }
    }
}