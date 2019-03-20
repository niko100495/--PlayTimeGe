using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Trade
    {
        
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MagazineId { get; set; }
        public int QuantityProduct { get; set; }
        public string BuyDate { get; set; }
    }
}