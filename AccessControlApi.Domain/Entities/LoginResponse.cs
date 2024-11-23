using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlApi.Domain.Entities
{


    public class LoginResponse
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
    }
}
