using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Shared
{
                             
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }



}
