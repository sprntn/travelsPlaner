using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("sites_tbl")]
    public class SitesEO
    {
        [Key]
        public int siteId { get; set; }
        public string siteName { get; set; }
        public string siteDescription { get; set; }
        public string imageSource { get; set; }
        public string webSite { get; set; }
        //[Column("siteAverageRating")]
        //public double averageRating { get; set; }//deleted
        //[ForeignKey("")]//להשלים
        public int mainCategoryFK { get; set; }
        //public int visitsNum { get; set; }//deleted
        //[ForeignKey("")]//להשלים
        public string managerEmailFK { get; set; }
        public int Latitude { get; set; }
        public int Longtitude { get; set; }
    }
}
