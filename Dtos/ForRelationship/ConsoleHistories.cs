namespace WebApi.Dtos.ForRelationship
{
    public class ConsoleHistories
    {
        public int ActionId { get; set; }
        public int UserId { get; set; } 
        public int UserAvatarId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public int UserNumber { get; set; }
        public string UserCreateDate { get; set; }
        public int UserFK_Role { get; set; }
        public int ConsoleId { get; set; }
        public string ConsoleName { get; set; } 
        public int ConsoleNumber { get; set; }
        public string ConsoleType { get; set; }
        public float ConsoleStandartPrice { get; set; }
        public float ConsoleCurrentPrice { get; set; }
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