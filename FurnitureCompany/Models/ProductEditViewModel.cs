using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FurnitureCompany.Models  // ← это правильно
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Article { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int ProductTypeId { get; set; }
        [Required]
        public int MainMaterialId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal MinPartnerCost { get; set; }
        public List<SelectListItem> ProductTypes { get; set; } = new();
        public List<SelectListItem> MaterialTypes { get; set; } = new();
    }
}