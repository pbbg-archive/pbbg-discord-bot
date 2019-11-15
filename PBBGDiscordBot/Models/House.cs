using System;
namespace GoTCBot.Models
{
    public class House
    {
        public int House_ID { get; set; }
        public string Name { get; set; }
        public int Role_FK { get; set; }
        public int Owner_FK { get; set; }
        public string Identifier { get; set; }
    }
}
