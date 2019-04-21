using System;

namespace WebApi.Dtos.ForRelationship
{
    public class getLiveWorkInfo
    {
        public string Paid { get; set; }
        public DateTime Time { get; set; }
        public float Price { get; set; }
        public bool isFixedTime { get; set; }
        public string FixedTime { get; set; }
        public string FinishTime { get; set; }

    }
}