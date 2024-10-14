using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Shared.Dtos
{
    public class SensitiveSettings
    {
        public SensitiveJwtSettings JwtSettings { get; set; }
        public SensitiveEmailSettings EmailSettings { get; set; }
        public string ApplicationInsightsConnectionString { get; set; }
    }

    public class SensitiveJwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
        public string PrivateKeyPath { get; set; }
        public string PublicKeyPath { get; set; }
    }

    public class SensitiveEmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }

}
