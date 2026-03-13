namespace FurnitureCompany.Models
{
    public class Workshop
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int WorkersCount { get; set; }
        public decimal ProductionTimeHours { get; set; }
    }
}
