using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("travels_tbl")]
    public class TravelsEO
    {
        [Key]
        public int travelId { get; set; }

        public string userEmailFK { get; set; }

        public DateTime beginDate { get; set; }
        public DateTime endDate { get; set; }

        //public int adultsNum { get; set; }
        //public int childrenNum { get; set; }
        public int participantsNum { get; set; }

        //public int budget { get; set; }

        //public VisitsDTO[] travelPlan { get; set; }//אי אפשר ליצור שדה מקביל במאגר נתונים
    }
}
