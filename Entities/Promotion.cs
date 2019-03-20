namespace WebApi.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public string PromotionName { get; set; }   
        public string PromotionDesc { get; set; }
        public int Price { get; set; }
        public bool AllTime { get; set; } 

        public string DateStart { get; set; }
        public string DateEnd { get; set; }  
        public string HourStart { get; set; }
        public string HourEnd { get; set; }
    }
}