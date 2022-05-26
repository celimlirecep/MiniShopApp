using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShopApp.Entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Kategori ismi zorunludur!")]
        [StringLength(30,MinimumLength =5,ErrorMessage ="5-30 krarkter arasında giriniz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Kategori açıklaması zorunludur!")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "5-30 krarkter arasında giriniz")]
        public string Description { get; set; }
        public string Url { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
