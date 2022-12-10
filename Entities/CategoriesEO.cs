using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travels_server_side.Entities
{
    [Table("categories_tbl")]
    public class CategoriesEO
    {
        [Key]
        public int categoryId { get; set; }
        public string categoryName { get; set; }
    }
}
