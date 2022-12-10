using System;

namespace travels_server_side.Models
{
    public class SelectedSite
    {
        public int siteId { get; set; }
        public string siteName { get; set; }
        public string siteDescription { get; set; }
        public string imageSource { get; set; }
        public string webSite { get; set; }
        //public double siteAverageRating { get; set; }
        public DateTime datetime { get; set; }

        public int visitId { get; set; }
    }
}
