using System.Collections.ObjectModel;
using WebApi.Entities;

namespace WebApi.Dtos
{
    public class ConsoleDto
    {
        public int Id { get; set; }
        public string ConsoleName { get; set; } 
        public int ConsoleNumber { get; set; }
        public string Type { get; set; }
        public float StandartPrice { get; set; }
        

        
    }
}