using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Library.Services
{
    public static class EmailService
    {
        public static void Send(string ToAddress, string Subject, string Body)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("sharuk155796@gmail.com", "XXXMy PasswordXXX"),
                EnableSsl = true
            };
            client.Send("shehabu868@gmail.com", ToAddress, Subject, Body);
        }
    }
}
