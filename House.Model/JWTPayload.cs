using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model
{
    public class JWTPayload
    {
        public string iss { get; set; }
        public long sub { get; set; }
        public List<string> role { get; set; }
        public string aud { get; set; }
        public long exp { get; set; }
        public long nbf { get; set; }
        public long iat { get; set; }
        public string jti { get; set; }
    }

    public class ServiceJwtPayload
    {
        public string iss { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public long exp { get; set; }
        public long nbf { get; set; }
        public long iat { get; set; }
        public string jti { get; set; }
    }
}
