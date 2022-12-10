using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("users_tbl")]
    public class UsersEO
    {
        [Key]
        public string email { get; set; }
        //public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
    }
}
