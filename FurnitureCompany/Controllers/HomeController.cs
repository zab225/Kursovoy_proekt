using Microsoft.AspNetCore.Mvc;
using FurnitureCompany.Data;
using FurnitureCompany.Models;
using System.Collections.Generic;
using System;

namespace FurnitureCompany.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public HomeController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            List<Product> products;

            try
            {
                products = _dbHelper.GetProductsWithProductionTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в HomeController: {ex}");
                products = new List<Product>();
                ViewBag.Error = "Не удалось загрузить данные. Детали: " + ex.Message; // ← добавили детали
            }

            return View(products);
        }
    }
}