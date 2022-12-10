using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using travels_server_side.DBcontext;
using travels_server_side.Entities;
using travels_server_side.Iservices;
using travels_server_side.Models;

namespace travels_server_side.Services
{
    public class UsersService : IUsersService
    {
        private readonly TravelsDbContext _travelDbContext;

        public UsersService(TravelsDbContext travelsDbContext)
        {
            _travelDbContext = travelsDbContext;
        }

        public bool AvailableEmail(string userEmail)
        {
            if(_travelDbContext.users.Any(u => u.email == userEmail))
            {
                return false;
            }
            return true;
        }

        public List<UsersDTO> getUsers() 
        {
            List<UsersDTO> users = _travelDbContext.users.Select(u => new UsersDTO() {
                address = u.address,
                email = u.email,
                firstName = u.firstName,
                lastName = u.lastName,
                password = u.password,
                phone = u.phone
            }).ToList();
            return users;
        }

        public int addUser(UsersDTO user)
        {
            //for now, need to switch to mapper
            UsersEO userAdd = new UsersEO()
            {
                email = user.email,
                password = user.password,
                firstName = user.firstName,
                lastName = user.lastName,
                phone = user.phone,
                address = user.address
            };
            if(userAdd == null)
            {
                return 2;
            }
            _travelDbContext.users.Add(userAdd);
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int updateUser(UsersDTO upUser)
        {
            UsersEO user = _travelDbContext.users.FirstOrDefault(u => u.email == upUser.email);
            if (user == null)//not found
            {
                return 2;
            }
            /*
            PropertyInfo[] properties = typeof(UsersDTO).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(upUser) != null )//&& property.GetValue(upUser) != property.GetValue(user))
                {
                    property.SetValue(user, property.GetValue(upUser));
                }
            }
            */
            
            user.phone = upUser.phone;
            user.address = upUser.address;
            user.password = upUser.password;
            user.firstName = upUser.firstName;
            user.lastName = upUser.lastName;

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int deleteUser(string userEmail)
        {
            UsersEO user = _travelDbContext.users.FirstOrDefault(u => u.email == userEmail);
            if (user == null)//site not found
            {
                return 2;
            }
            _travelDbContext.users.Remove(user);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public UsersDTO getUserByEmail(string userEmail)
        {
            UsersEO userFound = _travelDbContext.users.FirstOrDefault(u => u.email == userEmail);
            if (userFound != null)
            {
                UsersDTO userReturn = new UsersDTO()
                {
                    email = userFound.email,
                    password = userFound.password,
                    firstName = userFound.firstName,
                    lastName = userFound.lastName,
                    phone = userFound.phone,
                    address = userFound.address,
                };
                return userReturn;
            }
            return null;
        }

        /*
        public List<UserPreferencesDTO> getPreferences()
        {
            List<UserPreferencesDTO> preferences = _travelDbContext.preferences.Select(p => new UserPreferencesDTO()
            {
                userEmail = p.userEmail,
                categoryId = p.categoryId,
                categoryRating = p.categoryRating
            }).ToList();
            return preferences;
        }
        */

        //public List<UserPreferencesDTO> getPreferences()
        //{

        //}

        public List<UserPreferencesDTO> getUserPref(string email)
        {
            List<UserPreferencesDTO> userPreferences = _travelDbContext.preferences.
                Where(p => p.userEmail == email).Select(p => new UserPreferencesDTO()
                {
                    userEmail = p.userEmail,
                    categoryId = p.categoryId,
                    categoryRating = p.categoryRating
                }).ToList();
            return userPreferences;
        }

        private bool isNewPreference(UserPreferencesDTO pref)
        {
            if (_travelDbContext.preferences.Any(p => p.userEmail == pref.userEmail && p.categoryId == pref.categoryId))
            {
                return false;
            }
            
            return true;
        }

        public int addUserPref(UserPreferencesDTO pref)
        {
            if (!isNewPreference(pref))
            {
                return updateUserPref(pref);
            }
            UserPreferencesEO newPref = new UserPreferencesEO()
            {
                userEmail = pref.userEmail,
                categoryId = pref.categoryId,
                categoryRating = pref.categoryRating
            };
            if (newPref == null)
            {
                return 2;
            }
            _travelDbContext.preferences.Add(newPref);
            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int updateUserPref(UserPreferencesDTO pref)
        {
            UserPreferencesEO editPref = _travelDbContext.preferences.
                FirstOrDefault(p => p.userEmail == pref.userEmail && p.categoryId == pref.categoryId);
            if(editPref == null)
            {
                return 2;
            }
            editPref.categoryRating = pref.categoryRating;

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int deletePreference(UserPreferencesDTO pref)
        {
            UserPreferencesEO deletePref = _travelDbContext.preferences.
                FirstOrDefault(p => p.userEmail == pref.userEmail && p.categoryId == pref.categoryId);
            if (deletePref == null)
            {
                return 2;
            }

            _travelDbContext.preferences.Remove(deletePref);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public List<SiteRatingsDTO> getUserRatings(string userEmail)
        {
            List<SiteRatingsDTO> ratings = _travelDbContext.ratings.Where(r => r.userEmail == userEmail).
                Select(r => new SiteRatingsDTO()
                {
                    userEmail = r.userEmail,
                    siteId = r.siteId,
                    rating = (int)r.rating
                }).ToList();
            return ratings;
        }

        private bool isNewRating(SiteRatingsDTO rating)
        {
            if(_travelDbContext.ratings.Any(r => r.siteId == rating.siteId && r.userEmail == rating.userEmail))
            {
                return false;
            }
            return true;
        }

        public int addUserRating(SiteRatingsDTO rating)
        {
            if (!isNewRating(rating))
            {
                return updateRating(rating);
            }
            SiteRatingsEO newRating = new SiteRatingsEO()
            {
                rating = rating.rating,
                siteId = rating.siteId,
                userEmail = rating.userEmail
            };
            _travelDbContext.ratings.Add(newRating);

            int check = _travelDbContext.SaveChanges();
            return check;
        }

        public int updateRating(SiteRatingsDTO rating)
        {
            SiteRatingsEO upRating = _travelDbContext.ratings.FirstOrDefault(r => r.siteId == rating.siteId && r.userEmail == rating.userEmail);
            if(upRating == null)
            {
                return 2;
            }
            if(upRating.rating == rating.rating)
            {
                return 1;
            }
            upRating.rating = rating.rating;

            int check = _travelDbContext.SaveChanges();
            return check;
        }
    }
}
