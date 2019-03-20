using System;
using System.Collections;
using System.Collections.Generic;

namespace WebApi.Dtos
{
    public class PromotionRelation
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }   
        public string PromotionDesc { get; set; }
        public float Price { get; set; }
        public bool AllTime { get; set; } 

        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string HourStart { get; set; }
        public string HourEnd { get; set; }
        public List<ConsoleProm> Console { get; set; }
    }
    public class ConsoleProm
    {
        public int ConsoleId { get; set; }
        public string ConsoleName { get; set; }
        public int ConsoleNumber { get; set; }
        public string Type { get; set; }
        public float StandartPrice { get; set; }
        public float CurrentPrice { get; set; }
    }
}