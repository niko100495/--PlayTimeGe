namespace WebApi.Entities
{
    public class ConsolePlay
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int ConsoleId { get; set; }
        public int Gamer { get; set; }  
        public float Price { get; set; }
        public bool isFixedTime { get; set; }
        public string FixedTime { get; set; }
        public string PauseStart { get; set; }
        public string PauseFinish { get; set; }
        public int PauseDuration { get; set; }
        public string PlayStart { get; set; }
        public string PlayFinish { get; set; }
        public double Paid { get; set; }
    }
}