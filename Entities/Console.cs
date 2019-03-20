using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace WebApi.Entities
{
    public class Console
    {
        public int Id { get; set; }
        public string ConsoleName { get; set; } 
        public int ConsoleNumber { get; set; }
        public string Type { get; set; }
        public float StandartPrice { get; set; }
        public float CurrentPrice { get; set; }
        
        
       

    }
}
        