namespace FurnitureCompany.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Article { get; set; } = null!; 
        public string Name { get; set; } = null!;    
        public decimal MinCost { get; set; }        
        
        public int ProductTypeId { get; set; }
        public int MainMaterialId { get; set; }

        public virtual ProductType ProductType { get; set; } = null!;
        public virtual Material MainMaterial { get; set; } = null!;
        public virtual ICollection<ProductWorkshop> ProductWorkshops { get; set; } = new List<ProductWorkshop>();
    }