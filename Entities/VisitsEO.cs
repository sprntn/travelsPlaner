using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("visits_tbl")]
    public class VisitsEO
    {
        [Key]
        public int visitId { get; set; }


        [Column("siteIdFK")]
        //[ForeignKey("travel")]
        public int siteIdFK { get; set; }//The key composed by the fields siteid and useremail//canceled

        [Column("travelIdFK")]
        //[ForeignKey("travel")]
        public int travelIdFK { get; set; }

        //[Column("userEmailFK")]
        //[ForeignKey("user")]
        //public string userEmailFK { get; set; }

        public int rating { get; set; }

        public bool executed { get; set; }


        public DateTime date { get; set; }
    }
}
