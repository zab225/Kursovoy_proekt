namespace FurnitureCompany.Models
{
    public class MaterialType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal LossPercent { get; set; }
    }
}