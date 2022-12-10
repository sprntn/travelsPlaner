using System.Collections.Generic;
using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface IAdminService
    {
        public int addSiteCategory(SiteCategoriesDTO category);
        public int updateSiteCategory(SiteCategoriesDTO upCategory);
        public int deleteSiteCategory(int categoryId);
        public int addAdmin(AdminDTO admin);
        public int updateAdmin(AdminDTO upAdmin);
        public int deleteAdmin(string adminEmail);
        public List<AdminDTO> getAdmins();
        public AdminDTO getAdmin(string adminEmail);
        public List<ManagersDTO> getManagers();
    }
}
