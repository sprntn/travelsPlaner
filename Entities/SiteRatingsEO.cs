using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("sites_rating_tbl")]
    public class SiteRatingsEO
    {
        public int siteId { get; set; }
        public string userEmail { get; set; }
        public int rating { get; set; }
    }
}
