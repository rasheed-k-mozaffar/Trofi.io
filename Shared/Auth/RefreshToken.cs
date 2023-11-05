using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trofi.io.Shared.Auth
{
    public class RefreshToken
    {
        public string? Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime RevokedOn { get; set; }
        public bool? IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool? IsActive => DateTime.UtcNow >= CreatedOn;

    }
}
