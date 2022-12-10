using System.Collections.Generic;

namespace travels_server_side.Models
{
    public class UserPreferencesDTO
    {
        public string userEmail { get; set; }
        public int categoryId { get; set; }
        public int categoryRating { get; set; }

        //public List<(int categoryId, string categoryName, int rating)> categoryRatings { get; set; }
        //public List<object> categoryRatings { get; set; }
    }
}
