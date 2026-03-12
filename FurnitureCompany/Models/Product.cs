namespace FurnitureCompany.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Article { get; set; } = null!; 
        public string Name { get; set; } = null!;    
        public decimal MinPartnerCost { get; set; }        
        
        public int ProductTypeId { get; set; }
        public int MainMaterialId { get; set; }

        public virtual ProductType ProductType { get; set; } = null!;
        public virtual MaterialType MainMaterial { get; set; } = null!;
        public decimal ProductionTimeHours { get; set; }
    }
}