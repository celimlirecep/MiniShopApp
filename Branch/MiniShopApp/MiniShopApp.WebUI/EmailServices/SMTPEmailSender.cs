using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MiniShopApp.WebUI.EmailServices
{
    public class SMTPEmailSender : IEmailSender
    {
        //Aspsetting içerisindeki bilgileri çekmek için(Dependency injection)
        private string _host;
        private int _port;
        private string _userName;
        private bool _enableSSL;
        private string _password;

        public SMTPEmailSender(string host,int port , bool enableSSL, string userName, string password)
        {
            _enableSSL = enableSSL;
            _host = host;
            _password = password;
            _port = port;
            _userName = userName;
        }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(this._host, this._port)
            {
                Credentials = new NetworkCredential(this._userName, this._password),
                EnableSsl = this._enableSSL
            };

            return client.SendMailAsync(
                new MailMessage(this._userName, email, subject, htmlMessage)
                {
                    IsBodyHtml=true
                }

                );
        }
    }
}
