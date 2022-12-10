using System.Text.RegularExpressions;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class ValidatorService : IValidatorService
    {
        const int MAX_RATING = 5;
        const int MAX_NAME_LENGTH = 16;
        const int MIN_NAME_LENGTH = 4;
        const int MAX_PASSWORD_LENGTH = 20;
        const int MIN_PASSWORD_LENGTH = 8;

        public string userValidation(UsersDTO user)
        {
            if(
                string.IsNullOrEmpty(user.firstName) ||
                string.IsNullOrEmpty(user.lastName) ||
                string.IsNullOrEmpty(user.email) ||
                string.IsNullOrEmpty(user.password)
                )
            {
                return "must to fill in all the required fields";
            }
            if (user.firstName.Length > MAX_NAME_LENGTH || user.firstName.Length < MIN_NAME_LENGTH)
            {
                return "invalid first name";
            }
            if (user.lastName.Length > MAX_NAME_LENGTH || user.lastName.Length < MIN_NAME_LENGTH)
            {
                return "invalid last name";
            }
            if (emailValidation(user.email))
            {
                return "invalid email";
            }
            string errors = passwordValidation(user.password);
            if (errors != "")
            {
                return "invalid password: " + errors;
            }
            return null;
        }

        public bool userValidator(VisitedSite site)
        {
            throw new System.NotImplementedException();
        }

        public bool visitValidator(VisitsDTO visit)
        {
            if (visit == null //|| visit.userEmail == null 
                || visit.siteId < 0 || visit.rating > MAX_RATING || visit.rating < 0)
            {
                return false;
            }
            return true;
        }

        private bool emailValidation(string email)
        {
            string pattern = @"/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}/";
            Regex reg = new Regex(pattern);
            Match m = reg.Match(email);
            return m.Success;
        }

        private string passwordValidation(string password)
        {
            string errors = "";
            if (string.IsNullOrEmpty(password)) { errors += "empty password, "; }
            if(password.Length > MAX_PASSWORD_LENGTH) { errors += "too long password, "; }
            if(password.Length < MIN_PASSWORD_LENGTH) { errors += "too short password, "; }
            string pattern = @"^(?=.*[a-z])";
            Regex reg = new Regex(pattern);
            Match m = reg.Match(password);
            if (!m.Success) { errors += "password not contain lowercase letter, "; }
            pattern = @"^(?=.*[A-Z])";
            reg = new Regex(pattern);
            m = reg.Match(password);
            if (!m.Success) { errors += "password not contain uppercase letter, "; }
            pattern = @"^(?=.*[0-9])";
            reg = new Regex(pattern);
            m = reg.Match(password);
            if (!m.Success) { errors += "password not contain digit, "; }
            return errors;
            /*
            if(
                password == null || 
                password.Length > MAX_PASSWORD_LENGTH ||
                password.Length < MIN_PASSWORD_LENGTH
                )
            {
                return false;
            }
            */
            //string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,20}$";
            //string pattern = @"^(?=.*[a - z])(?=.*[A - Z])(?=.*\d)(?=.*[^\da - zA - Z]).{ 8,20}$";
            //Regex reg = new Regex(pattern);
            //Match m = reg.Match(password);
            //return m.Success;
        }
    }
}
