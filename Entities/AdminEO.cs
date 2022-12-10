using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("admin_tbl")]
    public class AdminEO
    {
        [Key]
        public string email { get; set; }
        public string password { get; set; }
    }
}
