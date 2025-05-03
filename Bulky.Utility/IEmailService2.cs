using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public interface IEmailService2
    {
        Task SendEmailAsync2(string toEmail, string subject, string body);
    }
}
