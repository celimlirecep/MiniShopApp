using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniShopApp.WebUI.EmailServices;
using MiniShopApp.WebUI.Identity;
using MiniShopApp.WebUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniShopApp.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login( string ReturnUrl=null)
        {
            return View(
                new LoginModel()
                {
                    ReturnUrl=ReturnUrl
                }
                );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user==null)
            {
                ModelState.AddModelError("", "Böyle bir kullanıcı bulunamadı!");//en üstte gözükmesi için ("username","kullanıcı bulunamasuı")
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Hesabınız onaylanmamıştır! Lütfen mail adresinizi kontrol ediniz.");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user,model.Password,true,false);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }
            ModelState.AddModelError("", "Kullancı adı yada parola hatalı giriş yaptınız!");

            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//cookielerin sadece açılan tarayıcıda görülebilmesini sağlar

        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new User()
            {
                FirstName=model.FirstName,
                LastName=model.LastName,
                UserName=model.UserName,
                Email=model.Email
            };

            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)//Create işlemi başarılıysa
            {
                //Burada mail onay işlemleri yapılacak
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId=user.Id,
                    token=code,
                });

                //mail gönderme işlemleri

                await _emailSender.SendEmailAsync(model.Email, "MiniShopApp Hesap Onaylama", $"Lütfen email hesabınızı onaylamak için <a href='https://localhost:5001{url}'>tıklayınız.</a>");
                CreateMessage("Kaydınız başarılı", "success");
                return RedirectToAction("Login", "Account");

            }
            CreateMessage("Bir sorun oluştu, lütfen tekrar deneyiniz", "danger");

            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if (userId==null || token ==null)
            {
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user!=null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    CreateMessage("hesabınız Onaylanmıştır", "success");
                    return View();
                }

            }
            CreateMessage("Hesabınız onaylanamadı. Lütfen bilgileri kontrol ederek, yeniden deneyiniz!", "warning");
            return View();

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }


        public IActionResult ForgotPassWord()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassWord(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                CreateMessage("Email adresi boş bırakılamaz!", "Warning");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user==null)
            {
                CreateMessage("Email adresi bulunamadı!", "Warning");
                return View();
            }
        

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);//token oluşturuldu

         //url oluşturma
            var url = Url.Action("ResetPassword", "Account", new 
            {
                userId=user.Id,
                token=token
            });
            //mail gönderme
            await _emailSender.SendEmailAsync(user.Email, "MiniShopApp Reset Password", $"Lütfen şifrenizi belirlemek için  <a href='https://localhost:5001{url}'>tıklayınız.</a>");//url in başında slaş olucak ondan koymadık
            CreateMessage("E postanıza şifre yenileme mail gönderildi kontrol ediniz!", "Warning");

            return Redirect("~/");
        }

        public IActionResult ResetPassword( string userId,string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                CreateMessage("Geçersiz işlem!", "danger");
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordModel()
            {
                Token = token
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                CreateMessage("Böyle bir kullanıcı bulunamadı!", "Warning");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                CreateMessage("İşleminiz başarılı ile gerçekleştirilmiştir", "success");
                return RedirectToAction("Login");

            }
            CreateMessage("İşlem başarısız oldu daha sonra tekrar deneyiniz!","warning");
            return View(model);
        }









        private void CreateMessage(string message, string alertType)
        {
            var msg = new AlertMessage()
            {
                Message = message,
                AlertType = alertType
            };
            TempData["Message"] = JsonConvert.SerializeObject(msg);
        }




    }
}
