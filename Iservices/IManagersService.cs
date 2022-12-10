using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface IManagersService
    {
        public int addManager(ManagersDTO manager);
        public int updateManager(ManagersDTO manager);
        public int deleteManager(string email);
        public ManagersDTO getManager(string email);
    }
}
