namespace WebApi.Dtos
{
    public class ConsolePlayDto
    {
        public int UserId { get; set; } 
        public int ConsoleId { get; set; }
        public int Gamer { get; set; }  
        public float Price { get; set; }
        public bool isFixedTime { get; set; }
        public string FixedTime { get; set; }
    }
}