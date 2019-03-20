using System.Collections.Generic;

namespace WebApi.Dtos.ForRelationship
{
    public class StaticRelation
    {
        public string Date { get; set; }
        public List<AboutUser> AboutUser { get; set; }
    }
    public class AboutUser
    {
        public int Id { get; set; }
        public float SumDay { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public List<DetailDay> DetailDay  { get; set; }
    }
    public class DetailDay
    {
        public int ProductId { get; set; } 
        public string ProductName { get; set; }
        public float ProductPrice { get; set; } 
        public int QuantityProduct { get; set; } 
        public float Sum { get; set; } 
        public string DateBuy { get; set; } 
    }
}