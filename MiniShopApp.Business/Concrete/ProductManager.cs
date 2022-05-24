using MiniShopApp.Business.Abstract;
using MiniShopApp.Data.Abstract;
using MiniShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShopApp.Business.Concrete
{
    public class ProductManager : IProductService,IValidator<Product>
    {
        private IProductRepository _productRepository;
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

   

        public void Create(Product entity)
        {
            throw new NotImplementedException();
        }


        public bool Create(Product product, int[] categorieIds)
        {
            if (Validation(product))
            {
                _productRepository.Create(product, categorieIds);
                return true;
            }
            return false;
           
        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public List<Product> GetAll()
        {
            //Burada ürünlerin listelenmesi sağlanıyor.
            //Fakat ürün listeleme yapan metot çalıştırılmadan önce
            //Burada çeşitli iş kuralları uygulanacak.(Validation vb.)
            //Bunu daha sonra yazacağız.
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string name, int page, int pageSize)
        {
            return _productRepository.GetProductsByCategory(name, page, pageSize);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return _productRepository.GetSearchResult(searchString);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public void Update(Product product, int[] categories)
        {
             _productRepository.Update(product, categories);
        }
        public string ErrorMessage { get; set; }
        public bool Validation(Product Entity)
        {
            var isValid = true;
            if (string.IsNullOrEmpty(Entity.Name))
            {
                ErrorMessage += $"Ürün adı boş geçilemez!\n";
                isValid = false;
            }
            if (Entity.Price<=0)
            {
                ErrorMessage += $"Ürün fiyatı 0 dan büyük olmalıdır.!\n";
                isValid = false;
            }
            return isValid;
        }
    }
}
