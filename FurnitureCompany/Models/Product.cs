namespace FurnitureCompany.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public decimal MinPartnerCost { get; set; }
        public int ProductionTimeHours { get; set; }
    }
}