using System.Collections.Generic;
using travels_server_side.Models;

namespace travels_server_side.Iservices
{
    public interface IUsersService
    {
        int addUser(UsersDTO user);
        int updateUser(UsersDTO user);
        int deleteUser(string userEmail);
        bool AvailableEmail(string userEmail);
        UsersDTO getUserByEmail(string userEmail);
        List<UserPreferencesDTO> getUserPref(string userEmail);
        //public List<UserPreferencesDTO> getPreferences();
        public int addUserPref(UserPreferencesDTO pref);
        public int updateUserPref(UserPreferencesDTO pref);
        public int deletePreference(UserPreferencesDTO pref);
        List<UsersDTO> getUsers();
        List<SiteRatingsDTO> getUserRatings(string userEmail);
        int addUserRating(SiteRatingsDTO rating);
        int updateRating(SiteRatingsDTO rating);
    }
}
