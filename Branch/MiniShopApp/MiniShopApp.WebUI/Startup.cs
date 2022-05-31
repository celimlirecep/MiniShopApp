using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniShopApp.Business.Abstract;
using MiniShopApp.Business.Concrete;
using MiniShopApp.Data.Abstract;
using MiniShopApp.Data.Concrete.EfCore;
using MiniShopApp.WebUI.EmailServices;
using MiniShopApp.WebUI.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniShopApp.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationContext>(options=>options.UseSqlite("Data Source=MiniShopAppDB"));
            services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();//add default ile mail onay� ald����m�z (adddefaulttokenproviders)
            //buraya Identityt ile ilgili �e�itli se�enekleri tan�ml�yaca��z

            services.Configure<IdentityOptions>(options =>
           {

               //Pasword
               options.Password.RequireDigit = true;
               options.Password.RequireUppercase = true;
               options.Password.RequireLowercase = true;
               options.Password.RequiredLength = 6;

               //Lockout
               options.Lockout.MaxFailedAccessAttempts = 3;
               options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
               options.Lockout.AllowedForNewUsers = true;

               //user
               options.SignIn.RequireConfirmedEmail = true;
           }
            
            );
            services.ConfigureApplicationCookie(options => {

                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);//20 dakikal�k bir cookie
                options.SlidingExpiration = true;//her requestte 20 dakikay� resetler
                options.Cookie = new CookieBuilder()
                {
                   
                    HttpOnly = true,
                    Name="MiniShopApp.Security.Cookie",//isim verdik
                    SameSite=SameSiteMode.Strict//cookie sadece a��lan taray�c�da a��labilmesi i�in 

                };
            
            
            
            });
            services.AddScoped<IEmailSender, SMTPEmailSender>(i => new SMTPEmailSender (
            
                Configuration["EmailSender:Host"],
                Configuration.GetValue<int>("EmailSender:Port"),
                Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                Configuration["EmailSender:UserName"],
                Configuration["EmailSender:Password"]
                )
            );



            services.AddScoped<IProductRepository, EfCoreProductRepository>();
            services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
            services.AddScoped<IProductService, ProductManager>();
            //Proje boyunca ICategoryService �a�r�ld���nda, CategoryManager'i kullan.
            services.AddScoped<ICategoryService, CategoryManager>();
            //Projemizin MVC yap�s�nda olmas�n� sa�lar.
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();//login ve register i�lemi i�in gerekli
            app.UseRouting();
            
            app.UseAuthorization();//identity i�in gerekli

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "adminproductcreate",
                    pattern: "admin/products/create",
                    defaults: new { controller = "Admin", action = "ProductCreate" }
                    );
                endpoints.MapControllerRoute(
                 name: "admincategorycreate",
                 pattern: "admin/categories/create",
                 defaults: new { controller = "Admin", action = "CategoryCreate" }
                 );
                endpoints.MapControllerRoute(
                    name: "adminproducts",
                    pattern: "admin/products",
                    defaults: new { controller = "Admin", action = "ProductList" }
                    );
                endpoints.MapControllerRoute(
                    name: "admincategories",
                    pattern: "admin/categories",
                    defaults: new { controller = "Admin", action = "CategoryList" }
                    );
                endpoints.MapControllerRoute(
                    name: "search",
                    pattern: "search",
                    defaults: new { controller = "MiniShop", action = "Search" }
                    );
                endpoints.MapControllerRoute(
                   name: "products",
                   pattern: "products/{category?}",
                   defaults: new { controller = "MiniShop", action = "List" }
                   );
                endpoints.MapControllerRoute(
                    name: "adminproductedit",
                    pattern: "admin/products/{id?}",
                    defaults: new { controller = "Admin", action = "ProductEdit" }
                    );
                endpoints.MapControllerRoute(
                   name: "adminproductedit",
                   pattern: "admin/categories/{id?}",
                   defaults: new { controller = "Admin", action = "CategoryEdit" }
                   );
                endpoints.MapControllerRoute(
                    name: "productdetails",
                    pattern: "{url}",
                    defaults: new { controller = "MiniShop", action = "Details" }
                    );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
