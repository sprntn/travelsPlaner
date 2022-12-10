using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using travels_server_side.DBcontext;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class AdminService: IAdminService
    {
        private readonly TravelsDbContext _travelDbContext;
        private readonly IMapper _mapper;

        public AdminService(TravelsDbContext travelsDbContext, IMapper mapper)
        {
            _travelDbContext = travelsDbContext;
            _mapper = mapper;
        }

        //getSiteCategories in sitesService

        public int addSiteCategory(SiteCategoriesDTO category)
        {
            //validation here

            if(_travelDbContext.categories.Any(c => c.categoryName == category.categoryName))//the category exists in the database
            {
                return 3;
            }
            CategoriesEO newCategory = _mapper.Map<CategoriesEO>(category);
            if(newCategory == null)//not valid category
            {
                return 2;
            }
            _travelDbContext.categories.Add(newCategory);
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int updateSiteCategory(SiteCategoriesDTO upCategory)
        {
            //validation here

            CategoriesEO category = _travelDbContext.categories.FirstOrDefault(c =>  c.categoryId == upCategory.categoryId);
            if(category == null)//not found
            {
                return 2;
            }
            //category = _mapper.Map<CategoriesEO>(upCategory);
            category.categoryName = upCategory.categoryName;

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int deleteSiteCategory(int categoryId)
        {
            if (!noUsedCategory(categoryId))
            {
                return 3;
            }
            CategoriesEO category = _travelDbContext.categories.FirstOrDefault(c => c.categoryId == categoryId);
            if(category == null)//not found
            {
                return 2;
            }
            _travelDbContext.categories.Remove(category);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        private bool noUsedCategory(int categoryId)
        {
            if(_travelDbContext.sites.Any(s => s.mainCategoryFK == categoryId))
            {
                return false;
            }
            return true;
        }

        public int addAdmin(AdminDTO admin)
        {
            //validation here

            if(!availableEmail(admin.email))//the email exists in the database
            {
                return 3;
            }
            AdminEO newAdmin = _mapper.Map<AdminEO>(admin);
            if(newAdmin == null)//not valid admin
            {
                return 2;
            }
            _travelDbContext.admins.Add(newAdmin);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        private bool availableEmail(string adminEmail)
        {
            if (_travelDbContext.admins.Any(a => a.email == adminEmail))
            {
                return false;
            }
            return true;
        }

        public int updateAdmin(AdminDTO upAdmin)
        {
            //validation here

            AdminEO admin = _travelDbContext.admins.FirstOrDefault(c => c.email == upAdmin.email);
            if(admin == null)//not found
            {
                return 2;
            }
            admin.password = upAdmin.password;

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int deleteAdmin(string adminEmail)
        {
            if(adminEmail == null)
            {
                return 3;
            }
            AdminEO admin = _travelDbContext.admins.FirstOrDefault(a => a.email == adminEmail);
            if(admin == null)//admin not found
            {
                return 2;
            }
            _travelDbContext.admins.Remove(admin);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public AdminDTO getAdmin(string adminEmail)
        {
            //1st way
            //AdminDTO foundAdmin = _mapper.Map<AdminDTO>(_travelDbContext.admins.FirstOrDefault(a => a.email == adminEmail));

            //2nd way
            AdminDTO foundAdmin = _travelDbContext.admins.Where(a => a.email == adminEmail).Select(admin => _mapper.Map<AdminDTO>(admin)).FirstOrDefault();

            return foundAdmin;
        }

        public List<AdminDTO> getAdmins()
        {
            List<AdminDTO> admins = _travelDbContext.admins.Select(admin => _mapper.Map<AdminDTO>(admin)).ToList();
            return admins;
        }

        public List<ManagersDTO> getManagers()
        {
            List<ManagersDTO> managers = _travelDbContext.managers.Select(managers => _mapper.Map<ManagersDTO>(managers)).ToList();
            return managers;
        }
    }
}
