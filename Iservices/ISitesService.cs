using System.Collections.Generic;
using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface ISitesService
    {
        //public List<SitesDTO> DemoGetSites();
        //public SitesDTO DemoGetSiteById(int siteId);
        List<SitesDTO> getSuggestedSites(string userEmail);
        List<SitesDTO> getSuggestedSites();
        List<SitesDTO> getSites();
        SitesDTO getSite(int siteId);
        int updateSite(SitesDTO upSite);
        int deleteSite(int siteId);
        List<SiteCategoriesDTO> getCategories();
        public List<SitesDTO> getManagerSites(string email);
        public int addSite(SitesDTO site);

    }
}
