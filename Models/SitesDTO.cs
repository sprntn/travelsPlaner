using System;

namespace travels_server_side.Models
{
    public class SitesDTO
    {
        public int siteId { get; set; }
        public string siteName { get; set; }
        public string siteDescription { get; set; }
        public string imageSource { get; set; }
        public string webSite { get; set; }
        public double siteAverageRating { get; set; }
        //public int visitsNum { get; set; }//deleted
        public string managerEmail { get; set; }
        public int mainCategoryFK { get; set; }
    }
}
