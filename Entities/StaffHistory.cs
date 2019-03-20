namespace WebApi.Entities
{
    public class StaffHistory
    {
        public int Id { get; set; } 
        public int UserId { get; set; }

        public string EnterTime { get; set; }

        public string ExitTime { get; set; }
        
        public int CountEnter { get; set; }

        
    }
}