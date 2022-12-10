using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface IValidatorService
    {
        public bool visitValidator(VisitsDTO site);
        public bool userValidator(VisitedSite site);
        public string userValidation(UsersDTO user);
    }
}
