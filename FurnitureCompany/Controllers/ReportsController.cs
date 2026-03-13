using Microsoft.AspNetCore.Mvc;
using FurnitureCompany.Data;
using FurnitureCompany.Models;

namespace FurnitureCompany.Controllers
{
    /// <summary>
    /// Контроллер модуля 4 — отчёты и аналитика по продукции.
    /// </summary>
    public class ReportsController : Controller
    {
        private readonly DatabaseHelper _db;

        public ReportsController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var report = _db.GetReportData();
            return View(report);
        }
    }
}
