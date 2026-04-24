using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Modelss.AuthModel
{
    /// <summary>
    /// DTO containing the ID Token returned from Google after successful client-side login.
    /// </summary>
    public class GoogleLoginTokenRequest
    {
        public string idToken { get; set; }
    }
}