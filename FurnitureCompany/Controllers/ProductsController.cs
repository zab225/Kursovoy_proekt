using Microsoft.AspNetCore.Mvc;
using FurnitureCompany.Data;
using FurnitureCompany.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace FurnitureCompany.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DatabaseHelper _db;

        public ProductsController(DatabaseHelper db)
        {
            _db = db;
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var vm = new ProductEditViewModel
            {
                ProductTypes = _db.GetProductTypes().Select(pt => new SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name
                }).ToList(),
                MaterialTypes = _db.GetMaterialTypes().Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                }).ToList()
            };
            return View(vm);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Article = vm.Article,
                    Name = vm.Name,
                    ProductTypeId = vm.ProductTypeId,
                    MainMaterialId = vm.MainMaterialId,
                    MinPartnerCost = vm.MinPartnerCost
                };
                _db.InsertProduct(product);
                return RedirectToAction("Index", "Home");
            }

            // если ошибка валидации – перезагрузить списки
            vm.ProductTypes = _db.GetProductTypes().Select(pt => new SelectListItem
            {
                Value = pt.Id.ToString(),
                Text = pt.Name,
                Selected = pt.Id == vm.ProductTypeId
            }).ToList();
            vm.MaterialTypes = _db.GetMaterialTypes().Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name,
                Selected = mt.Id == vm.MainMaterialId
            }).ToList();
            return View(vm);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _db.GetProductById(id);
            if (product == null) return NotFound();

            var vm = new ProductEditViewModel
            {
                Id = product.Id,
                Article = product.Article,
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                MainMaterialId = product.MainMaterialId,
                MinPartnerCost = product.MinPartnerCost,
                ProductTypes = _db.GetProductTypes().Select(pt => new SelectListItem
                {
                    Value = pt.Id.ToString(),
                    Text = pt.Name,
                    Selected = pt.Id == product.ProductTypeId
                }).ToList(),
                MaterialTypes = _db.GetMaterialTypes().Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name,
                    Selected = mt.Id == product.MainMaterialId
                }).ToList()
            };
            return View(vm);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductEditViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Id = vm.Id,
                    Article = vm.Article,
                    Name = vm.Name,
                    ProductTypeId = vm.ProductTypeId,
                    MainMaterialId = vm.MainMaterialId,
                    MinPartnerCost = vm.MinPartnerCost
                };
                _db.UpdateProduct(product);
                return RedirectToAction("Index", "Home");
            }

            // перезагрузить списки
            vm.ProductTypes = _db.GetProductTypes().Select(pt => new SelectListItem
            {
                Value = pt.Id.ToString(),
                Text = pt.Name,
                Selected = pt.Id == vm.ProductTypeId
            }).ToList();
            vm.MaterialTypes = _db.GetMaterialTypes().Select(mt => new SelectListItem
            {
                Value = mt.Id.ToString(),
                Text = mt.Name,
                Selected = mt.Id == vm.MainMaterialId
            }).ToList();
            return View(vm);
        }
    }
}