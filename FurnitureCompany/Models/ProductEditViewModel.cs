using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureCompany.Models  // ← это правильно
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Артикул")]
        public string Article { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Тип продукции")]
        public int ProductTypeId { get; set; }
        [Required]
        [Display(Name = "Основной материал")]
        public int MainMaterialId { get; set; }
        [Range(0, double.MaxValue)]
        [Display(Name = "Мин. стоимость (₽)")]
        public decimal MinPartnerCost { get; set; }
        public List<SelectListItem> ProductTypes { get; set; } = new();
        public List<SelectListItem> MaterialTypes { get; set; } = new();
    }
}