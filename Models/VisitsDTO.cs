using System;

namespace travels_server_side.Models
{
    public class VisitsDTO
    {
        public int visitId { get; set; }

        //public string userEmail { get; set; }
        public int siteId { get; set; }
        public int travelId { get; set; }
        public int rating { get; set; }

        public bool executed { get; set; }

        public DateTime datetime { get; set; }
        //public String datetime { get; set; }
    }
}
