using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using travels_server_side.DBcontext;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class ManagersService: IManagersService
    {
        private readonly TravelsDbContext _travelDbContext;
        private readonly IMapper _mapper;

        public ManagersService(TravelsDbContext travelsDbContext, IMapper mapper)
        {
            _travelDbContext = travelsDbContext;
            _mapper = mapper;
        }

        public bool AvailableEmail(string managerEmail)
        {
            if (_travelDbContext.managers.Any(m => m.email == managerEmail))
            {
                return false;
            }
            return true;
        }

        public int addManager(ManagersDTO manager)
        {
            if (!availableEmail(manager.email))//the email exists in the database
            {
                return 3;
            }

            ManagersEO newManager = _mapper.Map<ManagersEO>(manager);
            if(newManager == null)
            {
                return 2;
            }
            _travelDbContext.managers.Add(newManager);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        private bool availableEmail(string email)
        {
            if (_travelDbContext.managers.Any(a => a.email == email))
            {
                return false;
            }
            return true;
        }

        public int updateManager(ManagersDTO manager)
        {
            //validation here

            ManagersEO foundManager = _travelDbContext.managers.FirstOrDefault(m => m.email == manager.email);
            if (foundManager == null)//not found
            {
                return 2;
            }
            PropertyInfo[] properties = typeof(ManagersDTO).GetProperties();
            foreach(PropertyInfo property in properties)
            {
                if(property.GetValue(manager) != null)
                {
                    property.SetValue(foundManager, property.GetValue(manager));
                }
            }
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        private bool isSiteToManager(string email)
        {
            List<SitesEO> managerSites = getManagerSites(email);
            foreach (SitesEO site in managerSites)
            {
                VisitsEO visit = _travelDbContext.visits.Where(v => v.siteIdFK == site.siteId).FirstOrDefault();
                if(visit != null)
                {
                    return false;
                }
            }
            return true;
        }

        private List<SitesEO> getManagerSites(string email)
        {
            List<SitesEO> managerSites = _travelDbContext.sites.Where(s => s.managerEmailFK == email).ToList();
            return managerSites;
        }

        public int deleteManager(string email)
        {
            if (email == null)
            {
                return 4;
            }
            ManagersEO manager = _travelDbContext.managers.FirstOrDefault(m => m.email == email);
            if(manager == null)
            {
                return 3;
            }
            /*
            List<SitesEO> managerSites = _travelDbContext.sites.Where(s => s.managerEmailFK == email).ToList();
            if(managerSites.Any(s => s.visitsNum > 0))
            {
                //there are sites that cannot be deleted, so the manager is not deleteable
                return 2;
            }
            */
            if (!isSiteToManager(email))
            {
                //there are sites that cannot be deleted, so the manager is not deleteable
                return 2;
            }
            List<SitesEO> managerSites = getManagerSites(email);
            foreach (SitesEO site in managerSites)
            {
                _travelDbContext.sites.Remove(site);
            }
            _travelDbContext.managers.Remove(manager);
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public ManagersDTO getManager(string email)
        {
            if (email == null)
            {
                return null;
            }
            ManagersDTO manager = _travelDbContext.managers.Where(m => m.email == email).Select(manager => _mapper.Map<ManagersDTO>(manager)).FirstOrDefault();

            return manager;
        }

        //get managers in adminService

        
    }
}
