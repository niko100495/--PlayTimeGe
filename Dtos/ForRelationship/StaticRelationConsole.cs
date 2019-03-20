using System.Collections.Generic;

namespace WebApi.Dtos.ForRelationship
{
    public class StaticRelationConsole
    {
        public string Date { get; set; }
        public List<AboutUserConsole> AboutUser { get; set; }
    }
    public class AboutUserConsole
    {
        public int UserId { get; set; }
        public double SumDay { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public List<DetailDayConsole> DetailDay  { get; set; }
    }
    public class DetailDayConsole
    {
        public int ConsoleId { get; set; } 
        public string ConsoleName { get; set; } 
        public int ConsoleNumber { get; set; } 
        public string ConsoleType { get; set; } 
        public float ConsoleStandartPrice { get; set; } 
        public string PlayStart { get; set; } 
        public string PlayFinish { get; set; } 
        public double Sum { get; set; }
    }
}