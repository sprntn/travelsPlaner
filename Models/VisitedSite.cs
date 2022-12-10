using System;

namespace travels_server_side.Models
{
    public class VisitedSite
    {
        public string userEmail { get; set; }
        public int siteId { get; set; }
        public string siteName { get; set; }
        //public string date { get; set; }
        //public DateTime date { get; set; }
        public string imageSrc { get; set; }
        public string siteDescription { get; set; }
        public int rating { get; set; }
    }
}
/*
 
public userEmail!: string;
    public SiteId!: number;
    public rating!: number;
    public date!: string;
    public imageSrc!: string;
    public siteDescription!: string;
    public siteName!: string; 

 */