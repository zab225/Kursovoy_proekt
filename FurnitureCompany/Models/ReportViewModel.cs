namespace FurnitureCompany.Models
{
    /// <summary>
    /// Данные для страницы отчётов (модуль 4).
    /// </summary>
    public class ReportViewModel
    {
        /// <summary>Общее количество продукции</summary>
        public int TotalProductsCount { get; set; }

        /// <summary>Суммарная мин. стоимость всей продукции (₽)</summary>
        public decimal TotalMinCost { get; set; }

        /// <summary>Средняя мин. стоимость за единицу (₽)</summary>
        public decimal AverageMinCost { get; set; }

        /// <summary>Количество по типам продукции</summary>
        public List<ProductTypeCount> ProductsByType { get; set; } = new();

        /// <summary>Топ продукции по стоимости</summary>
        public List<Product> TopProductsByCost { get; set; } = new();
    }

    public class ProductTypeCount
    {
        public string TypeName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
