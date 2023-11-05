using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trofi.io.Shared.Auth
{
    public class UserManagerResponse
    {
        public string message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string ?JWT {get;set;}
    }
}
