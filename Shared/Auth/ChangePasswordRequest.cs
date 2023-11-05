using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trofi.io.Shared.Auth
{
    public class ChangePasswordRequest
    {
        public string email { get; set; } = string.Empty;
        public string oldPassword { get; set; } = string.Empty;
        public string newPassword { get; set; } = string.Empty;
    }
}
