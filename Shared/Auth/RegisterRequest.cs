using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trofi.io.Shared.Auth
{
    public class RegisterRequest
    {
        public string email { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string password { get; set; } = string.Empty;
    }
}
