using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace DineGO_Api.SignalRHub
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Lấy userId từ query string
            return connection.GetHttpContext()?.Request.Query["userId"];
        }
    }
}