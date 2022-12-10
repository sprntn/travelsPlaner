using System;
using System.Collections.Generic;

namespace travels_server_side.Models
{
    public class TravelsDTO
    {
        public int travelId { get; set; }

        public string userEmail { get; set; }

        public DateTime beginDate { get; set; }
        public DateTime endDate { get; set; }

        //public int adultsNum { get; set; }
        //public int childrenNum { get; set; }
        public int participantsNum { get; set; }

        //public int budget { get; set; }

        //public VisitsDTO[] travelPlan  { get; set; }
        public List<VisitsDTO> travelPlan  { get; set; }
    }
}
