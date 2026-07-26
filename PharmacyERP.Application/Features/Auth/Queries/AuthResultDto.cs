using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyERP.Application.Features.Auth.Queries
{
    public class AuthResultDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
