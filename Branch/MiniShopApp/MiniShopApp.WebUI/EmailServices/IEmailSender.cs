using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniShopApp.WebUI.EmailServices
{
    public interface IEmailSender
    {
        //Farklı mail server tekniklerine göre yapılandırılmak için tasarlanmıştır
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
