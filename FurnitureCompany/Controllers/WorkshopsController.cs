using Microsoft.AspNetCore.Mvc;
using FurnitureCompany.Data;
using FurnitureCompany.Services;

namespace FurnitureCompany.Controllers
{
    public class WorkshopsController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly RawMaterialCalculator _calc;

        public WorkshopsController(DatabaseHelper db, RawMaterialCalculator calc)
        {
            _db = db;
            _calc = calc;
        }

        public IActionResult Index(int id)
        {
            var product = _db.GetProductById(id);
            if (product == null) return NotFound();

            var workshops = _db.GetWorkshopsByProductId(id);
            ViewBag.Product = product;
            return View(workshops);
        }

        [HttpPost]
        public IActionResult CalculateRawMaterial(int productId, int quantity, double param1, double param2)
        {
            var product = _db.GetProductById(productId);
            if (product == null) return NotFound();

            var result = _calc.Calculate(product.ProductTypeId, product.MainMaterialId, quantity, param1, param2);
            return Json(new { rawMaterial = result });
        }
    }
}
