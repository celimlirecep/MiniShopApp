using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniShopApp.Business.Abstract;
using MiniShopApp.Business.Concrete;
using MiniShopApp.Entity;
using MiniShopApp.WebUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniShopApp.WebUI.Controllers
{
    public class AdminController : Controller
    {


        JobManager jobManager = new JobManager();
        private IProductService _productService;
        private ICategoryService _categoryService;
        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ProductList()
        {
            return View(_productService.GetAll());
        }
        public IActionResult ProductCreate()
        {
            ViewBag.Categories = _categoryService.GetAll();
            var bosmodel = new ProductModel();
            return View(bosmodel);
        }
        [HttpPost]
        public IActionResult ProductCreate(ProductModel model,int[] categoryIds,IFormFile file)
        {
            if (ModelState.IsValid && categoryIds.Count()>0 && file!=null)
            {
                string url = jobManager.MakeUrl(model.Name);
                model.ImageUrl=jobManager.UploadImage(file, url);

                var product = new Product()
                {
                    Name = model.Name,
                    Url = url,
                    Price = model.Price,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    IsApproved = model.IsApproved,
                    IsHome = model.IsHome,
                };
                _productService.Create(product, categoryIds);
                return RedirectToAction("ProductList");
            }
            ViewBag.Categorymessage = "Lütfen bir kategoriseçimi yapınız";
            ViewBag.Categories = _categoryService.GetAll();
            model.CategoryList = categoryIds.ToList();
            if (file==null)
            {
                ViewBag.ImageMessage = "Lütfen Resim Seçiniz!";
            }
                return View(model);
        }
        public IActionResult ProductEdit(int? id)
        {
            var entity = _productService.GetByIdWithCategories((int)id);
            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Url = entity.Url,
                Price=entity.Price,
                Description=entity.Description,
                ImageUrl=entity.ImageUrl,
                IsApproved=entity.IsApproved,
                IsHome=entity.IsHome,
                SelectedCategories=entity.ProductCategories.Select(i=>i.Category).ToList()
            };
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }
        [HttpPost]
        public IActionResult ProductEdit(ProductModel model,int[] categoryIds,IFormFile file)
        {
          
           
            if (ModelState.IsValid && categoryIds.Count() > 0 )
            {
                string url = jobManager.MakeUrl(model.Name);
                if (file != null)
                {
                    model.ImageUrl = jobManager.UploadImage(file, url);
                }
                var entity = _productService.GetById(model.ProductId);
                entity.Name = model.Name;// chance tracker
                entity.Price = model.Price;
                entity.Url = model.Url;
                entity.Description = model.Description;
                entity.IsApproved = model.IsApproved;
                entity.IsHome = model.IsHome;
                entity.ImageUrl = model.ImageUrl;

                _productService.Update(entity, categoryIds);
                return RedirectToAction("ProductList");
            }

            ViewBag.Categorymessage = "Lütfen bir kategoriseçimi yapınız";
            ViewBag.Categories = _categoryService.GetAll();
            model.CategoryList = categoryIds.ToList();
           

            return View(model);

        }
        public IActionResult ProductShow(int productid,bool ishome)
        {
            //burdayım
            var entity = _productService.GetById(productid);
            entity.Name = entity.Name;// chance tracker
            entity.Price = entity.Price;
            entity.Url = entity.Url;
            entity.Description = entity.Description;
            entity.IsApproved = entity.IsApproved;

           
            if (ishome==true)
            {
                entity.IsHome = false;
            }
            else
            {
                entity.IsHome = true;
            }

            entity.ImageUrl = entity.ImageUrl;
            entity.ProductCategories = entity.ProductCategories;
            _productService.Update(entity);
            return RedirectToAction("ProductList");
        }

        public IActionResult ProductDelete(int productId)
        {
            var entity = _productService.GetById(productId);
            _productService.Delete(entity);
          return  RedirectToAction("ProductList");
        }


    }
}
