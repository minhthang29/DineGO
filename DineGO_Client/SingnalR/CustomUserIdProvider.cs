using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace DineGO_Client.SingnalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            var customerId = httpContext?.Session?.GetInt32(SessionConstants.CUSTOMER_ID);
            return customerId?.ToString();
        }
    }
}