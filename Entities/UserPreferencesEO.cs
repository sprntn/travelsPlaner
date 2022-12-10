using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("preferences_tbl")]
    public class UserPreferencesEO
    {
        public string userEmail { get; set; }
        public int categoryId { get; set; }
        [Column("rating")]
        public int categoryRating { get; set; }
    }
}
