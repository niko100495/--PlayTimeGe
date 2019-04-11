namespace WebApi.Dtos.ForRelationship
{
    public class GeTAllConsoleRelation
    {
        public int actionId { get; set; }   
        public float CurrentPrice { get; set; }
        public int CurrentTime { get; set; }
        public float DefaultPrice { get; set; }
        public bool fixedTimer { get; set; }
        public int id { get; set; }
        public int maxTime { get; set; }
        public int maxUser { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public bool timerActive { get; set; }
        public string type { get; set; }
        public int users { get; set; }
        public string startTime { get; set; }
        public string Promotion { get; set; }
        public string PromotionPrice { get; set; }
        public int ConsoleNumber { get; set; }
        public int PauseDuration { get; set; }
    }
}